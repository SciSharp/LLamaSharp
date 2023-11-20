using LLama;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.KernelMemory.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides text generation for LLamaSharp.
    /// </summary>
    public class LlamaSharpTextGeneration : ITextGeneration, IDisposable
    {
        private readonly LLamaWeights _weights;
        private readonly StatelessExecutor _executor;
        private readonly LLamaContext _context;
        private readonly InferenceParams? _defaultInferenceParams;
        private bool _ownsContext = false;
        private bool _ownsWeights = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGeneration"/> class.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        public LlamaSharpTextGeneration(LLamaSharpConfig config)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config?.ContextSize ?? 2048,
                Seed = config?.Seed ?? 0,
                GpuLayerCount = config?.GpuLayerCount ?? 20
            };
            _weights = LLamaWeights.LoadFromFile(parameters);
            _context = _weights.CreateContext(parameters);
            _executor = new StatelessExecutor(_weights, parameters);
            _defaultInferenceParams = config?.DefaultInferenceParams;
            _ownsWeights = _ownsContext = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGeneration"/> class from reused weights, context and executor.
        /// If executor is not specified, then a StatelessExecutor will be created with `context.Params`. So far only `StatelessExecutor` is expected.
        /// </summary>
        /// <param name="weights">A LLamaWeights object.</param>
        /// <param name="context">A LLamaContext object.</param>
        /// <param name="executor">An executor. Currently only StatelessExecutor is expected.</param>
        public LlamaSharpTextGeneration(LLamaWeights weights, LLamaContext context, StatelessExecutor? executor = null, InferenceParams? inferenceParams = null)
        {
            _weights = weights;
            _context = context;
            _executor = executor ?? new StatelessExecutor(_weights, _context.Params);
            _defaultInferenceParams = inferenceParams;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_ownsWeights)
            {
                _weights?.Dispose();
            }
            if (_ownsContext)
            {
                _context.Dispose();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<string> GenerateTextAsync(string prompt, TextGenerationOptions options, CancellationToken cancellationToken = default)
        {
            return _executor.InferAsync(prompt, OptionsToParams(options, this._defaultInferenceParams), cancellationToken: cancellationToken);
        }

        private static InferenceParams OptionsToParams(TextGenerationOptions options, InferenceParams? defaultParams)
        {
            if (defaultParams != null)
            {
                return defaultParams with
                {
                    AntiPrompts = defaultParams.AntiPrompts.Concat(options.StopSequences).ToList().AsReadOnly(),
                    Temperature = options.Temperature == defaultParams.Temperature ? defaultParams.Temperature : (float)options.Temperature,
                    MaxTokens = options.MaxTokens ?? defaultParams.MaxTokens,
                    FrequencyPenalty = options.FrequencyPenalty == defaultParams.FrequencyPenalty ? defaultParams.FrequencyPenalty : (float)options.FrequencyPenalty,
                    PresencePenalty = options.PresencePenalty == defaultParams.PresencePenalty ? defaultParams.PresencePenalty : (float)options.PresencePenalty,
                    TopP = options.TopP == defaultParams.TopP ? defaultParams.TopP : (float)options.TopP
                };
            }
            else
            {
                return new InferenceParams()
                {
                    AntiPrompts = options.StopSequences.ToList().AsReadOnly(),
                    Temperature = (float)options.Temperature,
                    MaxTokens = options.MaxTokens ?? 1024,
                    FrequencyPenalty = (float)options.FrequencyPenalty,
                    PresencePenalty = (float)options.PresencePenalty,
                    TopP = (float)options.TopP,
                };
            }
        }
    }
}
