using System;
using System.Collections.Generic;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

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
        public readonly Memory<LLamaTokenData> Data;

        /// <summary>
        /// Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.
        /// </summary>
        public bool Sorted;

        /// <summary>
        /// Create a new LLamaTokenDataArray
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="isSorted"></param>
        public LLamaTokenDataArray(Memory<LLamaTokenData> tokens, bool isSorted = false)
        {
            Data = tokens;
            Sorted = isSorted;
        }

        /// <summary>
        /// Create a new LLamaTokenDataArray, copying the data from the given logits
        /// </summary>
        /// <param name="logits"></param>
        /// <returns></returns>
        public static LLamaTokenDataArray Create(ReadOnlySpan<float> logits)
        {
            return Create(logits, new LLamaTokenData[logits.Length]);
        }

        /// <summary>
        /// Create a new LLamaTokenDataArray, copying the data from the given logits into temporary memory.
        /// </summary>
        /// <remarks>The memory must not be modified while this <see cref="LLamaTokenDataArray"/> is in use.</remarks>
        /// <param name="logits"></param>
        /// <param name="buffer">Temporary memory which will be used to work on these logits. Must be at least as large as logits array</param>
        /// <returns></returns>
        public static LLamaTokenDataArray Create(ReadOnlySpan<float> logits, Memory<LLamaTokenData> buffer)
        {
            if (buffer.Length < logits.Length)
                throw new ArgumentException("temporary memory is shorter than logits span");

            // take a slice of the output buffer which is exactly the size we need.
            var candidates = buffer.Slice(0, logits.Length);
            var candidatesSpan = candidates.Span;

            for (var token = 0; token < logits.Length; token++)
                candidatesSpan[token] = new LLamaTokenData(token, logits[token], 0.0f);

            return new LLamaTokenDataArray(candidates);
        }

        /// <summary>
        /// Overwrite the logit values for all given tokens
        /// </summary>
        /// <param name="values">tuples of token and logit value to overwrite</param>
        public void OverwriteLogits(ReadOnlySpan<(LLamaToken token, float logit)> values)
        {
            if (values.Length == 0)
                return;

            var dataSpan = Data.Span;
            foreach (var (token, value) in values)
            {
                for (var i = 0; i < Data.Length; i++)
                {
                    if (dataSpan[i].ID == token)
                    {
                        dataSpan[i].Logit = value;
                        break;
                    }
                }
            }

            Sorted = false;
        }

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        public void Softmax()
        {
            var data = Data.Span;

            // Sort logits **descending**
            data.Sort(new LLamaTokenDataLogitComparerDescending());
            Sorted = true;

            // Calculate softmax. Using TensorPrimitives is very fast (it uses SIMD etc) and is
            // definitely correct! So just copy to a temp and use that.
            var tempLogits = ArrayPool<float>.Shared.Rent(data.Length);
            var tempLogitsSpan = tempLogits.AsSpan(0, data.Length);
            try
            {
                // Copy to temporary
                for (var i = 0; i < data.Length; i++)
                    tempLogitsSpan[i] = data[i].Logit;

                // Softmax
                TensorPrimitives.SoftMax(tempLogitsSpan, tempLogitsSpan);

                // Copy back
                for (var i = 0; i < data.Length; i++)
                    data[i].Probability = tempLogitsSpan[i];
            }
            finally
            {
                ArrayPool<float>.Shared.Return(tempLogits, true);
            }
        }

        private struct LLamaTokenDataLogitComparerDescending
            : IComparer<LLamaTokenData>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(LLamaTokenData x, LLamaTokenData y)
            {
                return y.Logit.CompareTo(x.Logit);
            }
        }
    }

    /// <summary>
        /// Contains a pointer to an array of LLamaTokenData which is pinned in memory.
        /// </summary>
        /// <remarks>C# equivalent of llama_token_data_array</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaTokenDataArrayNative
    {
        /// <summary>
        /// A pointer to an array of LlamaTokenData
        /// </summary>
        /// <remarks>Memory must be pinned in place for all the time this LLamaTokenDataArrayNative is in use (i.e. `fixed` or `.Pin()`)</remarks>
        private unsafe LLamaTokenData* _data;

        /// <summary>
        /// Number of LLamaTokenData in the array
        /// </summary>
        private ulong _size;

        /// <summary>
        /// The index in the array (i.e. not the token id)
        /// </summary>
        private long _selected;

        private sbyte _sorted;

        /// <summary>
        /// A pointer to an array of LlamaTokenData
        /// </summary>
        public Span<LLamaTokenData> Data
        {
            get
            {
                unsafe
                {
                    return new Span<LLamaTokenData>(_data, checked((int)Size));
                }
            }
        }
        
        /// <summary>
        /// Indicates if the items in the array are sorted, so the most likely token is first
        /// </summary>
        public bool Sorted
        {
            get => Convert.ToBoolean(_sorted);
            set => _sorted = Convert.ToSByte(value);
        }

        /// <summary>
        /// The index of the selected token (i.e. <b>not the token id</b>)
        /// </summary>
        public long Selected
        {
            get => _selected;
            set => _selected = value;
        }

        /// <summary>
        /// Number of LLamaTokenData in the array. Set this to shrink the array
        /// </summary>
        public ulong Size
        {
            get => _size;
            set
            {
                if (value > _size)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot set Size property to a larger value");
                _size = value;
            }
        }

        /// <summary>
        /// Create a new LLamaTokenDataArrayNative around the data in the LLamaTokenDataArray 
        /// </summary>
        /// <param name="array">Data source</param>
        /// <param name="native">Created native array</param>
        /// <returns>A memory handle, pinning the data in place until disposed</returns>
        public static MemoryHandle Create(LLamaTokenDataArray array, out LLamaTokenDataArrayNative native)
        {
            var handle = array.Data.Pin();

            unsafe
            {
                native = new LLamaTokenDataArrayNative
                {
                    _data = (LLamaTokenData*)handle.Pointer,
                    Size = (ulong)array.Data.Length,
                    Sorted = array.Sorted
                };
            }

            return handle;
        }
    }
}
