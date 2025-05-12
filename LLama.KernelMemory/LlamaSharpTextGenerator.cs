using LLama;
using LLama.Common;
using LLama.Sampling;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides text generation for LLamaSharp.
    /// </summary>
    public sealed class LlamaSharpTextGenerator
        : ITextGenerator, IDisposable
    {
        private readonly StatelessExecutor _executor;

        private readonly LLamaWeights _weights;
        private readonly bool _ownsWeights;

        private readonly InferenceParams? _defaultInferenceParams;

        private readonly ModelParams? @params;

        public int MaxTokenTotal { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGenerator"/> class.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        public LlamaSharpTextGenerator(LLamaSharpConfig config)
        {
            @params = new ModelParams(config.ModelPath)
            {
                ContextSize = config?.ContextSize ?? 2048,
                GpuLayerCount = config?.GpuLayerCount ?? 20,
                MainGpu = config?.MainGpu ?? 0,
                SplitMode = config?.SplitMode ?? LLama.Native.GPUSplitMode.Layer,
                BatchSize = 512,
                UBatchSize = 512,
                FlashAttention = true,
                UseMemorymap = true
            };
            _weights = LLamaWeights.LoadFromFile(@params);
            _executor = new StatelessExecutor(_weights, @params);
            _defaultInferenceParams = config!.DefaultInferenceParams;
            _ownsWeights = true;
            MaxTokenTotal = (int)@params.ContextSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGenerator"/> class from reused weights, context and executor.
        /// If executor is not specified, then a StatelessExecutor will be created with `context.Params`. So far only `StatelessExecutor` is expected.
        /// </summary>
        /// <param name="weights">A LLamaWeights object.</param>
        /// <param name="executor">An executor. Currently only StatelessExecutor is expected.</param>
        public LlamaSharpTextGenerator(LLamaWeights weights, LLamaSharpConfig config, StatelessExecutor? executor = null)
        {
            InferenceParams? inferenceParams = config.DefaultInferenceParams;
            _weights = weights;
            @params = new ModelParams("")
            {
                ContextSize = config?.ContextSize ?? 2048,
                GpuLayerCount = config?.GpuLayerCount ?? 20,
                MainGpu = config?.MainGpu ?? 0,
                SplitMode = config?.SplitMode ?? LLama.Native.GPUSplitMode.Layer,
                BatchSize = 512,
                UBatchSize = 512,
                FlashAttention = true,
                UseMemorymap = true
            };
            _executor = executor ?? new StatelessExecutor(_weights, @params);
            _defaultInferenceParams = inferenceParams;
            MaxTokenTotal = (int)@params.ContextSize;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_ownsWeights)
            {
                _weights.Dispose();
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<GeneratedTextContent> GenerateTextAsync(string prompt, TextGenerationOptions options, CancellationToken cancellationToken = default)
        {
            return _executor
                  .InferAsync(prompt, OptionsToParams(options, _defaultInferenceParams), cancellationToken: cancellationToken)
                  .Select(a => new GeneratedTextContent(a));
        }

        private static InferenceParams OptionsToParams(TextGenerationOptions options, InferenceParams? defaultParams)
        {
            if (defaultParams != null)
            {
                return defaultParams with
                {
                    AntiPrompts = defaultParams.AntiPrompts.Concat(options.StopSequences).ToList().AsReadOnly(),
                    MaxTokens = options.MaxTokens ?? defaultParams.MaxTokens,

                    SamplingPipeline = new DefaultSamplingPipeline()
                    {
                        Temperature = (float)options.Temperature,
                        FrequencyPenalty = (float)options.FrequencyPenalty,
                        PresencePenalty = (float)options.PresencePenalty,
                        TopP = (float)options.NucleusSampling,
                    }
                };
            }

            return new InferenceParams
            {
                AntiPrompts = options.StopSequences.ToList().AsReadOnly(),
                MaxTokens = options.MaxTokens ?? 1024,
                    
                SamplingPipeline = new DefaultSamplingPipeline()
                {
                    Temperature = (float)options.Temperature,
                    FrequencyPenalty = (float)options.FrequencyPenalty,
                    PresencePenalty = (float)options.PresencePenalty,
                    TopP = (float)options.NucleusSampling,
                }
            };
        }

        /// <summary>
        /// Count tokens in the input text
        /// </summary>
        /// <param name="text">input text</param>
        /// <returns></returns>
        public int CountTokens(string text) => _weights?.CountTokens(text, @params!) ?? 0;

        /// <summary>
        /// Get the list of tokens for the input text
        /// </summary>
        /// <param name="text">Input string to be tokenized</param>
        /// <returns>Read-only list of tokens for the input test</returns>
        /// <remarks>
        /// It throws if text is null and Includes empty stop token because addBos is left true to be consistent with the CountTokens implementation.</remarks>
        /// <see cref="CountTokens(string)"/>
        public IReadOnlyList<string> GetTokens(string text) => _weights?.GetTokens(text, @params!) ?? new List<string>();
    }
}
