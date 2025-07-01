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

        /// <inheritdoc />
        public bool IsMultiModal => false;

        /// <inheritdoc />
        public LLavaWeights? ClipModel => default;

        /// <inheritdoc />
        public List<byte[]> Images { get; }

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
        /// Create a new stateless executor which will use the given model
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        public StatelessExecutor(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
        {
            Images = [ ];
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
            // Ensure the context from last time is disposed (it always should be)
            if (!Context.NativeHandle.IsClosed)
                Context.Dispose();

            // Create an inference context which will be disposed when this method exits
            using var context = _weights.CreateContext(_params, _logger);
            Context = context;

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

            // Evaluate the prompt, in chunks smaller than the max batch size
            var n_past = 0;
            var (r, _, past) = await Context.DecodeAsync(tokens, LLamaSeqId.Zero, _batch, n_past);
            n_past = past;

            if (r != DecodeResult.Ok)
                throw new LLamaDecodeError(r);

            // Begin loop, evaluating one token at a time
            var maxTokens = inferenceParams.MaxTokens < 0 ? int.MaxValue : inferenceParams.MaxTokens;
            for(var i = 0; i < maxTokens && !cancellationToken.IsCancellationRequested; i++)
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
                // based on this logic: https://github.com/ggerganov/llama.cpp/blob/master/examples/main/main.cpp#L497
                if (n_past + tokens.Count >= Context.ContextSize)
                {
                    var canAddBos = Context.Vocab.ShouldAddBOS;
                    var tokensKeep = inferenceParams.TokensKeep;

                    // number of tokens to keep when resetting context
                    // Ported from https://github.com/ggerganov/llama.cpp/blob/60325fa56f61c228464c9f065db3aa6a61f2156e/examples/main/main.cpp#L334
                    if (tokensKeep < 0 || tokensKeep > tokens.Count)
                    {
                        tokensKeep = tokens.Count;
                    }
                    else
                    {
                        tokensKeep += Convert.ToInt32(canAddBos);
                    }

                    var n_left = n_past - tokensKeep;
                    var n_discard = n_left / 2;

                    Context.NativeHandle.MemorySequenceRemove(LLamaSeqId.Zero, tokensKeep, tokensKeep + n_discard);
                    Context.NativeHandle.MemorySequenceAdd(LLamaSeqId.Zero, tokensKeep + n_discard, n_past, -n_discard);

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
