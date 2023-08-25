﻿using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

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
        private readonly IModelParams _params;

        /// <summary>
        /// The context used by the executor when running the inference.
        /// </summary>
        public LLamaContext Context { get; private set; }

        /// <summary>
        /// Create a new stateless executor which will use the given model
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        public StatelessExecutor(LLamaWeights weights, IModelParams @params)
        {
            _weights = weights;
            _params = @params;

            Context = _weights.CreateContext(_params);
            Context.Dispose();
        }

        /// <summary>
        /// Create a new stateless executor which will use the model used to create the given context
        /// </summary>
        /// <param name="context"></param>
        [Obsolete("Use the constructor which automatically creates contexts using the LLamaWeights")]
        public StatelessExecutor(LLamaContext context)
        {
            _weights = new LLamaWeights(context.NativeHandle.ModelHandle, context.Params.Encoding);
            _params = context.Params;

            Context = _weights.CreateContext(_params);
            Context.Dispose();
        }

        /// <inheritdoc />
        public IEnumerable<string> Infer(string text, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
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
            var n_past = 1;
            inferenceParams ??= new InferenceParams();

            var lastTokens = new List<llama_token>(inferenceParams.RepeatLastTokensCount);
            for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                lastTokens.Add(0);

            var tokens = Context.Tokenize(text).ToList();
            var n_prompt_tokens = tokens.Count;

            Context.Eval(tokens, n_past);

            lastTokens.AddRange(tokens);
            n_past += n_prompt_tokens;

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

                var response = Context.TokenToString(id);
                yield return response;

                tokens.Clear();
                tokens.Add(id);

                if (EndsWithAntiprompt(lastTokens, antiprompts))
                    break;

                // when run out of context
                // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L433
                if (n_past + tokens.Count > Context.ContextSize)
                {
                    var n_left = n_past - inferenceParams.TokensKeep;

                    n_past = Math.Max(1, inferenceParams.TokensKeep);

                    tokens.Clear();
                    tokens.AddRange(lastTokens.Skip(lastTokens.Count - n_left / 2).Take(n_left / 2));
                }

                n_past = Context.Eval(tokens, n_past);
            }
        }

        /// <summary>
        /// Check if the given tokens list ends with any of the antiprompts
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="antiprompts"></param>
        /// <returns></returns>
        private bool EndsWithAntiprompt(IReadOnlyList<llama_token> tokens, IReadOnlyList<string> antiprompts)
        {
            if (antiprompts.Count == 0 || tokens.Count == 0)
                return false;

            var builder = new StringBuilder();
            foreach (var token in tokens)
                builder.Append(Context.TokenToString(token));

            var last_output = builder.ToString();

            foreach (var antiprompt in antiprompts)
            {
                if (last_output.EndsWith(antiprompt))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var result in Infer(text, inferenceParams, cancellationToken))
            {
                yield return result;
            }
        }
    }
}
