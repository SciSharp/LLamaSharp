using LLama;
using LLama.Common;
using LLama.Native;
using Microsoft.KernelMemory.AI;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides text generation for LLamaSharp.
    /// </summary>
    public class LlamaSharpTextGenerator : ITextGenerator, IDisposable
    {
        private readonly LLamaWeights _weights;
        private readonly StatelessExecutor _executor;
        private readonly LLamaContext _context;
        private readonly InferenceParams? _defaultInferenceParams;
        private bool _ownsContext = false;
        private bool _ownsWeights = false;

        public int MaxTokenTotal { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGenerator"/> class.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        public LlamaSharpTextGenerator(LLamaSharpConfig config)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config?.ContextSize ?? 2048,
                Seed = config?.Seed ?? 0,
                GpuLayerCount = config?.GpuLayerCount ?? 20,
                MainGpu = config?.MainGpu ?? 0,
                SplitMode = config?.SplitMode ?? GPUSplitMode.None
            };
            _weights = LLamaWeights.LoadFromFile(parameters);
            _context = _weights.CreateContext(parameters);
            _executor = new StatelessExecutor(_weights, parameters);
            _defaultInferenceParams = config?.DefaultInferenceParams;
            _ownsWeights = _ownsContext = true;
            MaxTokenTotal = (int)parameters.ContextSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LlamaSharpTextGenerator"/> class from reused weights, context and executor.
        /// If executor is not specified, then a StatelessExecutor will be created with `context.Params`. So far only `StatelessExecutor` is expected.
        /// </summary>
        /// <param name="weights">A LLamaWeights object.</param>
        /// <param name="context">A LLamaContext object.</param>
        /// <param name="executor">An executor. Currently only StatelessExecutor is expected.</param>
        public LlamaSharpTextGenerator(LLamaWeights weights, LLamaContext context, StatelessExecutor? executor = null, InferenceParams? inferenceParams = null)
        {
            _weights = weights;
            _context = context;
            _executor = executor ?? new StatelessExecutor(_weights, _context.Params);
            _defaultInferenceParams = inferenceParams;
            MaxTokenTotal = (int)_context.ContextSize;
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
                    TopP = options.NucleusSampling == defaultParams.TopP ? defaultParams.TopP : (float)options.NucleusSampling
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
                    TopP = (float)options.NucleusSampling,
                };
            }
        }

        /// <inheritdoc/>
        public int CountTokens(string text) => _context.Tokenize(text, special: true).Length;

        /// <summary>
        /// Get the list of tokens for the input text
        /// </summary>
        /// <param name="text">Input string to be tokenized</param>
        /// <returns>Read-only list of tokens for the input test</returns>
        /// <remarks>
        /// It throws if text is null and Includes empty stop token because addBos is left true to be consistent with the CountTokens implementation.</remarks>
        /// <see cref="CountTokens(string)"/>
        public IReadOnlyList<string> GetTokens(string text)
        {
            /* see relevant unit tests for important implementation notes regarding unicode */
            var numericTokens = _context.Tokenize(text, special: true);
            var decoder = new StreamingTokenDecoder(_context);
            return numericTokens
                .Select(x => { decoder.Add(x); return decoder.Read(); })
                .ToList();
        }
    }
}
