using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LLama.Exceptions;
using LLama.Native;
using LLama.Sampling;
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

        /// <summary>
        /// The context used by the executor when running the inference.
        /// </summary>
        public LLamaContext Context { get; private set; }

        /// <summary>
        /// Create a new stateless executor which will use the given model
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        public StatelessExecutor(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
        {
            _weights = weights;
            _params = @params;
            _logger = logger;
            _batch = new LLamaBatch();

            Context = _weights.CreateContext(_params, logger);
            Context.Dispose();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<string> InferAsync(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Ensure the context from last time is disposed (it always hould be)
            if (!Context.NativeHandle.IsClosed)
                Context.Dispose();

            // Create an inference context which will be disposed when this method exits
            using var context = _weights.CreateContext(_params, _logger);
            Context = context;

            // Reset the sampling pipeline (if there is one)
            inferenceParams?.SamplingPipeline?.Reset();

            // Sanity check inference params
            inferenceParams ??= new InferenceParams();
            if (inferenceParams.TokensKeep > Context.ContextSize)
                throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({Context.ContextSize})");

            // Create decoders for the token stream
            var decoder = new StreamingTokenDecoder(Context);
            var antiprocessor = new AntipromptProcessor(inferenceParams.AntiPrompts);

            // Keep track of the last N tokens emitted
            var repeat_last_n = Math.Max(0, inferenceParams.RepeatLastTokensCount <0 ? _weights.ContextSize : inferenceParams.RepeatLastTokensCount);
            var lastTokens = new List<LLamaToken>(repeat_last_n);
            for (var i = 0; i < repeat_last_n; i++)
                lastTokens.Add(0);

            // Tokenize the prompt
            var tokens = Context.Tokenize(prompt).ToList();
            lastTokens.AddRange(tokens);

            // Evaluate the prompt, in chunks smaller than the max batch size
            var n_past = 0;
            var (r, _) = Context.NativeHandle.Decode(tokens, LLamaSeqId.Zero, _batch, ref n_past);
            if (r != DecodeResult.Ok)
                throw new LLamaDecodeError(r);

            // Begin loop, evaluating one token at a time
            var mu = (float?)null;
            var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(var i = 0; i < max_tokens && !cancellationToken.IsCancellationRequested; i++)
            {
                LLamaToken id;
                if (inferenceParams.SamplingPipeline is not null)
                {
                    id = inferenceParams.SamplingPipeline.Sample(Context.NativeHandle, Context.NativeHandle.GetLogitsIth(_batch.TokenCount - 1), lastTokens);
                }
                else
                {
                    // Penalize the generated tokens by various penalties
                    var tokenDataArray = Context.ApplyPenalty(_batch.TokenCount - 1, lastTokens, inferenceParams.LogitBias, repeat_last_n,
                        inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                    // Sample a single token
                    id = Context.Sample(
                        tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                        inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP, inferenceParams.Grammar,
                        inferenceParams.MinP
                    );
                }

                // Check if this is the EOS token
                if (id == _weights.EndOfSentenceToken)
                    break;

                // Decode this token into text
                decoder.Add(id);
                var decoded = decoder.Read();
                yield return decoded;

                // Check if any of the antiprompts have been generated
                if (antiprocessor.Add(decoded))
                    break;

                lastTokens.Add(id);
                tokens.Clear();
                tokens.Add(id);

                // when run out of context
                // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L497
                if (n_past + tokens.Count >= Context.ContextSize)
                {
                    var n_left = n_past - inferenceParams.TokensKeep - 1;
                    var n_discard = n_left / 2;

                    NativeApi.llama_kv_cache_seq_rm(Context.NativeHandle, (LLamaSeqId)0, inferenceParams.TokensKeep + 1, inferenceParams.TokensKeep + n_discard + 1);
                    NativeApi.llama_kv_cache_seq_shift(Context.NativeHandle, (LLamaSeqId)0, inferenceParams.TokensKeep + 1 + n_discard, n_past, -n_discard);

                    n_past -= n_discard;
                }

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
