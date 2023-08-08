using System;
using System.Text;
using LLama.Abstractions;
using LLama.Extensions;
using LLama.Native;

namespace LLama
{
    /// <summary>
    /// A set of model weights, loaded into memory.
    /// </summary>
    public class LLamaWeights
        : IDisposable
    {
        private readonly SafeLlamaModelHandle _weights;

        /// <summary>
        /// The native handle, which is used in the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLlamaModelHandle NativeHandle => _weights;

        private LLamaWeights(SafeLlamaModelHandle weights)
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
            using var pin = @params.ToLlamaContextParams(out var lparams);
            var weights = SafeLlamaModelHandle.LoadFromFile(@params.ModelPath, lparams);

            if (!string.IsNullOrEmpty(@params.LoraAdapter))
                weights.ApplyLoraFromFile(@params.LoraAdapter, @params.LoraBase, @params.Threads);

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
        /// <param name="encoding"></param>
        /// <returns></returns>
        public LLamaContext CreateContext(IModelParams @params, Encoding encoding)
        {
            return new LLamaContext(this, @params, encoding);
        }
    }
}
