using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private LLamaContext _context;
        private LLamaContext.State _originalState;
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
            _context.NativeHandle.Eval(tokens.AsMemory(0, tokens.Length), 0, _context.Params.Threads);
            _originalState = context.GetState();
        }

        /// <inheritdoc />
        public IEnumerable<string> Infer(string text, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int n_past = 1;
            if(inferenceParams is null)
            {
                inferenceParams = new InferenceParams();
            }
            List<llama_token> lastTokens = new(inferenceParams.RepeatLastTokensCount);
            for(int i = 0; i < lastTokens.Count; i++)
            {
                lastTokens[i] = 0;
            }
            List<llama_token> tokens = _context.Tokenize(text, true).ToList();
            int n_prompt_tokens = tokens.Count;

            _context.NativeHandle.Eval(tokens.ToArray().AsMemory(0, n_prompt_tokens), n_past, _context.Params.Threads);

            lastTokens.AddRange(tokens);
            n_past += n_prompt_tokens;

            var mu = (float?)null;
            int max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(int i = 0; i < max_tokens; i++)
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

                string response = _context.NativeHandle.TokenToString(id, _context.Encoding);
                yield return response;

                tokens.Clear();
                tokens.Add(id);

                if (inferenceParams.AntiPrompts is not null && inferenceParams.AntiPrompts.Count() > 0)
                {
                    string last_output = "";
                    foreach (var token in lastTokens)
                    {
                        last_output += _context.NativeHandle.TokenToString(token, _context.Encoding);
                    }

                    bool should_break = false;
                    foreach (var antiprompt in inferenceParams.AntiPrompts)
                    {
                        if (last_output.EndsWith(antiprompt))
                        {
                            should_break = true;
                            break;
                        }
                    }
                    if (should_break)
                    {
                        break;
                    }
                }

                // when run out of context
                if (n_past + tokens.Count > _context.ContextSize)
                {
                    int n_left = n_past - inferenceParams.TokensKeep;

                    n_past = Math.Max(1, inferenceParams.TokensKeep);

                    // insert n_left/2 tokens at the start of embed from last_n_tokens
                    tokens.InsertRange(0, lastTokens.Take(lastTokens.Count - tokens.Count).Skip(_context.ContextSize - n_left / 2 - tokens.Count));
                }

                n_past = _context.Eval(tokens.ToArray(), n_past);
            }

            _context.LoadState(_originalState);
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
