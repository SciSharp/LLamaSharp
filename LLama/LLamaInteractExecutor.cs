using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LLama
{
    using llama_token = Int32;
    public class InteractiveExecutor : StatefulExecutorBase
    {
        bool _is_prompt_run = true;
        llama_token[] _llama_token_newline;
        public InteractiveExecutor(LLamaModel model) : base(model)
        {
            _llama_token_newline = Utils.Tokenize(_model.NativeHandle, "\n", false, _model.Encoding).ToArray();
        }

        public override void SaveState(string filename)
        {
            InteractiveExecutorState state = new()
            {
                ConsumedSessionCount = _n_session_consumed,
                EmbedInps = _embed_inps,
                IsPromptRun = _is_prompt_run,
                ConsumedTokensCount = _consumedTokensCount,
                Embeds = _embeds,
                LastTokens = _last_n_tokens.ToArray(),
                LLamaNewlineTokens = _llama_token_newline,
                MatchingSessionTokensCount = _n_matching_session_tokens,
                PastTokensCount = _pastTokensCount,
                SessionFilePath = _pathSession,
                SessionTokens = _session_tokens,
                LastTokensCapacity = _last_n_tokens.Capacity
            };
            using(FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                JsonSerializer.Serialize<InteractiveExecutorState>(fs, state);
            }
        }
        public override void LoadState(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = JsonSerializer.Deserialize<InteractiveExecutorState>(fs);
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps;
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds;
                _last_n_tokens = new FixedSizeQueue<llama_token>(state.LastTokensCapacity, state.LastTokens);
                _llama_token_newline = state.LLamaNewlineTokens;
                _n_matching_session_tokens = state.MatchingSessionTokensCount;
                _pastTokensCount = state.PastTokensCount;
                _pathSession = state.SessionFilePath;
                _session_tokens = state.SessionTokens;
            }
        }

        /// <summary>
        /// Define whether to continue the loop to generate responses.
        /// </summary>
        /// <returns></returns>
        protected override bool GetLoopCondition(InferStateArgs args)
        {
            return args.RemainedTokens != 0 && !args.WaitForInput || _is_prompt_run;
        }

        protected override void PreprocessInputs(string text, InferStateArgs args)
        {
            if (_is_prompt_run)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
                text = " " + text;
                _embed_inps = _model.Tokenize(text, true).ToList();
            }
            else
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                var line_inp = _model.Tokenize(text, false);
                _embed_inps.AddRange(line_inp);
                args.RemainedTokens -= line_inp.Count();
            }
        }

        /// <summary>
        /// Return whether to break the generation.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool PostProcess(InferenceParams inferenceParams, InferStateArgs args, out IEnumerable<string>? extraOutputs)
        {
            extraOutputs = null;
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                {
                    string last_output = "";
                    foreach (var id in _last_n_tokens)
                    {
                        last_output += Utils.PtrToString(NativeApi.llama_token_to_str(_model.NativeHandle, id), _model.Encoding);
                    }

                    foreach (var antiprompt in args.Antiprompts)
                    {
                        if (last_output.EndsWith(antiprompt))
                        {
                            args.WaitForInput = true;
                            break;
                        }
                    }
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return true;
                }
            }

            if (_embeds.Count > 0 && _embeds.Last() == NativeApi.llama_token_eos())
            {
                extraOutputs = new string[] { " [end of text]\n" };
                return true;
            }

            if (args.RemainedTokens <= 0 && inferenceParams.MaxTokens != -1)
            {
                args.RemainedTokens = inferenceParams.MaxTokens;
                args.WaitForInput = true;
            }
            return false;
        }

        protected override void InferInternal(InferenceParams inferenceParams, InferStateArgs args)
        {
            if (_embeds.Count > 0)
            {
                _is_prompt_run = false;
                if (_pastTokensCount + _embeds.Count > _model.ContextSize)
                {
                    HandleRunOutOfContext(inferenceParams.TokensKeep);
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

            if (_embed_inps.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? _model.ContextSize : inferenceParams.RepeatLastTokensCount;

                // optionally save the session on first sample (for faster prompt loading next time)
                if (!string.IsNullOrEmpty(_pathSession) && args.NeedToSaveSession)
                {
                    args.NeedToSaveSession = false;
                    SaveSessionFile(_pathSession);
                }

                var tokenDataArray = _model.ApplyPenalty(_last_n_tokens, inferenceParams.LogitBias, repeat_last_n,
                    inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                var id = _model.Sample(tokenDataArray, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau, 
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP);

                _last_n_tokens.Enqueue(id);

                if (id == NativeApi.llama_token_eos())
                {
                    id = _llama_token_newline.First();
                    if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                    {
                        var first_antiprompt = _model.Tokenize(args.Antiprompts[0], false);
                        _embed_inps.AddRange(first_antiprompt);
                    }
                }

                _embeds.Add(id);

                args.RemainedTokens--;
                args.ReturnValue = true;
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
        }

        public class InteractiveExecutorState : ExecutorBaseState
        {
            [JsonPropertyName("is_prompt_run")]
            public bool IsPromptRun { get; set; }
            [JsonPropertyName("llama_token_newline")]
            public llama_token[] LLamaNewlineTokens { get; set; }
        }
    }
}
