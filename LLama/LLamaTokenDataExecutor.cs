using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LLama
{
    internal class LLamaTokenDataExecutor : ILLamaExecutor
    {
        private readonly ILogger? _logger;
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
        /// <param name="logger"></param>
        public LLamaTokenDataExecutor(LLamaWeights weights, IContextParams @params)
        {
            _weights = weights;
            _params = @params;

            Context = _weights.CreateContext(_params);
            Context.Dispose();
        }


        public async IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams, CancellationToken token)
        {
           await foreach (var tokenData in InferTokensAsync(text, inferenceParams, token))
                yield return tokenData.Content;
        }

        public async IAsyncEnumerable<TokenData> InferTokensAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            using (Context = _weights.CreateContext(_params))
            {
                if (inferenceParams != null)
                {
                    if (inferenceParams.TokensKeep > Context.ContextSize)
                        throw new ArgumentOutOfRangeException(nameof(inferenceParams), $"TokensKeep ({inferenceParams.TokensKeep}) cannot be larger than ContextSize ({Context.ContextSize})");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var antiprompts = inferenceParams?.AntiPrompts.ToArray() ?? Array.Empty<string>();
                var n_past = 1;
                inferenceParams ??= new InferenceParams();

                var lastTokens = new List<TokenData>(inferenceParams.RepeatLastTokensCount);
                for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                    lastTokens.Add(new TokenData(0));

                var tokens = Context.Tokenize(text, true)
                    .Select(x => new TokenData(x) { Content = Context.TokenToString(x) })
                    .ToList();
                var n_prompt_tokens = tokens.Count;

                Context.Eval(tokens.ToTokenIds(), n_past);

                lastTokens.AddRange(tokens);
                n_past += n_prompt_tokens;

                var mu = (float?)null;
                var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
                for (var i = 0; i < max_tokens; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var tokenDataArray = Context.ApplyPenalty(lastTokens, inferenceParams);

                    var id = Context.Sample(tokenDataArray, inferenceParams, ref mu);

                    var tokenData = tokenDataArray.GetTokenData(Context, id);

                    lastTokens.Add(tokenData);

                    yield return tokenData;

                    tokens.Clear();
                    tokens.Add(tokenData);

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

                    n_past = Context.Eval(tokens.ToTokenIds(), n_past);
                }
            }
        }


        /// <summary>
        /// Check if the given tokens list ends with any of the antiprompts
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="antiprompts"></param>
        /// <returns></returns>
        private bool EndsWithAntiprompt(IReadOnlyList<TokenData> tokens, IReadOnlyList<string> antiprompts)
        {
            if (antiprompts.Count == 0 || tokens.Count == 0)
                return false;

            var builder = new StringBuilder();
            foreach (var token in tokens)
                builder.Append(token.Content);

            var last_output = builder.ToString();

            foreach (var antiprompt in antiprompts)
            {
                if (last_output.EndsWith(antiprompt))
                    return true;
            }

            return false;
        }

    }

    public static class LLamaTokenDataExecutorExt
    {
        public static int[] ToTokenIds(this IEnumerable<TokenData> tokens)
        {
            return tokens.Select(x => x.Id).ToArray();
        }

        public static TokenData GetTokenData(this LLamaTokenDataArray tokenDataArray, LLamaContext context, int id)
        {
            // TODO: are all samplers sorted? if not we need to do a binary serach using id
#if NET6_0_OR_GREATER
            var tokenDataSpan = tokenDataArray.data[..1].Span;
#else
            var tokenDataSpan = new Memory<LLamaTokenData>().Span; // TODO
#endif

            if (tokenDataSpan.Length == 0)
                throw new InvalidOperationException("The input sequence is empty.");

            var tokenData = tokenDataSpan[0];
            return new TokenData(tokenData.id)
            {
                Logit = tokenData.logit,
                Probability = tokenData.p,
                Content = context.TokenToString(tokenData.id)
            };
        }

        public static LLamaTokenDataArray ApplyPenalty(this LLamaContext context, IEnumerable<TokenData> lastTokens, IInferenceParams inferenceParams)
        {
            var repeatLastN = inferenceParams.RepeatLastTokensCount < 0
                ? context.ContextSize
                : inferenceParams.RepeatLastTokensCount;

            return context.ApplyPenalty
            (
                lastTokens.ToTokenIds(),
                inferenceParams.LogitBias,
                repeatLastN,
                inferenceParams.RepeatPenalty,
                inferenceParams.FrequencyPenalty,
                inferenceParams.PresencePenalty,
                inferenceParams.PenalizeNL
            );
        }

        public static int Sample(this LLamaContext context, LLamaTokenDataArray tokenDataArray, IInferenceParams inferenceParams, ref float? mirostatMu)
        {
            return context.Sample
            (
                tokenDataArray,
                ref mirostatMu,
                inferenceParams.Temperature,
                inferenceParams.Mirostat,
                inferenceParams.MirostatTau,
                inferenceParams.MirostatEta,
                inferenceParams.TopK,
                inferenceParams.TopP,
                inferenceParams.TfsZ,
                inferenceParams.TypicalP,
                inferenceParams.Grammar
            );
        }
    }
}
