using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLama.Common
{
    /// <summary>
    /// A queue with fixed storage size backed by a circular buffer.
    /// </summary>
    public class FixedSizeQueue<T>
        : IReadOnlyList<T>
    {
        private readonly T[] _buffer;
        private int _start;
        private int _count;
        private T[]? _window;

        // Minimum capacity for the temporary buffer used to expose a contiguous view.
        private const int MinimumWindowSize = 4;
        // Resize multiplier for the temporary buffer to reduce copy churn as it grows.
        private const int WindowGrowthFactor = 2;

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var actualIndex = (_start + index) % Capacity;
                return _buffer[actualIndex];
            }
        }

        /// <summary>
        /// Number of items in this queue
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Maximum number of items allowed in this queue
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Create a new queue.
        /// </summary>
        /// <param name="size">The maximum number of items to store in this queue.</param>
        public FixedSizeQueue(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Capacity must be greater than zero.");

            Capacity = size;
            _buffer = new T[size];
            _start = 0;
            _count = 0;
        }

        /// <summary>
        /// Fill the queue with existing data. Please ensure that data.Count &lt;= size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="data"></param>
        public FixedSizeQueue(int size, IEnumerable<T> data)
            : this(size)
        {
#if NET6_0_OR_GREATER
            if (data.TryGetNonEnumeratedCount(out var dataCount) && dataCount > size)
                throw new ArgumentException($"The max size set for the queue is {size}, but got {dataCount} initial values.");
#endif

            if (data is ICollection<T> collection)
            {
                if (collection.Count > size)
                    throw new ArgumentException($"The max size set for the queue is {size}, but got {collection.Count} initial values.");

                foreach (var item in collection)
                    Enqueue(item);
                return;
            }

            var index = 0;
            foreach (var item in data)
            {
                if (index >= size)
                    throw new ArgumentException($"The max size set for the queue is {size}, but got {index + 1} initial values.");

                Enqueue(item);
                index++;
            }
        }

        /// <summary>
        /// Enqueue an element. When the queue is full the oldest element is overwritten.
        /// </summary>
        public void Enqueue(T item)
        {
            if (_count < Capacity)
            {
                var tail = (_start + _count) % Capacity;
                _buffer[tail] = item;
                _count++;
            }
            else
            {
                _buffer[_start] = item;
                _start++;
                if (_start == Capacity)
                    _start = 0;
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> Enumerate()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return this[i];
            }
        }
    }
}
