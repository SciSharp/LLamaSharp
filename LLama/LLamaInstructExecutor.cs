using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLama
{
    using llama_token = Int32;
    /// <summary>
    /// The LLama executor for instruct mode.
    /// </summary>
    public class InstructExecutor : StatefulExecutorBase
    {
        bool _is_prompt_run = true;
        string _instructionPrefix;
        llama_token[] _inp_pfx;
        llama_token[] _inp_sfx;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instructionPrefix"></param>
        /// <param name="instructionSuffix"></param>
        public InstructExecutor(LLamaContext context, string instructionPrefix = "\n\n### Instruction:\n\n",
            string instructionSuffix = "\n\n### Response:\n\n") : base(context)
        {
            _inp_pfx = Context.Tokenize(instructionPrefix, true);
            _inp_sfx = Context.Tokenize(instructionSuffix, false);
            _instructionPrefix = instructionPrefix;
        }

        /// <inheritdoc />
        public override ExecutorBaseState GetStateData()
        {
            InstructExecutorState state = new()
            {
                ConsumedSessionCount = _n_session_consumed,
                EmbedInps = _embed_inps,
                IsPromptRun = _is_prompt_run,
                ConsumedTokensCount = _consumedTokensCount,
                Embeds = _embeds,
                LastTokens = _last_n_tokens.ToArray(),
                InputPrefixTokens = _inp_pfx,
                InputSuffixTokens = _inp_sfx,
                MatchingSessionTokensCount = _n_matching_session_tokens,
                PastTokensCount = _pastTokensCount,
                SessionFilePath = _pathSession,
                SessionTokens = _session_tokens,
                LastTokensCapacity = _last_n_tokens.Capacity,
                MirostatMu = MirostatMu
            };
            return state;
        }
        /// <inheritdoc />
        public override void LoadState(ExecutorBaseState data)
        {
            if(data is InstructExecutorState state)
            {
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps;
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds;
                _last_n_tokens = new FixedSizeQueue<llama_token>(state.LastTokensCapacity, state.LastTokens);
                _inp_pfx = state.InputPrefixTokens;
                _inp_sfx = state.InputSuffixTokens;
                _n_matching_session_tokens = state.MatchingSessionTokensCount;
                _pastTokensCount = state.PastTokensCount;
                _pathSession = state.SessionFilePath;
                _session_tokens = state.SessionTokens;
            }
            else
            {
                throw new ArgumentException("Invalid state data type.");
            }
        }

        /// <inheritdoc />
        public override void SaveState(string filename)
        {
            var state = (InstructExecutorState)GetStateData();
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                JsonSerializer.Serialize(fs, state);
            }
        }
        /// <inheritdoc />
        public override void LoadState(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = JsonSerializer.Deserialize<InstructExecutorState>(fs);
                LoadState(state);
            }
        }

        /// <inheritdoc />
        protected override bool GetLoopCondition(InferStateArgs args)
        {
            return args.RemainedTokens != 0 || _is_prompt_run;
        }
        /// <inheritdoc />
        protected override void PreprocessInputs(string text, InferStateArgs args)
        {
            args.Antiprompts ??= new List<string>();
            args.Antiprompts.Add(_instructionPrefix);
            if (_is_prompt_run)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
                _embed_inps = Context.Tokenize(text, true).ToList();
            }
            else
            {
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                _consumedTokensCount = _embed_inps.Count;
                _embed_inps.AddRange(_inp_pfx);

                var line_inp = Context.Tokenize(text, false);
                _embed_inps.AddRange(line_inp);

                _embed_inps.AddRange(_inp_sfx);

                args.RemainedTokens -= line_inp.Length;
            }
        }
        /// <inheritdoc />
        protected override bool PostProcess(IInferenceParams inferenceParams, InferStateArgs args, out IEnumerable<string>? extraOutputs)
        {
            extraOutputs = null;
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                {
                    var last_output_builder = new StringBuilder();
                    foreach (var token in _last_n_tokens)
                        Context.NativeHandle.TokenToString(token, Context.Encoding, last_output_builder);
                    var last_output = last_output_builder.ToString();

                    foreach (var antiprompt in args.Antiprompts)
                    {
                        if (last_output.EndsWith(antiprompt))
                        {
                            args.WaitForInput = true;
                            return true;
                        }
                    }
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    extraOutputs = new[] { "\n> " };
                    return true;
                }
            }

            if (_embeds.Count > 0 && _embeds.Last() == NativeApi.llama_token_eos(Context.NativeHandle))
            {
                args.WaitForInput = true;
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
                if (_pastTokensCount + _embeds.Count > Context.ContextSize)
                {
                    HandleRunOutOfContext(inferenceParams.TokensKeep);
                }

                TryReuseMathingPrefix();
                _pastTokensCount = Context.Eval(_embeds, _pastTokensCount);

                if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                {
                    _session_tokens.AddRange(_embeds);
                    _n_session_consumed = _session_tokens.Count;
                }
            }

            _embeds.Clear();

            if (_embed_inps.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? Context.ContextSize : inferenceParams.RepeatLastTokensCount;

                // optionally save the session on first sample (for faster prompt loading next time)
                if (!string.IsNullOrEmpty(_pathSession) && args.NeedToSaveSession)
                {
                    args.NeedToSaveSession = false;
                    SaveSessionFile(_pathSession);
                }

                var tokenDataArray = Context.ApplyPenalty(_last_n_tokens, inferenceParams.LogitBias, repeat_last_n,
                    inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                var mu = MirostatMu;
                var id = Context.Sample(
                    tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP,
                    inferenceParams.Grammar
                );
                MirostatMu = mu;

                _last_n_tokens.Enqueue(id);

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
                    if (_embeds.Count >= Context.Params.BatchSize)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// The desciptor of the state of the instruct executor.
        /// </summary>
        public class InstructExecutorState : ExecutorBaseState
        {
            /// <summary>
            /// Whether the executor is running for the first time (running the prompt).
            /// </summary>
            [JsonPropertyName("is_prompt_run")]
            public bool IsPromptRun { get; set; }
            /// <summary>
            /// Instruction prefix tokens.
            /// </summary>
            [JsonPropertyName("inp_pfx")]
            public llama_token[] InputPrefixTokens { get; set; }
            /// <summary>
            /// Instruction suffix tokens.
            /// </summary>
            [JsonPropertyName("inp_sfx")]
            public llama_token[] InputSuffixTokens { get; set; }
        }
    }
}
