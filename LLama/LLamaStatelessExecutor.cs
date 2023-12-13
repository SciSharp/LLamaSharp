using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LLama.Native;
using LLama.Sampling;
using LLama.Control;
using Microsoft.Extensions.Logging;

namespace LLama
{
    using llama_token = Int32;

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

            await foreach(var item in InferAsync(prompt, Context, inferenceParams, cancellationToken))
            {
                yield return item;
            }
        }

        public static async IAsyncEnumerable<string> InferAsync(string prompt, LLamaContext context, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {

            // Sanity check inference params
            inferenceParams ??= new InferenceParams();
            if (inferenceParams.TokensKeep > context.ContextSize)
                throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({context.ContextSize})");

            // Keep track of the last N tokens emitted
            var repeat_last_n = Math.Max(0, inferenceParams.RepeatLastTokensCount < 0 ? context.ContextSize : inferenceParams.RepeatLastTokensCount);
            var lastTokens = new List<llama_token>(repeat_last_n);
            for (var i = 0; i < repeat_last_n; i++)
                lastTokens.Add(0);

            // Tokenize the prompt
            var tokens = inferenceParams.Tokenizer.Tokenize(context, prompt).ToList();
            lastTokens.AddRange(tokens);
            var n_past = 1 + tokens.Count;

            // Evaluate the prompt
            await Task.Run(() => { context.Eval(tokens, 1); }, cancellationToken)
                      .ConfigureAwait(false);

            // Begin loop, evaluating one token at a time
            var mu = (float?)null;
            var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for (var i = 0; i < max_tokens && !cancellationToken.IsCancellationRequested; i++)
            {
                llama_token id;
                if (inferenceParams.SamplingPipeline is not null)
                {
                    id = inferenceParams.SamplingPipeline.Sample(context.NativeHandle, context.NativeHandle.GetLogits(), lastTokens);
                }
                else
                {
                    // Penalize the generated tokens by various penalties
                    var tokenDataArray = context.ApplyPenalty(lastTokens, inferenceParams.LogitBias, repeat_last_n,
                        inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                    // Sample a single token
                    id = context.Sample(
                        tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                        inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP, inferenceParams.Grammar,
                        inferenceParams.MinP
                    );
                }

                // Decode this token into text
                var decoded = inferenceParams.Tokenizer.Detokenize(context, id);
                yield return decoded;

                // Check if the generation should stop
                if (inferenceParams.GenerationControl.ShouldStopGeneration(context, inferenceParams, decoded))
                    break;

                lastTokens.Add(id);
                tokens.Clear();
                tokens.Add(id);

                // when run out of context
                // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L497
                if (n_past + tokens.Count >= context.ContextSize)
                {
                    var n_left = n_past - inferenceParams.TokensKeep - 1;
                    var n_discard = n_left / 2;

                    NativeApi.llama_kv_cache_seq_rm(context.NativeHandle, (LLamaSeqId)0, inferenceParams.TokensKeep + 1, inferenceParams.TokensKeep + n_discard + 1);
                    NativeApi.llama_kv_cache_seq_shift(context.NativeHandle, (LLamaSeqId)0, inferenceParams.TokensKeep + 1 + n_discard, n_past, -n_discard);

                    n_past -= n_discard;
                }

                // ReSharper disable once AccessToModifiedClosure (Justification: n_past is modified inside and outside the capture, but not concurrently)
                n_past = await Task.Run(() => context.Eval(tokens, n_past), cancellationToken)
                                   .ConfigureAwait(false);
            }
        }
    }
}
