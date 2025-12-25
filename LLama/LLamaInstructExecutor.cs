using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using LLama.Sampling;
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
        private readonly string _instructionSuffix;

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
            _inp_pfx = Context.Tokenize(instructionPrefix, true, true);
            _inp_sfx = Context.Tokenize(instructionSuffix, false, true);
            _instructionPrefix = instructionPrefix;
            _instructionSuffix = instructionSuffix;
        }

        public InstructExecutor(LLamaContext context,
                                MtmdWeights clipModel,
                                string instructionPrefix = "\n\n### Instruction:\n\n",
                                string instructionSuffix = "\n\n### Response:\n\n",
                                ILogger? logger = null)
            : base(context, clipModel, logger)
        {
            _inp_pfx = Context.Tokenize(instructionPrefix, true, true);
            _inp_sfx = Context.Tokenize(instructionSuffix, false, true);
            _instructionPrefix = instructionPrefix;
            _instructionSuffix = instructionSuffix;
        }

        /// <inheritdoc />
        public override ExecutorBaseState GetStateData()
        {
            InstructExecutorState state = new()
            {
                ConsumedSessionCount = _n_session_consumed,
                EmbedInps = _embed_inps.ToArray(),
                IsPromptRun = _is_prompt_run,
                ConsumedTokensCount = _consumedTokensCount,
                Embeds = _embeds.ToArray(),
                LastTokens = _last_n_tokens.ToArray(),
                InputPrefixTokens = _inp_pfx,
                InputSuffixTokens = _inp_sfx,
                MatchingSessionTokensCount = _n_matching_session_tokens,
                PastTokensCount = _pastTokensCount,
                SessionFilePath = _pathSession,
                SessionTokens = _session_tokens.ToArray(),
                LastTokensCapacity = _last_n_tokens.Capacity,
            };
            return state;
        }
        /// <inheritdoc />
        public override Task LoadState(ExecutorBaseState data, CancellationToken cancellationToken = default)
        {
            DisposeMtmdChunks();
            if(data is InstructExecutorState state)
            {
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps!.ToList();
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds!.ToList();
                _last_n_tokens = new FixedSizeQueue<LLamaToken>(state.LastTokensCapacity, state.LastTokens!);
                _inp_pfx = state.InputPrefixTokens!;
                _inp_sfx = state.InputSuffixTokens!;
                _n_matching_session_tokens = state.MatchingSessionTokensCount;
                _pastTokensCount = state.PastTokensCount;
                _pathSession = state.SessionFilePath;
                _session_tokens = state.SessionTokens!.ToList();
            }
            else
            {
                throw new ArgumentException("Invalid state data type.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task SaveState(string filename, CancellationToken cancellationToken = default)
        {
            var state = (InstructExecutorState)GetStateData();
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, state, cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc />
        public override async Task LoadState(string filename, CancellationToken cancellationToken = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var state = await JsonSerializer.DeserializeAsync<InstructExecutorState>(fs);
                await LoadState(state!, cancellationToken);
            }
        }

        /// <inheritdoc />
        protected override Task<bool> GetLoopCondition(InferStateArgs args, CancellationToken cancellationToken)
        {
            return Task.FromResult(args.RemainedTokens != 0 || _is_prompt_run);
        }

        /// <inheritdoc />
        protected override Task PreprocessInputs(string? text, InferStateArgs args, CancellationToken cancellationToken)
        {
            args.Antiprompts ??= [];
            if (!args.Antiprompts.Contains(_instructionPrefix))
                args.Antiprompts.Add(_instructionPrefix);

            if (_is_prompt_run)
            {
                // When running the first input (prompt) in interactive mode, we should specially process it.
                if (text == null) throw new ArgumentException("Prompt cannot be null to trigger continuation if a prompt has not been provided previously.");
                if (!IsMultiModal)
                {
                    _embed_inps = Context.Tokenize(text, true, true).ToList();
                }
                else
                {
                    return PreprocessMtmd(text, args, addBos: true, replaceExisting: true);
                }
            }
            else
            {
                _consumedTokensCount = _embed_inps.Count;

                // Don't append the template tokens if continuation is requested (by providing a null prompt)
                if (text != null)
                {
                    if (!text.EndsWith("\n"))
                    {
                        text += "\n";
                    }
                    if (!IsMultiModal)
                    {
                        _embed_inps.AddRange(_inp_pfx);

                        var line_inp = Context.Tokenize(text, false, true);
                        _embed_inps.AddRange(line_inp);

                        _embed_inps.AddRange(_inp_sfx);

                        args.RemainedTokens -= line_inp.Length;
                    }
                    else
                    {
                        var builder = new StringBuilder();
                        builder.Append(_instructionPrefix);
                        builder.Append(text);
                        builder.Append(_instructionSuffix);
                        return PreprocessMtmd(builder.ToString(), args, addBos: false, replaceExisting: false);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<(bool, IReadOnlyList<string>)> PostProcess(IInferenceParams inferenceParams, InferStateArgs args, CancellationToken cancellationToken)
        {
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (!string.IsNullOrEmpty(args.LastOutput) && AntipromptProcessor.Add(args.LastOutput))
                {
                    args.WaitForInput = true;
                    return Task.FromResult<(bool, IReadOnlyList<string>)>((true, []));
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return Task.FromResult<(bool, IReadOnlyList<string>)>((true, ["\n> "]));
                }
            }

            if (_embeds.Count > 0 && _embeds.Last() == Context.Vocab.EOS)
            {
                args.WaitForInput = true;
            }

            if (args.RemainedTokens <= 0 && inferenceParams.MaxTokens != -1)
            {
                args.RemainedTokens = inferenceParams.MaxTokens;
                args.WaitForInput = true;
            }
            return Task.FromResult<(bool, IReadOnlyList<string>)>((false, []));
        }

        /// <inheritdoc />
        protected override async Task InferInternal(IInferenceParams inferenceParams, InferStateArgs args, CancellationToken cancellationToken)
        {
            var batch = new LLamaBatch();

            if (_embeds.Count > 0)
            {
                _is_prompt_run = false;
                if (_pastTokensCount + _embeds.Count > Context.ContextSize)
                {
                    // Ported from https://github.com/ggerganov/llama.cpp/blob/60325fa56f61c228464c9f065db3aa6a61f2156e/examples/main/main.cpp#L334
                    // Instruct always uses input token size.
                    var tokensToKeep = _embed_inps.Count;
                    HandleRunOutOfContext(tokensToKeep);
                }

                TryReuseMatchingPrefix();

                var (result, _, pastTokensCount) = await Context.DecodeAsync(_embeds, LLamaSeqId.Zero, batch, _pastTokensCount);
                _pastTokensCount = pastTokensCount;

                if (result != DecodeResult.Ok)
                {
                    throw new LLamaDecodeError(result);
                }

                if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                {
                    _session_tokens.AddRange(_embeds);
                    _n_session_consumed = _session_tokens.Count;
                }
            }
            else if (IsMultiModal && MtmdChunks is not null)
            {
                _is_prompt_run = false;
                var nPast = _pastTokensCount;
                var previousConsumed = _consumedTokensCount;
                EvaluateMtmdChunks(ref nPast, previousConsumed, nameof(InstructExecutor));
            }

            _embeds.Clear();

            if (_embed_inps.Count <= _consumedTokensCount && !args.WaitForInput)
            {
                if (inferenceParams.MaxTokens == 0)
                {
                    _embeds.Clear();
                    args.WaitForInput = true;
                    args.ReturnValue = false;
                    return;
                }
                // optionally save the session on first sample (for faster prompt loading next time)
                if (!string.IsNullOrEmpty(_pathSession) && args.NeedToSaveSession)
                {
                    args.NeedToSaveSession = false;
                    SaveSessionFile(_pathSession!);
                }

                // Sample with the pipeline
                var id = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, batch.TokenCount - 1);

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
                    if (_embeds.Count >= Context.BatchSize)
                    {
                        break;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// The descriptor of the state of the instruct executor.
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
            public LLamaToken[]? InputPrefixTokens { get; set; }
            /// <summary>
            /// Instruction suffix tokens.
            /// </summary>
            [JsonPropertyName("inp_sfx")]
            public LLamaToken[]? InputSuffixTokens { get; set; }
        }
    }
}
