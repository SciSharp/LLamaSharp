using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LLama.Exceptions;
using System.Linq;
using LLama.Extensions;

namespace LLama
{
    using llama_token = Int32;
    public class LLamaModel: IChatModel, IDisposable
    {
        LLamaParams _params;
        SafeLLamaContextHandle _ctx;
        string _path_session;
        List<llama_token> _session_tokens;
        List<llama_token> _embed_inp;
        int _n_ctx;
        List<llama_token> _inp_pfx;
        List<llama_token> _inp_sfx;
        List<llama_token> _llama_token_newline;
        List<llama_token> _last_n_tokens;
        bool _is_interacting;
        bool _is_antiprompt;
        bool _input_echo;

        // HACK - because session saving incurs a non-negligible delay, for now skip re-saving session
        // if we loaded a session with at least 75% similarity. It's currently just used to speed up the
        // initial prompt so it doesn't need to be an exact match.
        bool _need_to_save_session;
        int _n_past;
        int _n_remain;
        int _n_consumed;
        int _n_session_consumed;
        List<llama_token> _embed;

        // params related to chat API only
        bool _first_time_chat = true;

        public string Name { get; set; }
        public SafeLLamaContextHandle NativeHandle => _ctx;

        public LLamaModel(string model_path, string model_name, bool echo_input = false, bool verbose = false, int seed = 0, int n_threads = -1, int n_predict = -1,
            int n_parts = -1, int n_ctx = 512, int n_batch = 512, int n_keep = 0, int n_gpu_layers = -1,
            Dictionary<llama_token, float> logit_bias = null, int top_k = 40, float top_p = 0.95f,
            float tfs_z = 1.00f, float typical_p = 1.00f, float temp = 0.80f, float repeat_penalty = 1.10f,
            int repeat_last_n = 64, float frequency_penalty = 0.00f, float presence_penalty = 0.00f,
            int mirostat = 0, float mirostat_tau = 5.00f, float mirostat_eta = 0.10f, string prompt = "",
            string path_session = "", string input_prefix = "", string input_suffix = "",
            List<string> antiprompt = null, string lora_adapter = "", string lora_base = "",
            bool memory_f16 = true, bool random_prompt = false, bool use_color = false, bool interactive = false,
            bool embedding = false, bool interactive_first = false, bool prompt_cache_all = false, bool instruct = false, bool penalize_nl = true,
            bool perplexity = false, bool use_mmap = true, bool use_mlock = false, bool mem_test = false,
            bool verbose_prompt = false) : this(new LLamaParams(seed, n_threads, n_predict, n_parts, n_ctx, n_batch, 
                n_keep, n_gpu_layers, logit_bias, top_k, top_p, tfs_z, typical_p, temp, repeat_penalty, repeat_last_n, frequency_penalty, 
                presence_penalty, mirostat, mirostat_tau, mirostat_eta, model_path, prompt, path_session, input_prefix, 
                input_suffix, antiprompt, lora_adapter, lora_base, memory_f16, random_prompt, use_color, interactive, embedding, 
                interactive_first, prompt_cache_all, instruct, penalize_nl, perplexity, use_mmap, use_mlock, mem_test, verbose_prompt), model_name, echo_input, verbose)
        {
            
        }

        public unsafe LLamaModel(LLamaParams @params, string name = "", bool echo_input = false, bool verbose = false, string encoding = "UTF-8")
        {
            Name = name;
            _params = @params;
            _ctx = Utils.llama_init_from_gpt_params(ref _params);

            // Add a space in front of the first character to match OG llama tokenizer behavior
            _session_tokens = new List<llama_token>();

            _path_session = @params.path_session;
            if (!string.IsNullOrEmpty(_path_session))
            {
                if (verbose)
                {
                    Logger.Default.Info($"Attempting to load saved session from '{_path_session}'");
                }

                if (!File.Exists(_path_session))
                {
                    Logger.Default.Warn("Session file does not exist, will create.");
                }

                llama_token[] session_tokens = new llama_token[@params.n_ctx];
                ulong n_token_count_out = 0;
                if (!NativeApi.llama_load_session_file(_ctx, _path_session, session_tokens, (ulong)@params.n_ctx, &n_token_count_out))
                {
                    throw new RuntimeError($"Failed to load session file {_path_session}");
                }
                _session_tokens = session_tokens.Take((int)n_token_count_out).ToList();
                if (verbose)
                {
                    Logger.Default.Info($"Loaded a session with prompt size of {_session_tokens.Count} tokens");
                }
            }

            _n_ctx = NativeApi.llama_n_ctx(_ctx);

            WithPrompt(_params.prompt);

            // prefix & suffix for instruct mode
            _inp_pfx = Utils.llama_tokenize(_ctx, "\n\n### Instruction:\n\n", true, encoding);
            _inp_sfx = Utils.llama_tokenize(_ctx, "\n\n### Response:\n\n", false, encoding);

            // in instruct mode, we inject a prefix and a suffix to each input by the user
            if (_params.instruct)
            {
                _params.interactive_first = true;
                _params.antiprompt.Add("### Instruction:\n\n");
            }

            // enable interactive mode if reverse prompt or interactive start is specified
            if (_params.antiprompt.Count != 0 || _params.interactive_first)
            {
                _params.interactive = true;
            }

            // determine newline token
            _llama_token_newline = Utils.llama_tokenize(_ctx, "\n", false, encoding);

            if (_params.verbose_prompt)
            {
                Logger.Default.Info("\n");
                Logger.Default.Info($"prompt: '{_params.prompt}'");
                Logger.Default.Info($"number of tokens in prompt = {_embed_inp.Count}");
                for (int i = 0; i < _embed_inp.Count; i++)
                {
                    Logger.Default.Info($"{_embed_inp[i]} -> '{NativeApi.llama_token_to_str(_ctx, _embed_inp[i])}'");
                }
                if (_params.n_keep > 0)
                {
                    Logger.Default.Info($"static prompt based on n_keep: '");
                    for (int i = 0; i < _params.n_keep; i++)
                    {
                        Logger.Default.Info($"{NativeApi.llama_token_to_str(_ctx, _embed_inp[i])}");
                    }
                    Logger.Default.Info("\n");
                }
                Logger.Default.Info("\n");
            }

            if (_params.interactive && verbose)
            {
                Logger.Default.Info("interactive mode on.");
            }
            if (verbose)
            {
                Logger.Default.Info($"sampling: repeat_last_n = {_params.repeat_last_n}, " +
                $"repeat_penalty = {_params.repeat_penalty}, presence_penalty = {_params.presence_penalty}, " +
                $"frequency_penalty = {_params.frequency_penalty}, top_k = {_params.top_k}, tfs_z = {_params.tfs_z}," +
                $" top_p = {_params.top_p}, typical_p = {_params.typical_p}, temp = {_params.temp}, mirostat = {_params.mirostat}," +
                $" mirostat_lr = {_params.mirostat_eta}, mirostat_ent = {_params.mirostat_tau}");
                Logger.Default.Info($"generate: n_ctx = {_n_ctx}, n_batch = {_params.n_batch}, n_predict = {_params.n_predict}, " +
                    $"n_keep = {_params.n_keep}");
                Logger.Default.Info("\n");
            }

            _last_n_tokens = Enumerable.Repeat(0, _n_ctx).ToList();

            if (_params.interactive)
            {
                if (verbose)
                {
                    Logger.Default.Info("== Running in interactive mode. ==");
                }
                _is_interacting = _params.interactive_first;
            }

            _is_antiprompt = false;
            _input_echo = echo_input;
            _n_past = 0;
            _n_remain = _params.n_predict;
            _n_consumed = 0;
            _n_session_consumed = 0;
            _embed = new List<llama_token>();
        }

        public LLamaModel WithPrompt(string prompt, string encoding = "UTF-8")
        {
            _params.prompt = prompt.Insert(0, " ");
            _embed_inp = Utils.llama_tokenize(_ctx, _params.prompt, true, encoding);

            if (_embed_inp.Count > _n_ctx - 4)
            {
                throw new ArgumentException($"prompt is too long ({_embed_inp.Count} tokens, max {_n_ctx - 4})");
            }

            ulong n_matching_session_tokens = 0;
            if (_session_tokens.Count > 0)
            {
                foreach (var id in _session_tokens)
                {
                    if (n_matching_session_tokens >= (ulong)_embed_inp.Count || id != _embed_inp[(int)n_matching_session_tokens])
                    {
                        break;
                    }
                    n_matching_session_tokens++;
                }
                if (n_matching_session_tokens >= (ulong)_embed_inp.Count)
                {
                    Logger.Default.Info("Session file has exact match for prompt!");
                }
                else if (n_matching_session_tokens < (ulong)(_embed_inp.Count / 2))
                {
                    Logger.Default.Warn($"session file has low similarity to prompt ({n_matching_session_tokens} " +
                        $"/ {_embed_inp.Count} tokens); will mostly be reevaluated.");
                }
                else
                {
                    Logger.Default.Info($"Session file matches {n_matching_session_tokens} / {_embed_inp.Count} " +
                        $"tokens of prompt.");
                }
            }
            // number of tokens to keep when resetting context
            if (_params.n_keep < 0 || _params.n_keep > (int)_embed_inp.Count || _params.instruct)
            {
                _params.n_keep = _embed_inp.Count;
            }
            if (_embed_inp.Count > _n_ctx - 4)
            {
                throw new ArgumentException($"prompt is too long ({_embed_inp.Count} tokens, max {_n_ctx - 4})");
            }
            _need_to_save_session = !string.IsNullOrEmpty(_path_session) && n_matching_session_tokens < (ulong)(_embed_inp.Count * 3 / 4);
            return this;
        }

        public LLamaModel WithPromptFile(string promptFileName)
        {
            return WithPrompt(File.ReadAllText(promptFileName));
        }

        private string ProcessTextBeforeInfer(string text, string encoding)
        {
            if (!string.IsNullOrEmpty(_params.input_prefix))
            {
                text = _params.input_prefix + text;
            }
            if (!text.EndsWith("\n"))
            {
                text += "\n";
            }
            if (text.Length > 1)
            {
                // append input suffix if any
                if (!string.IsNullOrEmpty(_params.input_suffix))
                {
                    text += _params.input_suffix;
                    Console.Write(_params.input_suffix);
                }

                // instruct mode: insert instruction prefix
                if (_params.instruct && !_is_antiprompt)
                {
                    _n_consumed = _embed_inp.Count;
                    _embed_inp.AddRange(_inp_pfx);
                }

                var line_inp = Utils.llama_tokenize(_ctx, text, false, encoding);
                _embed_inp.AddRange(line_inp);

                // instruct mode: insert response suffix
                if (_params.instruct)
                {
                    _embed_inp.AddRange(_inp_sfx);
                }

                _n_remain -= line_inp.Count;
            }
            return text;
        }

        public void InitChatPrompt(string prompt, string encoding = "UTF-8")
        {
            WithPrompt(prompt);
        }

        public void InitChatAntiprompt(string[] antiprompt)
        {
            _params.antiprompt = antiprompt.ToList();
        }

        public IEnumerable<string> Chat(string text, string? prompt = null, string encoding = "UTF-8")
        {
            _params.interactive = true;
            _input_echo = false;
            if (!string.IsNullOrEmpty(prompt))
            {
                WithPrompt(prompt);
            }
            return Call(text, encoding);
        }

        public IEnumerable<string> Call(string text, string encoding = "UTF-8")
        {
            _is_antiprompt = false;
            if(_n_past > 0)
            {
                _is_interacting = false;
            }
            ProcessTextBeforeInfer(text, encoding);
            
            while ((_n_remain != 0 || _params.interactive) && !_is_interacting)
            {
                if (_embed.Count > 0)
                {
                    // infinite text generation via context swapping
                    // if we run out of context:
                    // - take the n_keep first tokens from the original prompt (via n_past)
                    // - take half of the last (n_ctx - n_keep) tokens and recompute the logits in batches
                    if (_n_past + _embed.Count > _n_ctx)
                    {
                        int n_left = _n_past - _params.n_keep;

                        _n_past = Math.Max(1, _params.n_keep);

                        // insert n_left/2 tokens at the start of embed from last_n_tokens
                        _embed.InsertRange(0, _last_n_tokens.Take(_last_n_tokens.Count - _embed.Count).Skip(_n_ctx - n_left / 2 - _embed.Count));

                        // stop saving session if we run out of context
                        _path_session = "";

                        // Console.WriteLine("\n---\n");
                        // Console.Write("resetting: '");
                        // for (int i = 0; i < embed.Count; i++) {
                        //     Console.Write(llama_token_to_str(ctx, embed[i]));
                        // }
                        // Console.WriteLine("'\n");
                        // Console.WriteLine("\n---\n");
                    }

                    // try to reuse a matching prefix from the loaded session instead of re-eval (via n_past)
                    // REVIEW
                    if (_n_session_consumed < _session_tokens.Count)
                    {
                        int i = 0;
                        for (; i < _embed.Count; i++)
                        {
                            if (_embed[i] != _session_tokens[_n_session_consumed])
                            {
                                _session_tokens = _session_tokens.Take(_n_session_consumed).ToList();
                                break;
                            }

                            _n_past++;
                            _n_session_consumed++;

                            if (_n_session_consumed >= _session_tokens.Count)
                            {
                                i++;
                                break;
                            }
                        }

                        if (i > 0)
                        {
                            _embed.RemoveRange(0, i);
                        }
                    }

                    // evaluate tokens in batches
                    // embed is typically prepared beforehand to fit within a batch, but not always
                    for (int i = 0; i < _embed.Count; i += _params.n_batch)
                    {
                        int n_eval = _embed.Count - i;

                        if (n_eval > _params.n_batch)
                        {
                            n_eval = _params.n_batch;
                        }

                        var array = _embed.Skip(i).ToArray();
                        if (NativeApi.llama_eval(_ctx, array, n_eval, _n_past, _params.n_threads) != 0)
                        {
                            Logger.Default.Error($"Failed to eval.");
                            throw new RuntimeError("Failed to eval.");
                        }

                        _n_past += n_eval;
                    }

                    if (_embed.Count > 0 && !string.IsNullOrEmpty(_path_session))
                    {
                        _session_tokens.AddRange(_embed);
                        _n_session_consumed = _session_tokens.Count;
                    }
                }

                _embed.Clear();

                if (_embed_inp.Count <= _n_consumed && !_is_interacting)
                {
                    var temp = _params.temp;
                    var top_k = _params.top_k <= 0 ? NativeApi.llama_n_vocab(_ctx) : _params.top_k;
                    var top_p = _params.top_p;
                    var tfs_z = _params.tfs_z;
                    var typical_p = _params.typical_p;
                    var repeat_last_n = _params.repeat_last_n < 0 ? _n_ctx : _params.repeat_last_n;
                    var repeat_penalty = _params.repeat_penalty;
                    var alpha_presence = _params.presence_penalty;
                    var alpha_frequency = _params.frequency_penalty;
                    var mirostat = _params.mirostat;
                    var mirostat_tau = _params.mirostat_tau;
                    var mirostat_eta = _params.mirostat_eta;
                    var penalize_nl = _params.penalize_nl;

                    // optionally save the session on first sample (for faster prompt loading next time)
                    if (!string.IsNullOrEmpty(_path_session) && _need_to_save_session)
                    {
                        _need_to_save_session = false;
                        NativeApi.llama_save_session_file(_ctx, _path_session, _session_tokens.ToArray(), (ulong)_session_tokens.Count);
                    }

                    llama_token id = 0;

                    {
                        var n_vocab = NativeApi.llama_n_vocab(_ctx);
                        var logits = Utils.llama_get_logits(_ctx, n_vocab);

                        // Apply params.logit_bias map
                        foreach (var (key, value) in _params.logit_bias)
                        {
                            logits[key] += value;
                        }

                        var candidates = new List<LLamaTokenData>();
                        candidates.Capacity = n_vocab;
                        for (llama_token token_id = 0; token_id < n_vocab; token_id++)
                        {
                            candidates.Add(new LLamaTokenData(token_id, logits[token_id], 0.0f));
                        }

                        LLamaTokenDataArray candidates_p = new LLamaTokenDataArray(candidates.ToArray(), (ulong)candidates.Count, false);

                        // Apply penalties
                        float nl_logit = logits[NativeApi.llama_token_nl()];
                        var last_n_repeat = Math.Min(Math.Min(_last_n_tokens.Count, repeat_last_n), _n_ctx);
                        SamplingApi.llama_sample_repetition_penalty(_ctx, candidates_p,
                            _last_n_tokens.Skip(_last_n_tokens.Count - last_n_repeat).ToArray(),
                            (ulong)last_n_repeat, repeat_penalty);
                        SamplingApi.llama_sample_frequency_and_presence_penalties(_ctx, candidates_p,
                            _last_n_tokens.Skip(_last_n_tokens.Count - last_n_repeat).ToArray(),
                            (ulong)last_n_repeat, alpha_frequency, alpha_presence);
                        if (!penalize_nl)
                        {
                            logits[NativeApi.llama_token_nl()] = nl_logit;
                        }

                        if (temp <= 0)
                        {
                            // Greedy sampling
                            id = SamplingApi.llama_sample_token_greedy(_ctx, candidates_p);
                        }
                        else
                        {
                            if (mirostat == 1)
                            {
                                float mirostat_mu = 2.0f * mirostat_tau;
                                const int mirostat_m = 100;
                                SamplingApi.llama_sample_temperature(_ctx, candidates_p, temp);
                                id = SamplingApi.llama_sample_token_mirostat(_ctx, candidates_p, mirostat_tau, mirostat_eta, mirostat_m, ref mirostat_mu);
                            }
                            else if (mirostat == 2)
                            {
                                float mirostat_mu = 2.0f * mirostat_tau;
                                SamplingApi.llama_sample_temperature(_ctx, candidates_p, temp);
                                id = SamplingApi.llama_sample_token_mirostat_v2(_ctx, candidates_p, mirostat_tau, mirostat_eta, ref mirostat_mu);
                            }
                            else
                            {
                                // Temperature sampling
                                SamplingApi.llama_sample_top_k(_ctx, candidates_p, top_k, 1);
                                SamplingApi.llama_sample_tail_free(_ctx, candidates_p, tfs_z, 1);
                                SamplingApi.llama_sample_typical(_ctx, candidates_p, typical_p, 1);
                                SamplingApi.llama_sample_top_p(_ctx, candidates_p, top_p, 1);
                                SamplingApi.llama_sample_temperature(_ctx, candidates_p, temp);
                                id = SamplingApi.llama_sample_token(_ctx, candidates_p);
                            }
                        }

                        _last_n_tokens.RemoveAt(0);
                        _last_n_tokens.Add(id);
                    }

                    // replace end of text token with newline token when in interactive mode
                    if (id == NativeApi.llama_token_eos() && _params.interactive && !_params.instruct)
                    {
                        id = _llama_token_newline[0];
                        if (_params.antiprompt.Count != 0)
                        {
                            // tokenize and inject first reverse prompt
                            var first_antiprompt = Utils.llama_tokenize(_ctx, _params.antiprompt[0], false, encoding);
                            _embed_inp.AddRange(first_antiprompt);
                        }
                    }

                    // add it to the context
                    _embed.Add(id);

                    // echo this to console
                    _input_echo = true;

                    // decrement remaining sampling budget
                    _n_remain--;
                }
                else
                {
                    // Assuming that the necessary variables have been defined and initialized,
                    // the C# equivalent code could be:

                    while (_embed_inp.Count > _n_consumed)
                    {
                        _embed.Add(_embed_inp[_n_consumed]);
                        _last_n_tokens.RemoveAt(0);
                        _last_n_tokens.Add(_embed_inp[_n_consumed]);
                        _n_consumed++;
                        if (_embed.Count >= _params.n_batch)
                        {
                            break;
                        }
                    }
                }

                if (_input_echo && !_is_interacting)
                {
                    foreach (var id in _embed)
                    {
                        var res = Utils.PtrToStringUTF8(NativeApi.llama_token_to_str(_ctx, id));
                        yield return res;
                    }
                }

                if (_params.interactive && _embed_inp.Count <= _n_consumed)
                {
                    if (_params.antiprompt.Count > 0)
                    {
                        string last_output = "";
                        foreach (var id in _last_n_tokens)
                        {
                            last_output += Utils.PtrToStringUTF8(NativeApi.llama_token_to_str(_ctx, id));
                        }

                        _is_antiprompt = false;
                        foreach (var antiprompt in _params.antiprompt)
                        {
                            if (last_output.EndsWith(antiprompt))
                            {
                                _is_interacting = true;
                                _is_antiprompt = true;
                                break;
                            }
                        }
                    }

                    if(_n_past > 0 && _is_interacting)
                    {
                        if (_params.instruct)
                        {
                            yield return "\n> ";
                        }
                        _input_echo = false;
                        break;
                    }

                    if (_embed.Count > 0 && _embed.Last() == NativeApi.llama_token_eos())
                    {
                        if (_params.instruct) {
                            _is_interacting = true;
                        } else
                        {
                            Logger.Default.Info(" [end of text]");
                        }
                    }

                    if (_params.interactive && _n_remain <= 0 && _params.n_predict != -1) {
                        _n_remain = _params.n_predict;
                        _is_interacting = true;
                    }
                }
            }

            if(!string.IsNullOrEmpty(_path_session) && _params.prompt_cache_all)
            {
                Logger.Default.Info($"saving final output to session file {_path_session}");
                var session_token_array = _session_tokens.ToArray();
                NativeApi.llama_save_session_file(_ctx, _path_session, session_token_array, (ulong)session_token_array.Length);
            }
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
