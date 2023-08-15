using System;
using System.Buffers;
using System.Runtime.InteropServices;

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
        /// Indicates if `data` is sorted
        /// </summary>
        public readonly bool sorted;

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
        public sbyte sorted;

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
                    sorted = Utils.BoolToSignedByte(array.sorted)
                };
            }

            return handle;
        }
    }
}
