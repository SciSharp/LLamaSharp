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
        private LLamaModelContext _model;
        private LLamaModelContext.State _originalState;
        /// <summary>
        /// The mode used by the executor when running the inference.
        /// </summary>
        public LLamaModelContext Context => _model;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The LLama model.</param>
        public StatelessExecutor(LLamaModelContext model)
        {
            _model = model;
            
            var tokens = model.Tokenize(" ", true).ToArray();
            Utils.Eval(_model.NativeHandle, tokens, 0, tokens.Length, 0, _model.Params.Threads);
            _originalState = model.GetState();
        }

        /// <inheritdoc />
        public IEnumerable<string> Infer(string text, InferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
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
            List<llama_token> tokens = _model.Tokenize(text, true).ToList();
            int n_prompt_tokens = tokens.Count;

            Utils.Eval(_model.NativeHandle, tokens.ToArray(), 0, n_prompt_tokens, n_past, _model.Params.Threads);

            lastTokens.AddRange(tokens);
            n_past += n_prompt_tokens;

            int max_tokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(int i = 0; i < max_tokens; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _model.LoadState(_originalState);
                    break;
                }
                var repeat_last_n = inferenceParams.RepeatLastTokensCount < 0 ? _model.ContextSize : inferenceParams.RepeatLastTokensCount;

                var tokenDataArray = _model.ApplyPenalty(lastTokens, inferenceParams.LogitBias, repeat_last_n,
                    inferenceParams.RepeatPenalty, inferenceParams.FrequencyPenalty, inferenceParams.PresencePenalty, inferenceParams.PenalizeNL);

                var id = _model.Sample(tokenDataArray, inferenceParams.Temperature, inferenceParams.Mirostat, inferenceParams.MirostatTau,
                    inferenceParams.MirostatEta, inferenceParams.TopK, inferenceParams.TopP, inferenceParams.TfsZ, inferenceParams.TypicalP);

                lastTokens.Add(id);

                string response = Utils.TokenToString(id, _model.NativeHandle, _model.Encoding);
                yield return response;

                tokens.Clear();
                tokens.Add(id);

                if (inferenceParams.AntiPrompts is not null && inferenceParams.AntiPrompts.Count() > 0)
                {
                    string last_output = "";
                    foreach (var token in lastTokens)
                    {
                        last_output += Utils.PtrToString(NativeApi.llama_token_to_str(_model.NativeHandle, id), _model.Encoding);
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
                if (n_past + tokens.Count > _model.ContextSize)
                {
                    int n_left = n_past - inferenceParams.TokensKeep;

                    n_past = Math.Max(1, inferenceParams.TokensKeep);

                    // insert n_left/2 tokens at the start of embed from last_n_tokens
                    tokens.InsertRange(0, lastTokens.Take(lastTokens.Count - tokens.Count).Skip(_model.ContextSize - n_left / 2 - tokens.Count));
                }

                n_past = _model.Eval(tokens.ToArray(), n_past);
            }

            _model.LoadState(_originalState);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<string> InferAsync(string text, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var result in Infer(text, inferenceParams, cancellationToken))
            {
                yield return result;
            }
        }
    }
}
