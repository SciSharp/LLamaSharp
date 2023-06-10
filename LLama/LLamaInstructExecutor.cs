using LLama.Abstractions.Params;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama
{
    using llama_token = Int32;
    public class LLamaInstructExecutor : LLamaExecutorBase
    {
        bool _prompt_run = true;
        readonly IEnumerable<llama_token> _llama_token_newline;
        readonly IEnumerable<llama_token> _inp_pfx;
        readonly IEnumerable<llama_token> _inp_sfx;
        public LLamaInstructExecutor(LLamaModel model, string inputPrefix = "\n\n### Instruction:\n\n",
            string inputSuffix = "\n\n### Response:\n\n") : base(model)
        {
            _llama_token_newline = Utils.Tokenize(_model.NativeHandle, "\n", false, _model.Encoding);
            _inp_pfx = _model.Tokenize(inputPrefix, true);
            _inp_sfx = _model.Tokenize(inputSuffix, false);
        }

        /// <summary>
        /// process the text and return the tokens consumed.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sessionParams"></param>
        /// <param name="encoding"></param>
        /// <param name="is_antiprompt"></param>
        /// <returns></returns>
        protected virtual int ProcessTextBeforeInfer(string text, SessionParams sessionParams)
        {
            if (text.Length > 1)
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                _consumedTokensCount = _embed_inps.Count;
                _embed_inps.AddRange(_inp_pfx);

                var line_inp = _model.Tokenize(text, false);
                _embed_inps.AddRange(line_inp);

                _embed_inps.AddRange(_inp_sfx);

                return line_inp.Count();
            }
            else
            {
                return 0;
            }
        }

        public override IEnumerable<string> Infer(string text, SessionParams? sessionParams = null, IEnumerable<string>? antiprompts = null)
        {
            if (sessionParams is null)
            {
                sessionParams = new SessionParams();
            }
            // if n_remain < 0, the response will be generated endlessly.
            int n_remain = sessionParams.ResponseTokensCount;
            bool return_value = false;
            bool wait_for_input = false;
            bool need_to_save_session = !string.IsNullOrEmpty(_pathSession) && _n_matching_session_tokens < _embed_inps.Count;

            if (_prompt_run)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
                text = " " + text;
                _embed_inps = _model.Tokenize(text, true).ToList();
            }
            else
            {
                n_remain -= ProcessTextBeforeInfer(text, sessionParams);
            }

            while (n_remain != 0 || _prompt_run)
            {
                if (_embeds.Count > 0)
                {
                    _prompt_run = false;
                    if (_pastTokensCount + _embeds.Count > _model.ContextSize)
                    {
                        HandleRunOutOfContext(sessionParams.TokensToKeep);
                    }

                    TryReuseMathingPrefix();
                    _pastTokensCount = _model.Eval(_embeds.ToArray(), _pastTokensCount);

                    if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                    {
                        _session_tokens.AddRange(_embeds);
                        _n_session_consumed = _session_tokens.Count;
                    }
                }

                _embeds.Clear();

                if (_embed_inps.Count <= _consumedTokensCount && !wait_for_input)
                {
                    var temp = sessionParams.Temperature;
                    var top_k = sessionParams.TopK <= 0 ? NativeApi.llama_n_vocab(_model.NativeHandle) : sessionParams.TopK;
                    var top_p = sessionParams.TopK;
                    var tfs_z = sessionParams.TfsZ;
                    var typical_p = sessionParams.TypicalP;
                    var repeat_last_n = sessionParams.RepeatLastTokensCount < 0 ? _model.ContextSize : sessionParams.RepeatLastTokensCount;
                    var repeat_penalty = sessionParams.RepeatPenalty;
                    var alpha_presence = sessionParams.PresencePenalty;
                    var alpha_frequency = sessionParams.FrequencyPenalty;
                    var mirostat = sessionParams.Mirostat;
                    var mirostat_tau = sessionParams.MirostatTau;
                    var mirostat_eta = sessionParams.MirostatEta;
                    var penalize_nl = sessionParams.PenalizeNL;

                    // optionally save the session on first sample (for faster prompt loading next time)
                    if (!string.IsNullOrEmpty(_pathSession) && need_to_save_session)
                    {
                        need_to_save_session = false;
                        SaveSessionFile(_pathSession);
                    }

                    var tokenDataArray = _model.ApplyPenalty(_last_n_tokens, sessionParams.LogitBias, repeat_last_n,
                        repeat_penalty, alpha_frequency, alpha_presence, penalize_nl);

                    var id = _model.Sample(tokenDataArray, temp, mirostat, mirostat_tau, mirostat_eta, top_k, top_p,
                        tfs_z, typical_p);

                    _last_n_tokens.Enqueue(id);

                    _embeds.Add(id);

                    n_remain--;
                    return_value = true;
                }
                else
                {
                    while (_embed_inps.Count > _consumedTokensCount)
                    {
                        _embeds.Add(_embed_inps[_consumedTokensCount]);
                        _last_n_tokens.Enqueue(_embed_inps[_consumedTokensCount]);
                        _consumedTokensCount++;
                        if (_embeds.Count >= _model.Params.BatchSize)
                        {
                            break;
                        }
                    }
                }

                if (return_value)
                {
                    foreach (var item in _model.GenerateResult(_embeds))
                    {
                        yield return item;
                    }
                }

                if (_embed_inps.Count <= _consumedTokensCount)
                {
                    if (antiprompts is not null && antiprompts.Count() > 0)
                    {
                        string last_output = "";
                        foreach (var id in _last_n_tokens)
                        {
                            last_output += Utils.PtrToString(NativeApi.llama_token_to_str(_model.NativeHandle, id), _model.Encoding);
                        }

                        foreach (var antiprompt in antiprompts)
                        {
                            if (last_output.EndsWith(antiprompt))
                            {
                                wait_for_input = true;
                                break;
                            }
                        }
                    }

                    if (_pastTokensCount > 0 && wait_for_input)
                    {
                        yield return "\n> ";
                        break;
                    }
                }

                if (_embeds.Count > 0 && _embeds.Last() == NativeApi.llama_token_eos())
                {
                    wait_for_input = true;
                }

                if (n_remain <= 0 && sessionParams.ResponseTokensCount != -1)
                {
                    n_remain = sessionParams.ResponseTokensCount;
                    wait_for_input = true;
                }
            }
        }
    }
}
