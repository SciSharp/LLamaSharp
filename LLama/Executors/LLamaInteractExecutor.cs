﻿using LLama.Common;
using LLama.Native;
using LLama.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LLama.Executors
{
    using llama_token = Int32;
    /// <summary>
    /// The LLama executor for interactive mode.
    /// </summary>
    public class InteractiveExecutor : StatefulExecutorBase<InteractiveExecutorState>
    {
        bool _is_prompt_run = true;
        llama_token[] _llama_token_newline;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stateFile"></param>
        public InteractiveExecutor(LLamaModel model, string stateFile = "ExecutorState.json")
            : base(model, stateFile)
        {
            _llama_token_newline = _model.NativeHandle.Tokenize("\n", false, _model.Encoding);
        }

        /// <inheritdoc />
        public override InteractiveExecutorState GetStateData()
        {
            return new()
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
                LastTokensCapacity = _last_n_tokens.Capacity,
                MirostateMu = MirostateMu
            };
        }
        /// <inheritdoc />
        public override void LoadState(InteractiveExecutorState state)
        {
            if (state is null)
                throw new ArgumentException("Invalid state data type.");

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

        /// <inheritdoc />
        public override void SaveState(string filename)
        {
            InteractiveExecutorState state = GetStateData() as InteractiveExecutorState;
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                JsonSerializer.Serialize<InteractiveExecutorState>(fs, state);
            }
        }
        /// <inheritdoc />
        public override void LoadState(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = JsonSerializer.Deserialize<InteractiveExecutorState>(fs);
                LoadState(state);
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

        /// <inheritdoc />
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
                args.RemainedTokens -= line_inp.Length;
            }
        }

        /// <summary>
        /// Return whether to break the generation.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool PostProcess(IInferenceParams inferenceParams, InferStateArgs args, out IEnumerable<string>? extraOutputs)
        {
            extraOutputs = null;
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                {
                    string last_output = "";
                    foreach (var id in _last_n_tokens)
                    {
                        last_output += _model.NativeHandle.TokenToString(id, _model.Encoding);
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

        /// <inheritdoc />
        protected override void InferInternal(IInferenceParams inferenceParams, InferStateArgs args)
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

                var mu = MirostateMu;
                var id = _model.Sample(
                    tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP
                );
                MirostateMu = mu;

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


    }
}
