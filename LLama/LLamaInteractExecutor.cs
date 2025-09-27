using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Common;
using LLama;
using LLama.Exceptions;
using LLama.Native;
using LLama.Sampling;
using Microsoft.Extensions.Logging;


namespace LLama
{
    /// <summary>
    /// The LLama executor for interactive mode.
    /// </summary>
    public class InteractiveExecutor : StatefulExecutorBase
    {
        // Indicates whether the executor is currently evaluating the initial prompt or a follow-up turn.
        private bool _is_prompt_run = true;

        // MTMD multimodal state
        private SafeMtmdInputChunks? _mtmdChunks;  // Pending chunk collection produced by the multimodal tokenizer.
        private string? _mtmdMarker;  // Cached multimodal marker returned by the native helper.


        /// <summary>
        /// Create an interactive executor for text-only inference.
        /// </summary>
        /// <param name="context">LLama context to operate against.</param>
        /// <param name="logger">Optional logger for diagnostic output.</param>
        public InteractiveExecutor(LLamaContext context, ILogger? logger = null)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Create an interactive multimodal executor that can process text alongside media inputs.
        /// </summary>
        /// <param name="context">LLama context to operate against.</param>
        /// <param name="clipModel">Multimodal weights (MTMD) to attach to the executor.</param>
        /// <param name="logger">Optional logger for diagnostic output.</param>
        public InteractiveExecutor(LLamaContext context, SafeMtmdWeights clipModel, ILogger? logger = null)
            : base(context, clipModel, logger)
        {
        }

        /// <inheritdoc />
        public override ExecutorBaseState GetStateData()
        {
            InteractiveExecutorState state = new()
            {
                ConsumedSessionCount = _n_session_consumed,
                EmbedInps = _embed_inps.ToArray(),
                IsPromptRun = _is_prompt_run,
                ConsumedTokensCount = _consumedTokensCount,
                Embeds = _embeds.ToArray(),
                LastTokens = _last_n_tokens.ToArray(),
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
            if (data is InteractiveExecutorState state)
            {
                _n_session_consumed = state.ConsumedSessionCount;
                _embed_inps = state.EmbedInps!.ToList();
                _is_prompt_run = state.IsPromptRun;
                _consumedTokensCount = state.ConsumedTokensCount;
                _embeds = state.Embeds!.ToList();
                _last_n_tokens = new FixedSizeQueue<LLamaToken>(state.LastTokensCapacity, state.LastTokens!);
                _n_matching_session_tokens = state.MatchingSessionTokensCount;
                _pastTokensCount = state.PastTokensCount;
                _pathSession = state.SessionFilePath;
                _session_tokens = state.SessionTokens!.ToList();
            }
            else
                throw new ArgumentException("Invalid state data type.");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task SaveState(string filename, CancellationToken cancellationToken = default)
        {
            var state = (InteractiveExecutorState)GetStateData();
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, state, cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc />
        public override async Task LoadState(string filename, CancellationToken cancellationToken = default)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            var state = await JsonSerializer.DeserializeAsync<InteractiveExecutorState>(fs);
            await LoadState(state!, cancellationToken);
        }

        /// <summary>
        /// Decide whether generation should continue for the current iteration.
        /// </summary>
        /// <param name="args">Mutable inference state.</param>
        /// <returns><c>true</c> to keep generating; otherwise <c>false</c>.</returns>
        protected override Task<bool> GetLoopCondition(InferStateArgs args, CancellationToken cancellationToken)
        {
            return Task.FromResult(args.RemainedTokens != 0 && !args.WaitForInput || _is_prompt_run);
        }

        /// <summary>
        /// Preprocess the incoming prompt or continuation text before inference.
        /// </summary>
        /// <param name="text">Prompt text or continuation provided by the caller.</param>
        /// <param name="args">Mutable inference state.</param>
        protected override Task PreprocessInputs(string? text, InferStateArgs args, CancellationToken cancellationToken)
        {
            if (_is_prompt_run)
            {
                // When running the first input (prompt) in interactive mode, we should specially process it.
                if (text == null)
                {
                    throw new ArgumentException("Prompt cannot be null to trigger continuation if a prompt has not been provided previously.");
                }

                if (!IsMultiModal)
                {
                    _embed_inps = Context.Tokenize(text, true, true).ToList();
                }
                else
                {
                    PreprocessMtmd(text, args, true);
                }
            }
            else
            {
                // Don't add any tokens if continuation is requested (by providing a null prompt)
                if (text != null)
                {
                    if (!text.EndsWith("\n"))
                    {
                        text += "\n";
                    }

                    if (!IsMultiModal)
                    {
                        var line_inp = Context.Tokenize(text, false, true);
                        _embed_inps.AddRange(line_inp);
                        args.RemainedTokens -= line_inp.Length;
                    }
                    else
                    {
                        PreprocessMtmd(text, args, false);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Release any queued multimodal chunks and reset state.
        /// </summary>
        private void DisposeMtmdChunks()
        {
            _mtmdChunks?.Dispose();
            _mtmdChunks = null;
        }

        /// <summary>
        /// Dispose and clear any pending multimodal embeddings queued for evaluation.
        /// </summary>
        private void DisposeEmbeds()
        {
            if (Embeds.Count == 0)
            {
                return;
            }

            foreach (var embed in Embeds)
            {
                embed.Dispose();
            }

            Embeds.Clear();
        }

        /// <summary>
        /// Retrieve the marker token used to signal media segments to the tokenizer.
        /// </summary>
        private string GetMtmdMarker()
        {
            if (_mtmdMarker is not null)
            {
                return _mtmdMarker;
            }

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

        /// <summary>
        /// Preprocess multimodal prompts by aligning media markers and tokenizing via MTMD helpers.
        /// </summary>
        /// <param name="text">Prompt text containing optional media markers.</param>
        /// <param name="args">Mutable inference state.</param>
        /// <param name="addBos">Whether to treat the prompt as a fresh run and add the BOS token.</param>
        private Task PreprocessMtmd(string text, InferStateArgs args, bool addBos = true)
        {
            if (ClipModel is null)
            {
                throw new InvalidOperationException("Multimodal execution requires a loaded mtmd clip model.");
            }

            DisposeMtmdChunks();

            var marker = GetMtmdMarker();
            var prompt = text;

            if (Embeds.Count > 0)
            {
                if (prompt.Contains("<image>"))
                {
                    prompt = prompt.Replace("<image>", marker);
                }

                if (!prompt.Contains(marker))
                {
                    var suffix = string.Concat(Enumerable.Repeat(marker, Embeds.Count));  // Ensure tokenizer sees one marker per embed.
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

                _mtmdChunks = chunks;  // Own the chunk collection until evaluation completes.

                var tokens = new List<LLamaToken>();
                foreach (var chunk in chunks.Enumerate())
                {
                    using var scopedChunk = chunk;
                    if (scopedChunk.Type != SafeMtmdInputChunk.SafeMtmdInputChunkType.Text)
                    {
                        continue;
                    }

                    foreach (var token in scopedChunk.GetTextTokensSpan())
                    {
                        tokens.Add(unchecked((int)token));
                    }
                }

                var totalPositions = (int)ClipModel.CountPositions(chunks);
                var fillerToken = GetFillerToken(marker);

                if (addBos)
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
                DisposeEmbeds();  // Flush any embeds decoded in prior step; MTMD replays them via chunk eval.
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Decide whether generation should stop based on antiprompts, token limits, or end-of-generation markers.
        /// </summary>
        /// <param name="inferenceParams">Sampling parameters controlling generation.</param>
        /// <param name="args">Mutable inference state.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Tuple describing whether to stop and any additional outputs to emit.</returns>
        protected override Task<(bool, IReadOnlyList<string>)> PostProcess(IInferenceParams inferenceParams, InferStateArgs args, CancellationToken cancellationToken)
        {
            if (_embed_inps.Count <= _consumedTokensCount)
            {
                if (!string.IsNullOrEmpty(args.LastOutput) && AntipromptProcessor.Add(args.LastOutput))
                {
                    args.WaitForInput = true;
                }

                if (_pastTokensCount > 0 && args.WaitForInput)
                {
                    return Task.FromResult((true, (IReadOnlyList<string>)[]));
                }
            }

            if (_embeds.Count > 0 && _embeds.Last().IsEndOfGeneration(Context.Vocab))
            {
                return Task.FromResult((true, (IReadOnlyList<string>)[]));
            }

            if (args.RemainedTokens <= 0 && inferenceParams.MaxTokens != -1)
            {
                args.RemainedTokens = inferenceParams.MaxTokens;
                args.WaitForInput = true;
            }

            return Task.FromResult((true, (IReadOnlyList<string>)[]));
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
                    // number of tokens to keep when resetting context
                    // Ported from https://github.com/ggerganov/llama.cpp/blob/60325fa56f61c228464c9f065db3aa6a61f2156e/examples/main/main.cpp#L334
                    var tokensToKeep = inferenceParams.TokensKeep;
                    if (tokensToKeep < 0 || tokensToKeep > _embed_inps.Count)
                    {
                        tokensToKeep = _embed_inps.Count;
                    }
                    else
                    {
                        tokensToKeep += Convert.ToInt32(Context.Vocab.ShouldAddBOS); // always keep the BOS token
                    }

                    HandleRunOutOfContext(tokensToKeep);
                }

                if (_mtmdChunks is null)
                {
                    TryReuseMatchingPrefix();
                }

                if (IsMultiModal && _mtmdChunks is not null)
                {
                    var nPast = (long)_pastTokensCount;
                    var previousConsumed = _consumedTokensCount;
                    var evalStatus = ClipModel!.EvaluateChunks(_mtmdChunks, Context.NativeHandle, ref nPast, seqId: 0,
                        nBatch: checked((int)Context.BatchSize), logitsLast: true);
                    if (evalStatus != 0)
                    {
                        _logger?.LogError("[InteractiveExecutor] Failed to evaluate multimodal chunks. Status: {Status}", evalStatus);
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
                else
                {
                    var result = await Context.DecodeAsync(_embeds, LLamaSeqId.Zero, batch, _pastTokensCount);
                    _pastTokensCount = result.Item3;

                    if (result.Item1 != DecodeResult.Ok) throw new LLamaDecodeError(result.Item1);

                    if (_embeds.Count > 0 && !string.IsNullOrEmpty(_pathSession))
                    {
                        _session_tokens.AddRange(_embeds);
                        _n_session_consumed = _session_tokens.Count;
                    }
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
                    _logger?.LogError("[InteractiveExecutor] Failed to evaluate multimodal chunks. Status: {Status}", evalStatus);
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

                if (id == Context.NativeHandle.ModelHandle.Vocab.EOS)
                {
                    id = Context.NativeHandle.ModelHandle.Vocab.Newline!.Value;
                    if (args.Antiprompts is not null && args.Antiprompts.Count > 0)
                    {
                        var first_antiprompt = Context.Tokenize(args.Antiprompts[0], false, true);
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
                    if (_embeds.Count >= Context.BatchSize)
                    {
                        break;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Serializable state specific to the interactive executor.
        /// </summary>
        public class InteractiveExecutorState
            : StatefulExecutorBase.ExecutorBaseState
        {
            /// <summary>
            /// Whether the executor is running for the first time (running the prompt).
            /// </summary>
            [JsonPropertyName("is_prompt_run")]
            public bool IsPromptRun { get; set; }
        }
    }
}
