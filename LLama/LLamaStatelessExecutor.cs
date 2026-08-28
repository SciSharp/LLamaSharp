using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LLama.Exceptions;
using LLama.Native;
using LLama.Transformers;
using Microsoft.Extensions.Logging;

namespace LLama
{
    /// <summary>
    /// This executor infer the input as one-time job. Previous inputs won't impact on the 
    /// response to current input.
    /// </summary>
    public class StatelessExecutor
        : ILLamaExecutor
    {
        private readonly LLamaWeights _weights;
        private readonly IContextParams _params;
        private readonly ILogger? _logger;
        private readonly LLamaBatch _batch;

        #region speculative
        private readonly LLamaWeights? _draftWeights;
        private readonly IContextParams? _draftParams;
        private readonly int _draftTokens;
        private readonly bool _useMtp;
        /// <summary>
        /// The total number of draft tokens proposed by the draft model (or MTP heads) during the lifetime of this executor.
        /// </summary>
        public int TotalDraftTokensProposed { get; private set; }
        /// <summary>
        /// The total number of proposed draft tokens that were successfully verified and accepted by the target model. 
        /// <para>Note: This metric strictly counts accepted drafts and excludes the standard base token sampled during the verification phase.</para>
        /// </summary>
        public int TotalDraftTokensAccepted { get; private set; }
        /// <summary>
        /// The ratio of accepted draft tokens to proposed draft tokens (ranging from 0.0 to 1.0). 
        /// <para>A higher acceptance rate (e.g., > 0.4) generally indicates a more accurate draft model and a higher potential for generation speedups, depending on hardware memory bandwidth bottlenecks.</para>
        /// </summary>
        public double AcceptanceRate => TotalDraftTokensProposed == 0
            ? 0.0
            : (double)TotalDraftTokensAccepted / TotalDraftTokensProposed;
        #endregion

        /// <inheritdoc />
        public bool IsMultiModal => false;

        /// <inheritdoc />
        public MtmdWeights? ClipModel => default;

        /// <inheritdoc />
        public List<SafeMtmdEmbed> Embeds { get; }

        /// <summary>
        /// The context used by the executor when running the inference.
        /// </summary>
        public LLamaContext Context { get; private set; }

        /// <summary>
        /// If true, applies the default template to the prompt as defined in the rules for <a href="https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template">llama_chat_apply_template</a> template.  
        /// </summary>
        public bool ApplyTemplate { get; init; }

        /// <summary>
        /// The system message to use with the prompt. Only used when <see cref="ApplyTemplate" /> is true.
        /// </summary>
        public string? SystemMessage { get; init; }

        /// <summary>
        /// Creates a new stateless executor for inference. Supports Standard, Dual-Model Speculative, and MTP (Multi-Token Prediction) modes.
        /// <para><b>Dual-Model Speculation:</b> If using two different models, the target and draft models must share the exact same tokenizer architecture and vocabulary size (e.g., Llama 3.1 8B + 1B). A mismatch will cause cache desynchronization crashes.</para>
        /// <para><b>Performance Note:</b> For speculative decoding to yield a speedup, both models (or the full MTP model) must fit entirely within GPU VRAM, and the target model should be large enough (e.g., 8B+) to be memory-bandwidth bound.</para>
        /// </summary>
        /// <param name="weights">The weights of the primary target model.</param>
        /// <param name="params">The context parameters for the primary target model.</param>
        /// <param name="draftWeights">The weights of the draft model. <br/><b>Important:</b> In MTP mode, this parameter is ignored and the executor internally re-uses the target weights for the draft context. Therefore, it may be <c>null</c> for MTP.</param>
        /// <param name="draftParams">The context parameters for the draft model. In MTP mode, ensure the <c>ContextType</c> property is explicitly set to <c>LLamaContextType.Mtp</c>.</param>
        /// <param name="draftTokens">The budget of draft tokens to propose per burst. Keep this modest (e.g., 2-4 for Dual-Model) or match the exact number of projection heads for MTP models to prevent wasted compute.</param>
        /// <param name="useMtp">Set to <c>true</c> to enable Multi-Token Prediction (Self-Speculation) for supported models (e.g., DeepSeek-R1, Qwen3.5-MTP). Requires <c>LoadMtp = true</c> in the target model parameters.</param>
        /// <param name="logger">An optional logger instance.</param>
        public StatelessExecutor(
            LLamaWeights weights,
            IContextParams @params,
            LLamaWeights? draftWeights = null,
            IContextParams? draftParams = null,
            int draftTokens = 0,
            bool useMtp = false,
            ILogger? logger = null)
        {
            Embeds = [];
            _weights = weights;
            _params = @params;
            _draftWeights = draftWeights;
            _draftParams = draftParams ?? @params;
            _draftTokens = draftTokens;
            _useMtp = useMtp;
            _logger = logger;
            _batch = new LLamaBatch();

            Context = _weights.CreateContext(_params, logger);
            Context.Dispose();
        }


        /// <inheritdoc />
        public async IAsyncEnumerable<string> InferAsync(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Ensure the context from last time is disposed (it always should be)
            if (!Context.NativeHandle.IsClosed)
                Context.Dispose();

            // Create an inference context which will be disposed when this method exits
            using var context = _weights.CreateContext(_params, _logger);
            Context = context;

            #region speculative
            if (_draftTokens > 0 && _useMtp && _draftParams != null)
            {
                // Inject the target context's memory pointer into the draft context!
                // This allows the native MTP projection head to read the target's hidden states.
                _draftParams.CtxOther = Context.NativeHandle.DangerousGetHandle();
            }

            // Allow separate draft models to be used alongside MTP routing for models like Gemma 4
            LLamaWeights activeDraftWeights = _draftWeights ?? _weights;

            using var draftContext = (_draftTokens > 0)
                ? activeDraftWeights.CreateContext(_draftParams!, _logger)
                : null;

            using var specDecoder = (draftContext != null)
                ? new LLama.Speculative.SpeculativeDecoder(Context.NativeHandle, draftContext.NativeHandle, _draftTokens, _useMtp)
                : null;
            #endregion

            // Reset the sampling pipeline (if there is one)
            inferenceParams?.SamplingPipeline.Reset();

            // Sanity check inference params
            inferenceParams ??= new InferenceParams();
            if (inferenceParams.TokensKeep > Context.ContextSize)
                throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({Context.ContextSize})");

            // Create decoders for the token stream
            var decoder = new StreamingTokenDecoder(Context)
            {
                DecodeSpecialTokens = inferenceParams.DecodeSpecialTokens,
            };
            var antiprocessor = new AntipromptProcessor(inferenceParams.AntiPrompts);

            if (ApplyTemplate)
            {
                var template = new LLamaTemplate(_weights.NativeHandle) { AddAssistant = true };
                if (SystemMessage != null) template.Add("system", SystemMessage);

                template.Add("user", prompt);
                prompt = PromptTemplateTransformer.ToModelPrompt(template);
            }
            
            // Tokenize the prompt
            var tokens = Context.Tokenize(prompt, special: true).ToList();

            // Capture the initial prompt length
            var initialPromptLength = tokens.Count;

            // We must track the history of all tokens in this session in case we need to re-prefill the context
            var all_tokens = new List<LLamaToken>(tokens);

            // Evaluate the prompt, in chunks smaller than the max batch size
            var n_past = 0;
            var (r, _, past) = await Context.DecodeAsync(tokens, LLamaSeqId.Zero, _batch, n_past);
            n_past = past;

            if (r != DecodeResult.Ok)
                throw new LLamaDecodeError(r);

            // Sync the Draft Model's KV cache with the prompt so it aligns with the Target Model
            if (draftContext != null && !_useMtp)
            {
                var draftBatch = new LLamaBatch();
                await draftContext.DecodeAsync(tokens, LLamaSeqId.Zero, draftBatch, 0);
            }

            // Begin loop, evaluating one token at a time
            var maxTokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;

            #region speculative
            int generatedCount = 0;
            if (specDecoder != null)
            {
                // Kickstart the sequence by manually sampling the first token from the prompt's logits
                var id = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, _batch.TokenCount - 1);
                decoder.Add(id);
                var decodedStr = decoder.Read();
                yield return decodedStr;
                generatedCount++;
                all_tokens.Add(id);

                _batch.Clear();
                _batch.Add(id, n_past++, LLamaSeqId.Zero, true);

                TotalDraftTokensProposed = 0;
                TotalDraftTokensAccepted = 0;

                while (generatedCount < maxTokens && !cancellationToken.IsCancellationRequested)
                {
                    // Boundary check
                    if (n_past >= Context.ContextSize)
                    {
                        if (inferenceParams.OverflowStrategy == ContextOverflowStrategy.ThrowException) throw new ContextOverflowException();
                        _logger?.LogWarning("Context size reached during speculative decoding. Stopping generation.");
                        break;
                    }

                    var results = specDecoder.Decode(_batch);
                    _batch.Clear();

                    int rawAcceptedCount = (results.Length > 0) ? results[0].count : 0;

                    // The native engine returns (Drafts Accepted + 1 Target Token). 
                    // We subtract 1 to get the true drafts, and clamp it between 0 and our draft budget.
                    int trueDraftsAccepted = 0;
                    if (rawAcceptedCount > 0)
                    {
                        trueDraftsAccepted = Math.Min(_draftTokens, Math.Max(0, rawAcceptedCount - 1));
                    }

                    // Track metrics
                    TotalDraftTokensProposed += _draftTokens;
                    TotalDraftTokensAccepted += trueDraftsAccepted;

                    // Keep using rawAcceptedCount for the loop generation logic below!
                    int acceptedCount = rawAcceptedCount;

                    if (acceptedCount > 0)
                    {
                        var acceptedTokens = results[0].tokens.Take(acceptedCount).ToArray();
                        bool shouldStop = false;

                        foreach (var rawToken in acceptedTokens)
                        {
                            var token = (LLamaToken)rawToken;
                            if (token.IsEndOfGeneration(_weights.Vocab))
                            {
                                shouldStop = true;
                                break;
                            }

                            decoder.Add(token);
                            var decoded = decoder.Read();
                            yield return decoded;
                            generatedCount++;

                            // Keep our context-shifting tracker accurate!
                            all_tokens.Add(token);

                            if (antiprocessor.Add(decoded))
                            {
                                shouldStop = true;
                                break;
                            }
                        }

                        if (shouldStop || generatedCount >= maxTokens) break;

                        // Advance C# tracking by the exact number of accepted drafts
                        n_past += acceptedCount;

                        // Feed the LAST accepted token back at its native position to match the C++ API logic
                        _batch.Add((LLamaToken)acceptedTokens[^1], n_past - 1, LLamaSeqId.Zero, true);
                    }
                    else
                    {
                        // 0 Drafts accepted. Sample the next token manually using the updated native logits (-1)
                        var nextId = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, -1);

                        if (nextId.IsEndOfGeneration(_weights.Vocab)) break;

                        decoder.Add(nextId);
                        var dec = decoder.Read();
                        yield return dec;
                        generatedCount++;
                        all_tokens.Add(nextId);

                        if (antiprocessor.Add(dec)) break;

                        _batch.Add(nextId, n_past++, LLamaSeqId.Zero, true);
                    }
                }
                yield break;
            }
            #endregion

            for (var i = 0; i < maxTokens && !cancellationToken.IsCancellationRequested; i++)
            {
                // Sample with the pipeline
                var id = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, _batch.TokenCount - 1);

                // Check if this token should end generation
                if (id.IsEndOfGeneration(_weights.Vocab))
                    break;

                // Decode this token into text
                decoder.Add(id);
                var decoded = decoder.Read();
                yield return decoded;

                // Check if any of the antiprompts have been generated
                if (antiprocessor.Add(decoded))
                    break;

                tokens.Clear();
                tokens.Add(id);

                // when run out of context
                if (n_past + tokens.Count >= Context.ContextSize)
                {
                    if (inferenceParams.OverflowStrategy == ContextOverflowStrategy.ThrowException)
                    {
                        throw new ContextOverflowException();
                    }

                    var canAddBos = Context.Vocab.ShouldAddBOS;
                    var tokensKeep = inferenceParams.TokensKeep;

                    // number of tokens to keep when resetting context
                    if (tokensKeep < 0 || tokensKeep > initialPromptLength)
                    {
                        tokensKeep = initialPromptLength;
                    }
                    else
                    {
                        tokensKeep += Convert.ToInt32(canAddBos);
                    }

                    var n_left = n_past - tokensKeep;

                    if (n_left <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(inferenceParams), "Cannot truncate context: TokensKeep exceeds or equals the current context size.");
                    }

                    // Safely calculate discard amount using our configured percentage
                    var percentage = Math.Max(0.01f, Math.Min(0.99f, inferenceParams.ContextTruncationPercentage));
                    var n_discard = (int)(n_left * percentage);

                    // Clamp between 1 and n_left
                    n_discard = Math.Max(1, Math.Min(n_discard, n_left));

                    if (Context.NativeHandle.MemoryCanShift)
                    {
                        // Fast path: Attempt the fast native memory shift (works for standard models like Llama 2/3)
                        Context.NativeHandle.MemorySequenceRemove(LLamaSeqId.Zero, tokensKeep, tokensKeep + n_discard);
                        Context.NativeHandle.MemorySequenceAdd(LLamaSeqId.Zero, tokensKeep + n_discard, n_past, -n_discard);
                        n_past -= n_discard;
                        all_tokens.RemoveRange(tokensKeep, n_discard);
                    }
                    else
                    {
                        // Fallback: The model does not support native shifting (e.g., 2D RoPE models).
                        // We must clear the cache and perform a full context re-prefill.
                        _logger?.LogInformation("Model does not support native memory shifting. Falling back to context re-prefill.");

                        all_tokens.RemoveRange(tokensKeep, n_discard);

                        _batch.Clear();
                        Context.NativeHandle.MemoryClear();

                        var (rReprefill, _, pastReprefill) = await Context.DecodeAsync(all_tokens, LLamaSeqId.Zero, _batch, 0);
                        if (rReprefill != DecodeResult.Ok)
                            throw new LLamaDecodeError(rReprefill);

                        n_past = pastReprefill;
                    }
                }

                // Add the new token to our historical tracker
                all_tokens.Add(id);

                // Evaluate with this new token
                _batch.Clear();
                _batch.Add(id, n_past++, LLamaSeqId.Zero, true);
                var returnCode = await context.DecodeAsync(_batch, cancellationToken);
                if (returnCode != 0)
                    throw new LLamaDecodeError(returnCode);
            }
        }
    }
}
