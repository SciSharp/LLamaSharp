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
        /// Encoding to use to convert text into bytes for the model
        /// </summary>
        public Encoding Encoding { get; }

        internal LLamaWeights(SafeLlamaModelHandle weights, Encoding encoding)
        {
            _weights = weights;
            Encoding = encoding;
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

            return new LLamaWeights(weights, Encoding.GetEncoding(@params.Encoding));
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
        public LLamaContext CreateContext(IModelParams @params)
        {
            return new LLamaContext(this, @params);
        }
    }
}
