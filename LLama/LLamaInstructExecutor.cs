using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LLama.Exceptions;
using LLama.Extensions;
using Microsoft.Extensions.Logging;

namespace LLama
{
    /// <summary>
    /// The LLama executor for instruct mode.
    /// </summary>
    public class InstructExecutor
        : StatefulExecutorBase
    {
        private bool _is_prompt_run = true;
        private readonly string _instructionPrefix;
        private LLamaToken[] _inp_pfx;
        private LLamaToken[] _inp_sfx;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instructionPrefix"></param>
        /// <param name="instructionSuffix"></param>
        /// <param name="logger"></param>
        public InstructExecutor(LLamaContext context,
                                string instructionPrefix = "\n\n### Instruction:\n\n",
                                string instructionSuffix = "\n\n### Response:\n\n",
                                ILogger? logger = null)
            : base(context, logger)
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
        public override Task LoadState(ExecutorBaseState data)
        {
            if(data is InstructExecutorState state)
            {
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps;
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds;
                _last_n_tokens = new FixedSizeQueue<LLamaToken>(state.LastTokensCapacity, state.LastTokens);
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

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task SaveState(string filename)
        {
            var state = (InstructExecutorState)GetStateData();
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, state);
            }
        }
        /// <inheritdoc />
        public override async Task LoadState(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = await JsonSerializer.DeserializeAsync<InstructExecutorState>(fs);
                await LoadState(state);
            }
        }

        /// <inheritdoc />
        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 || _is_prompt_run);
        }

        /// <inheritdoc />
        protected override Task PreprocessInputs(string text, InferStateArgs args)
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

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override async Task<(bool, IReadOnlyList<string>)> PostProcess(IInferenceParams inferenceParams, InferStateArgs args)
        {
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (_last_n_tokens.TokensEndsWithAnyString(args.Antiprompts, Context.NativeHandle.ModelHandle, Context.Encoding))
                {
                    args.WaitForInput = true;
                    return (true, Array.Empty<string>());
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return (true, new[] { "\n> " });
                }
            }

            if (_embeds.Count > 0 && _embeds.Last() == NativeApi.llama_token_eos(Context.NativeHandle.ModelHandle))
            {
                args.WaitForInput = true;
            }

            if (args.RemainedTokens <= 0 && inferenceParams.MaxTokens != -1)
            {
                args.RemainedTokens = inferenceParams.MaxTokens;
                args.WaitForInput = true;
            }
            return (false, Array.Empty<string>());
        }

        /// <inheritdoc />
        protected override Task InferInternal(IInferenceParams inferenceParams, InferStateArgs args)
        {
            var batch = new LLamaBatch();

            if (_embeds.Count > 0)
            {
                _is_prompt_run = false;
                if (_pastTokensCount + _embeds.Count > Context.ContextSize)
                {
                    HandleRunOutOfContext(inferenceParams.TokensKeep);
                }

                TryReuseMathingPrefix();

                var (result, _) = Context.NativeHandle.Decode(_embeds, LLamaSeqId.Zero, batch, ref _pastTokensCount);
                if (result != DecodeResult.Ok)
                    throw new LLamaDecodeError(result);

                if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                {
                    _session_tokens.AddRange(_embeds);
                    _n_session_consumed = _session_tokens.Count;
                }
            }

            _embeds.Clear();

            if (_embed_inps.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? (int)Context.ContextSize : inferenceParams.RepeatLastTokensCount;

                // optionally save the session on first sample (for faster prompt loading next time)
                if (!string.IsNullOrEmpty(_pathSession) && args.NeedToSaveSession)
                {
                    args.NeedToSaveSession = false;
                    SaveSessionFile(_pathSession);
                }

                LLamaToken id;
                if (inferenceParams.SamplingPipeline is not null)
                {
                    id = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, Context.NativeHandle.GetLogitsIth(batch.TokenCount - 1), _last_n_tokens.ToArray());
                    inferenceParams.SamplingPipeline.Accept(Context.NativeHandle, id);
                }
                else
                {
                    var tokenDataArray = Context.ApplyPenalty(batch.TokenCount - 1, _last_n_tokens, inferenceParams.LogitBias, repeat_last_n,
                        inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                    var mu = MirostatMu;
                    id = Context.Sample(
                        tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                        inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP, inferenceParams.Grammar,
                        inferenceParams.MinP
                    );
                    MirostatMu = mu;
                }

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

            return Task.CompletedTask;
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
            public LLamaToken[] InputPrefixTokens { get; set; }
            /// <summary>
            /// Instruction suffix tokens.
            /// </summary>
            [JsonPropertyName("inp_sfx")]
            public LLamaToken[] InputSuffixTokens { get; set; }
        }
    }
}
