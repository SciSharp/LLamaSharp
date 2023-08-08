using System;
using System.Text;
using LLama.Common;
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
        public static LLamaWeights LoadFromFile(ModelParams @params)
        {
            using var pin = @params.ToLlamaContextParams(out var lparams);
            var weights = SafeLlamaModelHandle.LoadFromFile(@params.ModelPath, lparams);
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
        /// <param name="utf8"></param>
        /// <returns></returns>
        public LLamaContext CreateContext(ModelParams @params, Encoding utf8)
        {
            return new LLamaContext(this, @params, Encoding.UTF8);
        }
    }
}
