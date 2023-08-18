using LLama.Abstractions;
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
    public class StatelessExecutor : ILLamaExecutor
    {
        private readonly LLamaContext _context;
        private readonly LLamaContext.State _originalState;

        /// <summary>
        /// The context used by the executor when running the inference.
        /// </summary>
        public LLamaContext Context => _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">The LLama model.</param>
        public StatelessExecutor(LLamaContext context)
        {
            _context = context;
            
            var tokens = context.Tokenize(" ", true).ToArray();
            _context.NativeHandle.Eval(tokens.AsSpan(0, tokens.Length), 0, _context.Params.Threads);
            _originalState = context.GetState();
        }

        /// <inheritdoc />
        public IEnumerable<string> Infer(string text, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var antiprompts = inferenceParams?.AntiPrompts.ToArray() ?? Array.Empty<string>();
            var n_past = 1;
            inferenceParams ??= new InferenceParams();

            var lastTokens = new List<llama_token>(inferenceParams.RepeatLastTokensCount);
            for (var i = 0; i < inferenceParams.RepeatLastTokensCount; i++)
                lastTokens.Add(0);

            var tokens = _context.Tokenize(text).ToList();
            var n_prompt_tokens = tokens.Count;

            _context.Eval(tokens, n_past);

            lastTokens.AddRange(tokens);
            n_past += n_prompt_tokens;

            var mu = (float?)null;
            var max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(var i = 0; i < max_tokens; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _context.LoadState(_originalState);
                    break;
                }
                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? _context.ContextSize : inferenceParams.RepeatLastTokensCount;

                var tokenDataArray = _context.ApplyPenalty(lastTokens, inferenceParams.LogitBias, repeat_last_n,
                    inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                var id = _context.Sample(tokenDataArray, ref mu, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP);

                lastTokens.Add(id);

                var response = _context.TokenToString(id);
                yield return response;

                tokens.Clear();
                tokens.Add(id);

                if (EndsWithAntiprompt(lastTokens, antiprompts))
                    break;

                // todo: this seems to be based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L433
                // todo: but it's broken!
                // when run out of context
                if (n_past + tokens.Count > _context.ContextSize)
                {
                    int n_left = n_past - inferenceParams.TokensKeep;

                    n_past = Math.Max(1, inferenceParams.TokensKeep);

                    // insert n_left/2 tokens at the start of embed from last_n_tokens
                    tokens.InsertRange(0, lastTokens.Take(lastTokens.Count - tokens.Count).Skip(_context.ContextSize - n_left / 2 - tokens.Count));
                }

                n_past = _context.Eval(tokens, n_past);
            }

            _context.LoadState(_originalState);
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
                builder.Append(_context.TokenToString(token));

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
