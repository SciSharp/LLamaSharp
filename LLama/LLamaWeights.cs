using System;
using LLama.Abstractions;
using LLama.Extensions;
using LLama.Native;

namespace LLama
{
    /// <summary>
    /// A set of model weights, loaded into memory.
    /// </summary>
    public sealed class LLamaWeights
        : IDisposable
    {
        private readonly SafeLlamaModelHandle _weights;

        /// <summary>
        /// The native handle, which is used in the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLlamaModelHandle NativeHandle => _weights;

        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => NativeHandle.VocabCount;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize => NativeHandle.ContextSize;

        /// <summary>
        /// Get the size of this model in bytes
        /// </summary>
        public ulong SizeInBytes => NativeHandle.SizeInBytes;

        /// <summary>
        /// Get the number of parameters in this model
        /// </summary>
        public ulong ParameterCount => NativeHandle.ParameterCount;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => NativeHandle.EmbeddingSize;

        internal LLamaWeights(SafeLlamaModelHandle weights)
        {
            _weights = weights;
        }

        /// <summary>
        /// Load weights into memory
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        public static LLamaWeights LoadFromFile(IModelParams @params)
        {
            using var pin = @params.ToLlamaModelParams(out var lparams);
            var weights = SafeLlamaModelHandle.LoadFromFile(@params.ModelPath, lparams);

            foreach (var adapter in @params.LoraAdapters)
            {
                if (string.IsNullOrEmpty(adapter.Path))
                    continue;
                if (adapter.Scale <= 0)
                    continue;

                weights.ApplyLoraFromFile(adapter.Path, adapter.Scale, @params.LoraBase, @params.Threads);
            }

            return new LLamaWeights(weights);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _weights.Dispose();
        }

        /// <summary>
        /// Create a llama_context using this model
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        public LLamaContext CreateContext(IContextParams @params)
        {
            return new LLamaContext(this, @params);
        }
    }
}
