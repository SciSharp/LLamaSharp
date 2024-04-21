using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Extensions
{
    /// <summary>
    /// Extension to use <see cref="LinkedList{T}"/> as a deque.
    /// </summary>
    public static class DequeExtensions
    {
        /// <inheritdoc/>
        public static void AddFront<T>(this LinkedList<T> deque, T item)
        {
            deque.AddFirst(item);
        }

        /// <inheritdoc/>
        public static void AddBack<T>(this LinkedList<T> deque, T item)
        {
            deque.AddLast(item);
        }

        /// <inheritdoc/>
        public static T RemoveFront<T>(this LinkedList<T> deque)
        {
            if (deque.Count == 0)
            {
                throw new InvalidOperationException("The deque is empty.");
            }

            T item = deque.First!.Value;
            deque.RemoveFirst();
            return item;
        }

        /// <inheritdoc/>
        public static T RemoveBack<T>(this LinkedList<T> deque)
        {
            if (deque.Count == 0)
            {
                throw new InvalidOperationException("The deque is empty.");
            }

            T item = deque.Last!.Value;
            deque.RemoveLast();
            return item;
        }

        /// <inheritdoc/>
        public static T PeekFront<T>(this LinkedList<T> deque)
        {
            if (deque.Count == 0)
            {
                throw new InvalidOperationException("The deque is empty.");
            }

            return deque.First!.Value;
        }

        /// <inheritdoc/>
        public static T PeekBack<T>(this LinkedList<T> deque)
        {
            if (deque.Count == 0)
            {
                throw new InvalidOperationException("The deque is empty.");
            }

            return deque.Last!.Value;
        }

        /// <inheritdoc/>
        public static void ExtendFront<T>(this LinkedList<T> deque, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                deque.AddFront(item);
            }
        }

        /// <inheritdoc/>
        public static void ExtendBack<T>(this LinkedList<T> deque, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                deque.AddBack(item);
            }
        }
    }
}
