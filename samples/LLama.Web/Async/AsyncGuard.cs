using System.Collections.Concurrent;

namespace LLama.Web.Async
{

    /// <summary>
    /// Creates a async/thread-safe guard helper
    /// </summary>
    /// <seealso cref="AsyncGuard&lt;byte&gt;" />
    public class AsyncGuard : AsyncGuard<byte>
    {
        private readonly byte _key;
        private readonly ConcurrentDictionary<byte, bool> _lockData;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGuard"/> class.
        /// </summary>
        public AsyncGuard()
        {
            _key = 0;
            _lockData = new ConcurrentDictionary<byte, bool>();
        }


        /// <summary>
        /// Guards this instance.
        /// </summary>
        /// <returns>true if able to enter an guard, false if already guarded</returns>
        public bool Guard()
        {
            return _lockData.TryAdd(_key, true);
        }


        /// <summary>
        /// Releases the guard.
        /// </summary>
        /// <returns></returns>
        public bool Release()
        {
            return _lockData.TryRemove(_key, out _);
        }


        /// <summary>
        /// Determines whether this instance is guarded.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is guarded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGuarded()
        {
            return _lockData.ContainsKey(_key);
        }
    }


    public class AsyncGuard<T>
    {
        private readonly ConcurrentDictionary<T, bool> _lockData;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGuard{T}"/> class.
        /// </summary>
        public AsyncGuard()
        {
            _lockData = new ConcurrentDictionary<T, bool>();
        }


        /// <summary>
        /// Guards the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true if able to enter a guard for this value, false if this value is already guarded</returns>
        public bool Guard(T value)
        {
            return _lockData.TryAdd(value, true);
        }


        /// <summary>
        /// Releases the guard on the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Release(T value)
        {
            return _lockData.TryRemove(value, out _);
        }


        /// <summary>
        /// Determines whether the specified value is guarded.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is guarded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGuarded(T value)
        {
            return _lockData.ContainsKey(value);
        }
    }
}
