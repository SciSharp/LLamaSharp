using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLama.Common
{
    /// <summary>
    /// A queue with fixed storage size.
    /// Currently it's only a naive implementation and needs to be further optimized in the future.
    /// </summary>
    public class FixedSizeQueue<T>
        : IEnumerable<T>
    {
        private readonly int _maxSize;
        private readonly List<T> _storage;

        internal IReadOnlyList<T> Items => _storage;

        /// <summary>
        /// Number of items in this queue
        /// </summary>
        public int Count => _storage.Count;

        /// <summary>
        /// Maximum number of items allowed in this queue
        /// </summary>
        public int Capacity => _maxSize;

        /// <summary>
        /// Create a new queue
        /// </summary>
        /// <param name="size">the maximum number of items to store in this queue</param>
        public FixedSizeQueue(int size)
        {
            _maxSize = size;
            _storage = new();
        }

        /// <summary>
        /// Fill the quene with the data. Please ensure that data.Count &lt;= size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="data"></param>
        public FixedSizeQueue(int size, IEnumerable<T> data)
        {
#if !NETSTANDARD2_0 
            // Try to check the size without enumerating the entire IEnumerable. This may not be able to get the count,
            // in which case we'll have to check later
            if (data.TryGetNonEnumeratedCount(out var dataCount) && dataCount > size)
                throw new ArgumentException($"The max size set for the quene is {size}, but got {dataCount} initial values.");
#endif

            // Size of "data" is unknown, copy it all into a list
            _maxSize = size;
            _storage = new List<T>(data);

            // Now check if that list is a valid size.
            if (_storage.Count > _maxSize)
                throw new ArgumentException($"The max size set for the quene is {size}, but got {_storage.Count} initial values.");
        }

        /// <summary>
        /// Replace every item in the queue with the given value
        /// </summary>
        /// <param name="value">The value to replace all items with</param>
        /// <returns>returns this</returns>
        public FixedSizeQueue<T> FillWith(T value)
        {
            for(var i = 0; i < Count; i++)
            {
                _storage[i] = value;
            }
            return this;
        }

        /// <summary>
        /// Enquene an element.
        /// </summary>
        /// <returns></returns>
        public void Enqueue(T item)
        {
            _storage.Add(item);
            if(_storage.Count >= _maxSize)
            {
                _storage.RemoveAt(0);
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
