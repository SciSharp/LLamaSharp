using LLama.Exceptions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LLama.Types;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections;

namespace LLama
{
    using llama_token = Int32;
    /// <summary>
    /// High-level Wrapper of a llama.cpp model for inference. Note that it's more recommended to use `LLamaModel`.
    /// This class may be removed in the future. However, if all you want is to get the embeddings, then using `LLamaModelV1`
    /// is ok now.
    /// </summary>
    [Obsolete]
    public class LLamaModelV1
    {
        private string _model_path;
        LLamaContextParams _params;
        private int _n_threads;
        private int _n_batch;
        private int _last_n_tokens_size;
        private string? _lora_base;
        private string? _lora_path;
        private bool _verbose;

        private Queue<llama_token> _eval_tokens;
        private Queue<float[]> _eval_logits;
        private LLamaCache? _cache;
        private SafeLLamaContextHandle _ctx;

        private static readonly (int, int)[] _numAndPatterns = new (int, int)[] { (2, 192), (3, 224), (4, 240) };

    /// <summary>
    /// Load a llama.cpp model from the path.
    /// </summary>
    /// <remarks>Note that the API is still unstable. The order of them is likely to
    /// be changed in the future. It's recommened to specify the parameter name when
    ///  building your app. We use the cpp style parameter names here because it introduces
    ///  convenience for searching the docs.</remarks>
    /// <param name="model_path">Path to the model.</param>
    /// <param name="n_ctx">Maximum context size.</param>
    /// <param name="n_parts">Number of parts to split the model into. If -1, the number of parts is automatically determined.</param>
    /// <param name="seed">Random seed. 0 for random.</param>
    /// <param name="f16_kv">Use half-precision for key/value cache.</param>
    /// <param name="logits_all">Return logits for all tokens, not just the last token.</param>
    /// <param name="vocab_only">Only load the vocabulary no weights.</param>
    /// <param name="use_mmap">Use mmap if possible.</param>
    /// <param name="use_mlock">Force the system to keep the model in RAM.</param>
    /// <param name="embedding">Embedding mode only.</param>
    /// <param name="n_threads">Number of threads to use. If is not specified, the number of threads is automatically determined.</param>
    /// <param name="n_batch">Maximum number of prompt tokens to batch together when calling llama_eval.</param>
    /// <param name="last_n_tokens_size">Maximum number of tokens to keep in the last_n_tokens deque.</param>
    /// <param name="lora_base">Optional path to base model, useful if using a quantized base model and you want to apply LoRA to an f16 model.</param>
    /// <param name="lora_path">Path to a LoRA file to apply to the model.</param>
    /// <param name="verbose">Print verbose output to stderr.</param>
    public LLamaModelV1(string model_path, int n_ctx = 512, int n_parts = -1, int seed = 1337, 
            bool f16_kv = true, bool logits_all = false, bool vocab_only = false, bool use_mmap = true, 
            bool use_mlock = false, bool embedding = false, int n_threads = -1, int n_batch = 512, 
            int last_n_tokens_size = 64, string? lora_base = null, string? lora_path = null, bool verbose = true)
        {
            _verbose = verbose;
            _model_path = model_path;

            _params = NativeApi.llama_context_default_params();
            _params.n_ctx = n_ctx;
            _params.n_parts = n_parts;
            _params.seed = seed;
            _params.f16_kv = f16_kv;
            _params.logits_all = logits_all;
            _params.vocab_only = vocab_only;
            _params.use_mmap = lora_path is null ? use_mmap : false;
            _params.use_mlock = use_mlock;
            _params.embedding = embedding;

            _last_n_tokens_size = last_n_tokens_size;
            _n_batch = Math.Min(n_ctx, n_batch);

            _eval_tokens = new Queue<int>(capacity:  n_ctx);
            _eval_logits = new Queue<float[]>(logits_all ? n_ctx : 1);

            _cache = null;

            _n_threads = n_threads;
            if(_n_threads == -1)
            {
                _n_threads = Math.Max(Environment.ProcessorCount / 2, 1);
            }

            _lora_base = lora_base;
            _lora_path = lora_path;

            if(!File.Exists(model_path) && !Directory.Exists(model_path))
            {
                throw new FileNotFoundException($"Model path does not exist: {model_path}");
            }

            // Move from heap to stack to prevent the moving.
            _ctx = new SafeLLamaContextHandle(NativeApi.llama_init_from_file(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(model_path)), _params));

            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);

            if(_lora_path is not null)
            {
                if(NativeApi.llama_apply_lora_from_file(_ctx, lora_path, lora_base, _n_threads) != 0)
                {
                    throw new RuntimeError($"Failed to apply LoRA from lora path: {_lora_path} to base path: {_lora_base}");
                }
            }

            if (_verbose)
            {
                Logger.Default.Info(Utils.PtrToStringUTF8(NativeApi.llama_print_system_info()));
            }
        }

        public LLamaModelV1(LLamaModelV1 other)
        {
            _ctx = other._ctx;
            _model_path = other._model_path;
            _params = other._params;
            _last_n_tokens_size = other._last_n_tokens_size;
            _n_threads = other._n_threads;
            _n_batch = other._n_batch;
            _verbose = other._verbose;
            _lora_base = other._lora_base;
            _lora_path = other._lora_path;
            _eval_logits = new Queue<float[]>(other._eval_logits);
            _eval_tokens = new Queue<llama_token>(other._eval_tokens);
        }

        /// <summary>
        /// Tokenize a string.
        /// </summary>
        /// <param name="text">The utf-8 encoded string to tokenize.</param>
        /// <returns>A list of tokens.</returns>
        /// <exception cref="RuntimeError">If the tokenization failed.</exception>
        public List<llama_token> Tokenize(string text)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            var n_ctx = NativeApi.llama_n_ctx(_ctx);
            var tokens = new llama_token[n_ctx];
            var n_tokens = NativeApi.llama_tokenize(_ctx, text, tokens, n_ctx, true);
            if(n_tokens < 0)
            {
                throw new RuntimeError($"Failed to tokenize: text=\"{text}\" n_tokens={n_tokens}");
            }
            return tokens.Take(n_tokens).ToList();
        }

        /// <summary>
        /// Detokenize a list of tokens.
        /// </summary>
        /// <param name="tokens">The list of tokens to detokenize.</param>
        /// <returns>The detokenized string.</returns>
        public string DeTokenize(IEnumerable<llama_token> tokens)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            string output = "";
            foreach(var token in tokens)
            {
                output += Utils.PtrToStringUTF8(NativeApi.llama_token_to_str(_ctx, token));
            }
            return output;
        }

        public string DeTokenize(llama_token token)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            return Utils.PtrToStringUTF8(NativeApi.llama_token_to_str(_ctx, token)) ?? "";
        }

        /// <summary>
        /// Set the cache.
        /// </summary>
        /// <param name="cache">The cache to set.</param>
        public void SetCache(LLamaCache? cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Reset the model state.
        /// </summary>
        public void Reset()
        {
            _eval_tokens.Clear();
            _eval_logits.Clear();
        }

        /// <summary>
        /// Evaluate a list of tokens.
        /// </summary>
        /// <param name="tokens">The list of tokens to evaluate.</param>
        /// <exception cref="RuntimeError"></exception>
        public unsafe void Eval(List<llama_token> tokens)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            var n_ctx = NativeApi.llama_n_ctx(_ctx);
            for(int i = 0; i < tokens.Count; i += _n_batch)
            {
                var batch = tokens.Take(Math.Min(tokens.Count, i + _n_batch)).Skip(i);
                llama_token n_past = Math.Min(n_ctx - batch.Count(), _eval_tokens.Count);
                llama_token n_tokens = batch.Count();
                llama_token return_code = NativeApi.llama_eval(
                    ctx: _ctx, 
                    tokens: batch.ToArray(), 
                    n_tokens: n_tokens,
                    n_past: n_past,
                    n_threads: _n_threads
                );
                if(return_code != 0)
                {
                    throw new RuntimeError($"llama_eval returned {return_code}");
                }
                foreach(var b in batch)
                {
                    _eval_tokens.Enqueue(b);
                }
                int rows = _params.logits_all ? n_tokens : 1;
                llama_token n_vocab = NativeApi.llama_n_vocab(_ctx);
                var cols = n_vocab;
                var logits_view = NativeApi.llama_get_logits(_ctx);
                for(int j = 0; j < rows; j++)
                {
                    float[] logit = new float[cols];
                    for(int k = 0; k < cols; k++)
                    {
                        logit[k] = logits_view[j * cols + k];
                    }
                    _eval_logits.Enqueue(logit);
                }
            }
        }

        private llama_token SampleInternal(llama_token[] last_n_tokens_data, int last_n_tokens_size, int top_k, 
            float top_p, float temp, float repeat_penalty, float frequency_penalty, float presence_penalty)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            Debug.Assert(_eval_logits.Count > 0);
            llama_token n_vocab = NativeApi.llama_n_vocab(_ctx);
            var logits = _eval_logits.Last();
            LLamaTokenData[] data = new LLamaTokenData[n_vocab];
            for(int i = 0; i < n_vocab; i++)
            {
                data[i] = new LLamaTokenData(i, logits[i], .0f);
            }
            ulong size = (ulong)n_vocab;
            bool sorted = false;
            LLamaTokenDataArray candidates = new(data, size, sorted);
            SamplingApi.llama_sample_repetition_penalty(_ctx, candidates, last_n_tokens_data, (ulong)last_n_tokens_size,
                repeat_penalty);
            //SamplingApi.llama_sample_frequency_and_presence_penalties(_ctx, candidates, last_n_tokens_data, (ulong)last_n_tokens_size,
            //    frequency_penalty, presence_penalty);
            if(temp == .0f)
            {
                return SamplingApi.llama_sample_token_greedy(_ctx, candidates);
            }
            else
            {
                SamplingApi.llama_sample_top_k(_ctx, candidates, top_k, 1);
                SamplingApi.llama_sample_tail_free(_ctx, candidates, 1.0f, 1);
                SamplingApi.llama_sample_typical(_ctx, candidates, 1.0f, 1);
                SamplingApi.llama_sample_top_p(_ctx, candidates, top_p, 1);
                SamplingApi.llama_sample_temperature(_ctx, candidates, temp);
                return SamplingApi.llama_sample_token(_ctx, candidates);
            }
        }

        /// <summary>
        /// Sample a token from the model.
        /// </summary>
        /// <param name="top_k">The top-k sampling parameter.</param>
        /// <param name="top_p">The top-p sampling parameter.</param>
        /// <param name="temp">The temperature parameter.</param>
        /// <param name="repeat_penalty">The repeat penalty parameter.</param>
        /// <param name="frequency_penalty"></param>
        /// <param name="presence_penalty"></param>
        /// <returns>The sampled token.</returns>
        public llama_token Sample(int top_k, float top_p, float temp, float repeat_penalty, float frequency_penalty = .0f, 
            float presence_penalty = .0f)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            var last_n_tokens_data = Enumerable.Repeat(0, Math.Max(0, _last_n_tokens_size - _eval_tokens.Count));
            last_n_tokens_data = last_n_tokens_data.Concat(_eval_tokens.ToList()
                .Skip(Math.Max(0, _eval_tokens.Count - _last_n_tokens_size)));
            llama_token[] tokens_data = new llama_token[_last_n_tokens_size];
            int i = 0;
            foreach(var data in last_n_tokens_data)
            {
                if(i < _last_n_tokens_size)
                {
                    tokens_data[i++] = data;
                }
                else
                {
                    break;
                }
            }
            return SampleInternal(tokens_data, _last_n_tokens_size, top_k, top_p, temp, repeat_penalty, frequency_penalty, presence_penalty);
        }

        /// <summary>
        /// Create a generator of tokens from a prompt.
        /// </summary>
        /// <example>
        /// Examples:
        /// var llama = new LlamaModel("models/ggml-7b.bin")
        /// var tokens = llama.Tokenize(b"Hello, world!")
        /// foreach(var token in llama.Generate(tokens, top_k:40, top_p:0.95, temp:1.0, repeat_penalty:1.1)){
        ///     Console.WriteLine(llama.DeTokenize(new []{token}));
        /// }
        /// </example>
        /// <param name="tokens"></param>
        /// <param name="top_k"></param>
        /// <param name="top_p"></param>
        /// <param name="temp"></param>
        /// <param name="repeat_penalty"></param>
        /// <param name="frequency_penalty"></param>
        /// <param name="presence_penalty"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public IEnumerable<llama_token> Generate(IEnumerable<llama_token> tokens, int top_k, float top_p, float temp, 
            float repeat_penalty, float frequency_penalty = .0f, float presence_penalty = .0f, bool reset = true)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            if(reset && _eval_tokens.Count > 0)
            {
                int longest_prefix = 0;
                foreach(var (a, b) in _eval_tokens.ToList().Zip(tokens.Take(tokens.Count() - 1), (x, y) => (x, y)))
                {
                    if(a == b)
                    {
                        longest_prefix += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if(longest_prefix > 0)
                {
                    if (_verbose)
                    {
                        Logger.Default.Info("Llama.generate: prefix-match hit");
                    }
                    reset = false;
                    tokens = tokens.Skip(longest_prefix);
                    for(int i = 0; i < _eval_tokens.Count - longest_prefix; i++)
                    {
                        _eval_tokens.Dequeue();
                        if(_eval_logits.Count > 0)
                        {
                            _eval_logits.Dequeue();
                        }
                    }
                }
            }

            if (reset)
            {
                Reset();
            }

            while (true)
            {
                Eval(tokens.ToList());
                var token = Sample(top_k, top_p, temp, frequency_penalty, presence_penalty, repeat_penalty);
                yield return token;
                // TODO(Rinne): verify if the implementation is correct.
            }
        }

        /// <summary>
        /// Embed a string.
        /// </summary>
        /// <param name="input">The utf-8 encoded string to embed.</param>
        /// <returns>An embedding object.</returns>
        /// <exception cref="RuntimeError"></exception>
        public unsafe Embedding CreateEmbedding(string input)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            if (!_params.embedding)
            {
                throw new RuntimeError("Llama model must be created with embedding=True to call this method");
            }

            if (_verbose)
            {
                NativeApi.llama_reset_timings(_ctx);
            }

            var tokens = Tokenize(input);
            Reset();
            Eval(tokens);
            int n_tokens = tokens.Count;
            var embeddingPtr = NativeApi.llama_get_embeddings(_ctx);
            int cnt = NativeApi.llama_n_embd(_ctx);
            float[] embedding = new float[cnt];
            for(int i = 0; i < cnt; i++)
            {
                embedding[i] = embeddingPtr[i];
            }

            if (_verbose)
            {
                NativeApi.llama_print_timings(_ctx);
            }

            return new Embedding("list", _model_path, new[] { new EmbeddingData(0, "embedding", embedding) },
                new EmbeddingUsage(n_tokens, n_tokens));
        }

        public float[] Embed(string input)
        {
            return CreateEmbedding(input).Data[0].Embedding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="suffix"></param>
        /// <param name="max_tokens"></param>
        /// <param name="temperature"></param>
        /// <param name="top_p"></param>
        /// <param name="logprobs"></param>
        /// <param name="echo"></param>
        /// <param name="stop"></param>
        /// <param name="frequency_penalty"></param>
        /// <param name="presence_penalty"></param>
        /// <param name="repeat_penalty"></param>
        /// <param name="top_k"></param>
        /// <returns>IEnumerable of Completion and CompletionChunk</returns>
        /// <exception cref="ArgumentException"></exception>
        private IEnumerable<CompletionChunk> CreateCompletionInternal(string prompt, string?suffix = null, int max_tokens = 16, float temperature = 0.8f, 
            float top_p = 0.95f, int logprobs = -1, bool echo = false, string[]? stop = null, float frequency_penalty = .0f, 
            float presence_penalty = .0f, float repeat_penalty = 1.1f, int top_k = 40)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            string completionId = $"cmpl-{Guid.NewGuid()}";
            var created = DateTime.Now.Millisecond;
            List<llama_token> completionTokens = new List<llama_token>();

            var promptTokens = Tokenize($" {prompt}");
            string text = "";
            int returnedCharacters = 0;
            if(stop is null)
            {
                stop = new string[0];
            }

            if (_verbose)
            {
                NativeApi.llama_reset_timings(_ctx);
            }

            if(promptTokens.Count + max_tokens > NativeApi.llama_n_ctx(_ctx))
            {
                throw new ArgumentException($"Requested tokens exceed context window of {NativeApi.llama_n_ctx(_ctx)}");
            }
            
            if(logprobs != -1 && !_params.logits_all)
            {
                throw new ArgumentException("logprobs is not supported for models created with logits_all=False");
            }

            if(_cache is not null)
            {
                try
                {
                    // TODO(Rinne): revise it since it will compare reference instead of elements.
                    var cacheItem = _cache[promptTokens.ToArray()];
                    var cachePrefixLen = LongestTokenPrefix(_eval_tokens.AsEnumerable(), promptTokens);
                    var evalPrefixLen = LongestTokenPrefix(_eval_tokens.AsEnumerable(), promptTokens);
                    if(cachePrefixLen > evalPrefixLen)
                    {
                        LoadState(cacheItem);
                        if (_verbose)
                        {
                            Logger.Default.Info("Llama._create_completion: cache hit");
                        }
                    }
                }
                catch (KeyNotFoundException)
                {
                    if (_verbose)
                    {
                        Logger.Default.Warn("Llama._create_completion: cache miss");
                    }
                }
            }

            string finishReason = "length";
            int multibyteFix = 0;
            bool reset = true;
            List<llama_token> tokens = new(promptTokens);
            if (reset && _eval_tokens.Count > 0)
            {
                int longest_prefix = 0;
                foreach (var (a, b) in _eval_tokens.ToList().Zip(tokens.Take(tokens.Count - 1), (x, y) => (x, y)))
                {
                    if (a == b)
                    {
                        longest_prefix += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (longest_prefix > 0)
                {
                    if (_verbose)
                    {
                        Logger.Default.Info("Llama.generate: prefix-match hit");
                    }
                    reset = false;
                    tokens = tokens.Skip(longest_prefix).ToList();
                    for (int i = 0; i < _eval_tokens.Count - longest_prefix; i++)
                    {
                        _eval_tokens.Dequeue();
                        if (_eval_logits.Count > 0)
                        {
                            _eval_logits.Dequeue();
                        }
                    }
                }
            }

            if (reset)
            {
                Reset();
            }
            //foreach (var token in Generate(promptTokens, top_k, top_p, temperature, frequency_penalty, presence_penalty, repeat_penalty))
            string allText = "";
            while (true)
            {
                Eval(tokens);
                var token = Sample(top_k, top_p, temperature, repeat_penalty, frequency_penalty, presence_penalty);
                tokens.Clear();
                tokens.Add(token);
                if (token == NativeApi.llama_token_eos())
                {
                    text = DeTokenize(completionTokens);
                    finishReason = "stop";
                    break;
                }

                completionTokens.Add(token);

                allText = DeTokenize(completionTokens);

                int cut = Math.Min(3, allText.Length);
                for(int i = allText.Length - cut; i < allText.Length; i++)
                {
                    var c = (int)allText[i];
                    int k = cut - i;
                    foreach(var (num, pattern) in _numAndPatterns)
                    {
                        if(num > k && (pattern & c) == pattern)
                        {
                            multibyteFix = num - k;
                        }
                    }
                }

                if(multibyteFix > 0)
                {
                    multibyteFix--;
                    continue;
                }

                var anyStop = stop.Where(s => allText.Contains(s));
                if(anyStop.Count() > 0)
                {
                    var firstStop = anyStop.First();
                    text = allText.Substring(0, allText.IndexOf(firstStop));
                    finishReason = "stop";
                    break;
                }

                var start = returnedCharacters;
                int longest = 0;
                foreach (var s in stop)
                {
                    for (int i = s.Length; i > 0; i--)
                    {
                        if (allText.EndsWith(s.Substring(0, i)))
                        {
                            if (i > longest)
                            {
                                longest = i;
                            }
                            break;
                        }
                    }
                }
                text = allText.Substring(0, allText.Length - longest);
                returnedCharacters += text.Skip(start).Count();
                yield return new CompletionChunk(completionId, "text_completion", created, _model_path, new CompletionChoice[]
                {
                     new CompletionChoice(text.Substring(start), 0, null, finishReason)
                });
            }

            if (_cache is not null)
            {
                if (_verbose)
                {
                    Logger.Default.Info("Llama._create_completion: cache save");
                }
                _cache[promptTokens.Concat(completionTokens).ToArray()] = SaveState();
            }

            string textStr = text;
            if (echo)
            {
                textStr = prompt + textStr;
            }
            if(suffix is not null)
            {
                textStr = textStr + suffix;
            }

            CompletionLogprobs? logProbs = null;
            if (logprobs != -1)
            {
                int textOffset = 0;
                List<int> textOffsets = new();
                List<float> tokenLogprobs = new();
                List<string> tokenStrs = new();
                List<Dictionary<string, float>> topLogprobs = new();

                var allTokens = promptTokens.Concat(completionTokens).ToArray();
                var allTokenStrs = allTokens.Select(t => DeTokenize(new[] { t }));
                var allLogProbs = _eval_logits.Select(row => LogitsToLogprobs(row));

                foreach (var (token, tokenStr, logProbsToken) in allTokens.Zip(allTokenStrs, (x, y) => (x, y))
                    .Zip(allLogProbs, (x, y) => (x.x, x.y, y)))
                {
                    textOffsets.Add(textOffset);
                    textOffset += tokenStr.Length;
                    tokenStrs.Add(tokenStr);
                    var sortedLogprobs = logProbsToken.Zip(Enumerable.Range(0, logProbsToken.Count()), (x, y) => (x, y))
                        .OrderByDescending(x => x.x).ToList();
                    tokenLogprobs.Add(sortedLogprobs[token].x);
                    var topLogprob = sortedLogprobs.Take(logprobs).ToDictionary(t => DeTokenize(new[] { t.y }), t => t.x);
                    topLogprob[tokenStr] = sortedLogprobs[token].x;
                    topLogprobs.Add(topLogprob);
                }

                logProbs = new(textOffsets.ToArray(), tokenLogprobs.ToArray(), tokenStrs.ToArray(), topLogprobs.ToArray());
            }

            if (_verbose)
            {
                NativeApi.llama_print_timings(_ctx);
            }
        }

        /// <summary>
        /// Generate text from a prompt and yield return the result.
        /// </summary>
        /// <param name="prompt">The prompt to generate text from.</param>
        /// <param name="suffix">A suffix to append to the generated text. If None, no suffix is appended.</param>
        /// <param name="max_tokens">The maximum number of tokens to generate.</param>
        /// <param name="temperature">The temperature to use for sampling.</param>
        /// <param name="top_p">The top-p value to use for sampling.</param>
        /// <param name="logprobs">The number of logprobs to return. If None, no logprobs are returned.</param>
        /// <param name="echo">Whether to echo the prompt.</param>
        /// <param name="stop">A list of strings to stop generation when encountered.</param>
        /// <param name="frequency_penalty"></param>
        /// <param name="presence_penalty"></param>
        /// <param name="repeat_penalty">The penalty to apply to repeated tokens.</param>
        /// <param name="top_k">The top-k value to use for sampling.</param>
        /// <returns></returns>
        public IEnumerable<CompletionChunk> CreateCompletion(string prompt, string? suffix = null, int max_tokens = 128, float temperature = 0.8f,
            float top_p = 0.95f, int logprobs = -1, bool echo = false, string[]? stop = null, float frequency_penalty = .0f,
            float presence_penalty = .0f, float repeat_penalty = 1.1f, int top_k = 40)
        {
            return CreateCompletionInternal(prompt, suffix, max_tokens, temperature, top_p, logprobs, echo, stop, 
                frequency_penalty, presence_penalty, repeat_penalty, top_k);
        }

        /// <summary>
        /// Generate text from a prompt and yield return the result.
        /// </summary>
        /// <param name="prompt">The prompt to generate text from.</param>
        /// <param name="suffix">A suffix to append to the generated text. If None, no suffix is appended.</param>
        /// <param name="max_tokens">The maximum number of tokens to generate.</param>
        /// <param name="temperature">The temperature to use for sampling.</param>
        /// <param name="top_p">The top-p value to use for sampling.</param>
        /// <param name="logprobs">The number of logprobs to return. If None, no logprobs are returned.</param>
        /// <param name="echo">Whether to echo the prompt.</param>
        /// <param name="stop">A list of strings to stop generation when encountered.</param>
        /// <param name="frequency_penalty"></param>
        /// <param name="presence_penalty"></param>
        /// <param name="repeat_penalty">The penalty to apply to repeated tokens.</param>
        /// <param name="top_k">The top-k value to use for sampling.</param>
        /// <returns></returns>
        public IEnumerable<CompletionChunk> Call(string prompt, string? suffix = null, int max_tokens = 128, float temperature = 0.8f,
            float top_p = 0.95f, int logprobs = -1, bool echo = false, string[]? stop = null, float frequency_penalty = .0f,
            float presence_penalty = .0f, float repeat_penalty = 1.1f, int top_k = 40)
        {
            return CreateCompletion(prompt, suffix, max_tokens, temperature, top_p, logprobs, echo, stop,
                frequency_penalty, presence_penalty, repeat_penalty, top_k);
        }

        private ChatCompletion ConvertTextCompletionToChat(Completion completion)
        {
            return new ChatCompletion($"chat{completion.Id}", "chat.completion", completion.Created, completion.Model,
                new[] { new ChatCompletionChoice(0, new ChatCompletionMessage(ChatRole.Assistant, completion.Choices[0].Text),
                completion.Choices[0].FinishReason) }, completion.Usage);
        }

        private IEnumerable<ChatCompletionChunk> ConvertTextCompletionChunksToChat(IEnumerable<CompletionChunk> chunks)
        {
            bool isFirst = true;
            foreach(var chunk in chunks)
            {
                if(isFirst)
                {
                    yield return new ChatCompletionChunk($"chat{chunk.Id}", chunk.Model, "chat.completion.chunk", chunk.Created, 
                        new[] { new ChatCompletionChunkChoice(0, new ChatCompletionChunkDelta("assistant", null), null) });
                    isFirst = false;
                }
                yield return new ChatCompletionChunk($"chat{chunk.Id}", chunk.Model, "chat.completion.chunk", chunk.Created,
                        new[] { new ChatCompletionChunkChoice(0, new ChatCompletionChunkDelta(null, chunk.Choices[0].Text),
                        chunk.Choices[0].FinishReason) });
            }
        }

        /// <summary>
        /// Generate a chat completion from a list of messages and yield return the result.
        /// </summary>
        /// <param name="messages">A list of messages to generate a response for.</param>
        /// <param name="temperature">The temperature to use for sampling.</param>
        /// <param name="top_p">The top-p value to use for sampling.</param>
        /// <param name="top_k">The top-k value to use for sampling.</param>
        /// <param name="stop">A list of strings to stop generation when encountered.</param>
        /// <param name="max_tokens">The maximum number of tokens to generate.</param>
        /// <param name="presence_penalty"></param>
        /// <param name="frequency_penalty"></param>
        /// <param name="repeat_penalty">The penalty to apply to repeated tokens.</param>
        /// <returns></returns>
        public IEnumerable<ChatCompletionChunk> CreateChatCompletion(IEnumerable<ChatCompletionMessage> messages, float temperature = .2f, float top_p = .95f,
            int top_k = 40, string[]? stop = null, int max_tokens = 256, float presence_penalty = .0f, float frequency_penalty = .0f,
            float repeat_penalty = 1.1f)
        {
            if (stop is null)
            {
                stop = new string[0];
            }
            string GetRole(ChatCompletionMessage message)
            {
                return message.Role == ChatRole.Human ? "Human" : "Assistant";
            }
            string chatHistory = string.Join("", messages.Select(m => $"### {GetRole(m)}:{m.Content}"));
            var prompt = chatHistory + "### Assistant:";
            prompt = prompt.Substring(Math.Max(0, prompt.Length - max_tokens));
            var promptStop = new[] { "### Assistant:", "### Human:" }.Concat(stop).ToArray();
            var completion = Call(prompt, stop: promptStop, temperature: temperature, top_p: top_p, top_k: top_k, max_tokens: max_tokens,
                repeat_penalty: repeat_penalty, presence_penalty: presence_penalty, frequency_penalty: frequency_penalty);
            return ConvertTextCompletionChunksToChat(completion);
        }

        public LLamaState SaveState()
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            ulong stateSize = NativeApi.llama_get_state_size(_ctx);
            byte[] llamaState = new byte[stateSize];
            ulong nBytes = NativeApi.llama_copy_state_data(_ctx, llamaState);
            if(nBytes > stateSize)
            {
                throw new RuntimeError("Failed to copy llama state data");
            }
            byte[] llamaStateCompact = new byte[nBytes];
            llamaState.Take((int)nBytes).ToArray().CopyTo(llamaStateCompact, 0);
            if (_verbose)
            {
                Logger.Default.Info($"Llama.save_state: saving {nBytes} bytes of llama state");
            }
            return new LLamaState(new Queue<llama_token>(_eval_tokens), new Queue<float[]>(_eval_logits),
                llamaStateCompact, (int)nBytes);
        }

        public void LoadState(LLamaState state)
        {
            Debug.Assert(_ctx.DangerousGetHandle() != IntPtr.Zero);
            _eval_tokens = new Queue<llama_token>(state.EvalTokens);
            _eval_logits = new Queue<float[]>(state.EvalLogits);
            if(NativeApi.llama_set_state_data(_ctx, state.State) != (ulong)state.Size)
            {
                throw new RuntimeError($"Failed to set llama state data");
            }
        }

        private static IEnumerable<float> LogitsToLogprobs(IEnumerable<float> logits)
        {
            var exps = logits.Select(x => (float)Math.Exp(x));
            var sumExps = exps.Sum();
            return exps.Select(x => (float)Math.Log(x / sumExps));
        }

        internal static int LongestTokenPrefix(IEnumerable<llama_token> a, IEnumerable<llama_token> b)
        {
            int longestPrefix = 0;
            foreach(var (x, y) in a.Zip(b, (x, y) => (x, y)))
            {
                if(x == y)
                {
                    longestPrefix++;
                }
                else
                {
                    break;
                }
            }
            return longestPrefix;
        }
    }
}
