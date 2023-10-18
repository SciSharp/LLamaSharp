using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LLama.Extensions;
using LLama.Native;

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

        /// <summary>
        /// The context used by the executor when running the inference.
        /// </summary>
        public LLamaContext Context { get; private set; }

        /// <summary>
        /// Create a new stateless executor which will use the given model
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        public StatelessExecutor(LLamaWeights weights, IContextParams @params)
        {
            _weights = weights;
            _params = @params;

            Context = _weights.CreateContext(_params);
            Context.Dispose();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var context = _weights.CreateContext(_params);
            Context = context;

            if (!Context.NativeHandle.IsClosed)
                Context.Dispose();
            Context = _weights.CreateContext(Context.Params);

            if (inferenceParams != null)
            {
                if (inferenceParams.TokensKeep > Context.ContextSize)
                    throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({Context.ContextSize})");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var antiprompts = inferenceParams?.AntiPrompts.ToArray() ?? Array.Empty<string>();
            inferenceParams ??= new InferenceParams();

            var lastTokens = new List<llama_token>(inferenceParams.RepeatLastTokensCount);
            for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                lastTokens.Add(0);

            var tokens = Context.Tokenize(text).ToList();

            await Task.Run(() => { Context.Eval(tokens, 1); }, cancellationToken)
                      .ConfigureAwait(false);

            lastTokens.AddRange(tokens);
            var n_past = 1 + tokens.Count;

            var mu = (float?)null;
            var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(var i = 0; i < max_tokens; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? Context.ContextSize : inferenceParams.RepeatLastTokensCount;

                var tokenDataArray = Context.ApplyPenalty(lastTokens, inferenceParams.LogitBias, repeat_last_n,
                    inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                var id = Context.Sample(tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP, inferenceParams.Grammar);

                lastTokens.Add(id);
                yield return Context.TokenToString(id);

                tokens.Clear();
                tokens.Add(id);

                // Check if any of the antiprompts have been generated
                if (lastTokens.TokensEndsWithAnyString(antiprompts, Context))
                    break;

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

                // ReSharper disable once AccessToModifiedClosure (Justification: n_past is modified inside and outside the capture, but not concurrently)
                n_past = await Task.Run(() => Context.Eval(tokens, n_past), cancellationToken)
                                   .ConfigureAwait(false);
            }
        }
    }
}
