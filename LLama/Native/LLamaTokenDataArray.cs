using System;
using System.Buffers;
using System.Runtime.InteropServices;

using llama_token = System.Int32;

namespace LLama.Native
{
    /// <summary>
    /// Contains an array of LLamaTokenData, potentially sorted.
    /// </summary>
    public struct LLamaTokenDataArray
    {
        /// <summary>
        /// The LLamaTokenData
        /// </summary>
        public readonly Memory<LLamaTokenData> data;

        /// <summary>
        /// Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.
        /// </summary>
        public bool sorted;

        /// <summary>
        /// Create a new LLamaTokenDataArray
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="isSorted"></param>
        public LLamaTokenDataArray(Memory<LLamaTokenData> tokens, bool isSorted = false)
        {
            data = tokens;
            sorted = isSorted;
        }

        /// <summary>
        /// Create a new LLamaTokenDataArray, copying the data from the given logits
        /// </summary>
        /// <param name="logits"></param>
        /// <returns></returns>
        public static LLamaTokenDataArray Create(ReadOnlySpan<float> logits)
        {
            var candidates = new LLamaTokenData[logits.Length];
            for (var token_id = 0; token_id < logits.Length; token_id++)
                candidates[token_id] = new LLamaTokenData(token_id, logits[token_id], 0.0f);

            return new LLamaTokenDataArray(candidates);
        }
    }

    /// <summary>
    /// Contains a pointer to an array of LLamaTokenData which is pinned in memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaTokenDataArrayNative
    {
        /// <summary>
        /// A pointer to an array of LlamaTokenData
        /// </summary>
        /// <remarks>Memory must be pinned in place for all the time this LLamaTokenDataArrayNative is in use</remarks>
        public IntPtr data;

        /// <summary>
        /// Number of LLamaTokenData in the array
        /// </summary>
        public ulong size;

        /// <summary>
        /// Indicates if the items in the array are sorted
        /// </summary>
        public bool sorted
        {
            get => Convert.ToBoolean(_sorted);
            set => _sorted = Convert.ToSByte(value);
        }
        private sbyte _sorted;

        /// <summary>
        /// Create a new LLamaTokenDataArrayNative around the data in the LLamaTokenDataArray 
        /// </summary>
        /// <param name="array">Data source</param>
        /// <param name="native">Created native array</param>
        /// <returns>A memory handle, pinning the data in place until disposed</returns>
        public static MemoryHandle Create(LLamaTokenDataArray array, out LLamaTokenDataArrayNative native)
        {
            var handle = array.data.Pin();

            unsafe
            {
                native = new LLamaTokenDataArrayNative
                {
                    data = new IntPtr(handle.Pointer),
                    size = (ulong)array.data.Length,
                    sorted = array.sorted
                };
            }

            return handle;
        }

        /// <summary>
        /// Perform TopK sampling, sorting the data and reducing the size to k
        /// </summary>
        /// <param name="context"></param>
        /// <param name="k">Number of tokens to keep</param>
        /// <param name="minKeep">Minimum number to keep</param>
        public void TopK(SafeLLamaContextHandle context, int k, ulong minKeep = 1)
        {
            NativeApi.llama_sample_top_k(context, ref this, k, minKeep);
        }

        /// <summary>
        /// Perform top p sampling, sorting the data and keeping only logits more likely than p
        /// </summary>
        /// <param name="context"></param>
        /// <param name="p"></param>
        /// <param name="minKeep"></param>
        public void TopP(SafeLLamaContextHandle context, float p, ulong minKeep = 1)
        {
            NativeApi.llama_sample_top_p(context, ref this, p, minKeep);
        }

        /// <summary>
        /// Apply temperature to logits
        /// </summary>
        /// <param name="context"></param>
        /// <param name="temp"></param>
        public void Temperature(SafeLLamaContextHandle context, float temp)
        {
            NativeApi.llama_sample_temperature(context, ref this, temp);
        }

        /// <summary>
        /// Sample a token from the set of possible tokens
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int SampleToken(SafeLLamaContextHandle context)
        {
            return NativeApi.llama_sample_token(context, ref this);
        }
    }
}
