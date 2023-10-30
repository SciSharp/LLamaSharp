using LLama;
using LLama.Common;
using Microsoft.SemanticMemory.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    public class LlamaSharpTextGeneration : ITextGeneration, IDisposable
    {
        private readonly LlamaSharpConfig _config;
        private readonly LLamaWeights _weights;
        private readonly InstructExecutor _executor;
        private readonly LLamaContext _context;

        public LlamaSharpTextGeneration(LlamaSharpConfig config)
        {
            this._config = config;
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config?.ContextSize ?? 1024,
                Seed = config?.Seed ?? 0,
                GpuLayerCount = config?.GpuLayerCount ?? 20
            };
            _weights = LLamaWeights.LoadFromFile(parameters);
            _context = _weights.CreateContext(parameters);
            _executor = new InstructExecutor(_context);

        }

        public void Dispose()
        {
            _context.Dispose();
            _weights.Dispose();
        }

        public IAsyncEnumerable<string> GenerateTextAsync(string prompt, TextGenerationOptions options, CancellationToken cancellationToken = default)
        {
            return _executor.InferAsync(prompt, OptionsToParams(options), cancellationToken: cancellationToken);
        }

        private static InferenceParams OptionsToParams(TextGenerationOptions options)
        {
            return new InferenceParams()
            {
                AntiPrompts = options.StopSequences,
                Temperature = (float)options.Temperature,
                MaxTokens = options.MaxTokens ?? 1024,
                FrequencyPenalty = (float)options.FrequencyPenalty,
                PresencePenalty = (float)options.PresencePenalty,
                TopP = (float)options.TopP,
            };
        }
    }
}
