#nullable enable

using LLama.Common;
using LLama.Abstractions;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Web.Common
{
    public class InferenceOptions
        : IInferenceParams
    {
        /// <inheritdoc />
        public int TokensKeep { get; set; } = 0;

        /// <inheritdoc />
        public int MaxTokens { get; set; } = -1;

        /// <inheritdoc />
        public IReadOnlyList<string> AntiPrompts { get; set; } = Array.Empty<string>();

        /// <inheritdoc />
        public ISamplingPipeline SamplingPipeline { get; set; } = new DefaultSamplingPipeline();

        /// <inheritdoc />
        public bool DecodeSpecialTokens { get; set; }
    }
}
