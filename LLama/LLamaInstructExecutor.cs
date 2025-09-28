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
using System.Threading.Tasks;
using LLama.Exceptions;
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
        private SafeMtmdInputChunks? _mtmdChunks;
        private string? _mtmdMarker;
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
                                SafeMtmdWeights clipModel,
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
        public override Task LoadState(ExecutorBaseState data)
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
                await LoadState(state!);
            }
        }

        /// <inheritdoc />
        protected override Task<bool> GetLoopCondition(InferStateArgs args)
        {
            return Task.FromResult(args.RemainedTokens != 0 || _is_prompt_run);
        }

        /// <inheritdoc />
        protected override Task PreprocessInputs(string? text, InferStateArgs args)
        {
            args.Antiprompts ??= [ ];
            if (!args.Antiprompts.Contains(_instructionPrefix))
                args.Antiprompts.Add(_instructionPrefix);

            if (_is_prompt_run)
            {
                // When running the first input (prompt) in inteactive mode, we should specially process it.
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

        private void DisposeMtmdChunks()
        {
            _mtmdChunks?.Dispose();
            _mtmdChunks = null;
        }

        private void DisposeEmbeds()
        {
            if (Embeds.Count == 0)
                return;

            foreach (var embed in Embeds)
                embed.Dispose();

            Embeds.Clear();
        }

        private string GetMtmdMarker()
        {
            if (_mtmdMarker is not null)
                return _mtmdMarker;

            _mtmdMarker = NativeApi.MtmdDefaultMarker() ?? "<media>";
            return _mtmdMarker;
        }

        private static List<LLamaToken> BuildTokensWithFiller(List<LLamaToken> tokens, int totalPositions, LLamaToken fillerToken)
        {
            if (totalPositions <= tokens.Count)
                return new List<LLamaToken>(tokens);

            var result = new List<LLamaToken>(totalPositions);
            result.AddRange(tokens);
            result.AddRange(Enumerable.Repeat(fillerToken, totalPositions - tokens.Count));
            return result;
        }

        private LLamaToken GetFillerToken(string marker)
        {
            var markerTokens = Context.Tokenize(marker, false, true);
            if (markerTokens.Length > 0)
                return markerTokens[markerTokens.Length - 1];

            var eos = Context.Vocab.EOS;
            if (eos.HasValue)
                return eos.Value;

            return default(LLamaToken);
        }

        private Task PreprocessMtmd(string text, InferStateArgs args, bool addBos, bool replaceExisting)
        {
            if (ClipModel is null)
                throw new InvalidOperationException("Multimodal execution requires a loaded mtmd clip model.");

            DisposeMtmdChunks();

            var marker = GetMtmdMarker();
            var prompt = text;

            if (Embeds.Count > 0)
            {
                if (prompt.Contains("<image>"))
                    prompt = prompt.Replace("<image>", marker);

                if (!prompt.Contains(marker))
                {
                    var suffix = string.Concat(Enumerable.Repeat(marker, Embeds.Count));
                    prompt = string.Concat(prompt, suffix);
                }
            }

            SafeMtmdInputChunks? chunks = null;
            try
            {
                var status = ClipModel.Tokenize(prompt, addBos, parseSpecial: true, out chunks);
                if (status != 0 || chunks is null)
                {
                    ClipModel.ClearMedia();
                    throw new RuntimeError($"Failed to tokenize multimodal prompt. Status: {status}.");
                }

                _mtmdChunks = chunks;

                var tokens = new List<LLamaToken>();
                foreach (var chunk in chunks.Enumerate())
                {
                    using var scopedChunk = chunk;
                    if (scopedChunk.Type != SafeMtmdInputChunk.SafeMtmdInputChunkType.Text)
                        continue;

                    foreach (var token in scopedChunk.GetTextTokensSpan())
                        tokens.Add(unchecked((int)token));
                }

                var totalPositions = (int)ClipModel.CountPositions(chunks);
                var fillerToken = GetFillerToken(marker);

                if (replaceExisting)
                {
                    _embed_inps = BuildTokensWithFiller(tokens, totalPositions, fillerToken);
                    _consumedTokensCount = 0;
                }
                else
                {
                    if (_embed_inps.Count == 0)
                        _embed_inps = new List<LLamaToken>();

                    _embed_inps.AddRange(tokens);
                    var fillerCount = totalPositions - tokens.Count;
                    if (fillerCount > 0)
                        _embed_inps.AddRange(Enumerable.Repeat(fillerToken, fillerCount));

                    args.RemainedTokens -= tokens.Count;
                }
            }
            catch
            {
                chunks?.Dispose();
                _mtmdChunks = null;
                throw;
            }
            finally
            {
                DisposeEmbeds();
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

            if (_embeds.Count > 0 && _embeds.Last() == Context.Vocab.EOS)
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
        protected override async Task InferInternal(IInferenceParams inferenceParams, InferStateArgs args)
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
                    throw new LLamaDecodeError(result);

                if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                {
                    _session_tokens.AddRange(_embeds);
                    _n_session_consumed = _session_tokens.Count;
                }
            }
            else if (IsMultiModal && _mtmdChunks is not null)
            {
                _is_prompt_run = false;
                var nPast = (long)_pastTokensCount;
                var previousConsumed = _consumedTokensCount;
                var evalStatus = ClipModel!.EvaluateChunks(_mtmdChunks, Context.NativeHandle, ref nPast, seqId: 0, nBatch: checked((int)Context.BatchSize), logitsLast: true);
                if (evalStatus != 0)
                {
                    _logger?.LogError("[InstructExecutor] Failed to evaluate multimodal chunks. Status: {Status}", evalStatus);
                    DisposeMtmdChunks();
                    throw new RuntimeError($"Failed to evaluate multimodal chunks. Status: {evalStatus}.");
                }

                _pastTokensCount = checked((int)nPast);
                DisposeMtmdChunks();

                if (!string.IsNullOrEmpty(_pathSession) && _embed_inps.Count > previousConsumed)
                {
                    _session_tokens.AddRange(_embed_inps.Skip(previousConsumed));
                    _n_session_consumed = _session_tokens.Count;
                }

                _consumedTokensCount = _embed_inps.Count;
                _embeds.Clear();
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
