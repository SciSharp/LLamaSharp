using System;
using System.Collections.Generic;
using LLama.Abstractions;
using LLama.Extensions;
using LLama.Native;
using Microsoft.Extensions.Logging;

namespace LLama
{
    /// <summary>
    /// A set of model weights, loaded into memory.
    /// </summary>
    public sealed class LLamaWeights
        : IDisposable
    {
        /// <summary>
        /// The native handle, which is used in the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLlamaModelHandle NativeHandle { get; }

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
        /// Get the newline token for this model
        /// </summary>
        public int NewlineToken => NativeApi.llama_token_nl(NativeHandle);

        /// <summary>
        /// Get the "end of sentence" token for this model
        /// </summary>
        public int EndOfSentenceToken => NativeApi.llama_token_eos(NativeHandle);

        /// <summary>
        /// Get the "beginning of sentence" token for this model
        /// </summary>
        public int BeginningOfSentenceToken => NativeApi.llama_token_bos(NativeHandle);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => NativeHandle.EmbeddingSize;

        /// <summary>
        /// All metadata keys in this model
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; }

        private LLamaWeights(SafeLlamaModelHandle weights)
        {
            NativeHandle = weights;
            Metadata = weights.ReadMetadata();
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

                weights.ApplyLoraFromFile(adapter.Path, adapter.Scale, @params.LoraBase);
            }

            return new LLamaWeights(weights);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            NativeHandle.Dispose();
        }

        /// <summary>
        /// Create a llama_context using this model
        /// </summary>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public LLamaContext CreateContext(IContextParams @params, ILogger? logger = null)
        {
            return new LLamaContext(this, @params, logger);
        }
    }
}
