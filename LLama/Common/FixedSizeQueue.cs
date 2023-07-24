using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Common
{
    /// <summary>
    /// A queue with fixed storage size.
    /// Currently it's only a naive implementation and needs to be further optimized in the future.
    /// </summary>
    public class FixedSizeQueue<T>: IEnumerable<T>
    {
        int _maxSize;
        List<T> _storage;

        public int Count => _storage.Count;
        public int Capacity => _maxSize;
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
            _maxSize = size;
            if(data.Count() > size)
            {
                throw new ArgumentException($"The max size set for the quene is {size}, but got {data.Count()} initial values.");
            }
            _storage = new(data);
        }

        public FixedSizeQueue<T> FillWith(T value)
        {
            for(int i = 0; i < Count; i++)
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

        public T[] ToArray()
        {
            return _storage.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
