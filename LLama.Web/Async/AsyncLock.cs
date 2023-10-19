namespace LLama.Web.Async
{
    /// <summary>
    /// Create an Async locking using statment
    /// </summary>
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Task<IDisposable> _releaser;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLock"/> class.
        /// </summary>
        public AsyncLock()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _releaser = Task.FromResult((IDisposable)new Releaser(this));
        }


        /// <summary>
        /// Locks the using statement asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task<IDisposable> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            if (wait.IsCompleted)
                return _releaser;

            return wait.ContinueWith((_, state) => (IDisposable)state, _releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        /// <summary>
        /// IDisposable wrapper class to release the lock on dispose
        /// </summary>
        /// <seealso cref="IDisposable" />
        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock _lockToRelease;

            internal Releaser(AsyncLock lockToRelease)
            {
                _lockToRelease = lockToRelease;
            }

            public void Dispose()
            {
                _lockToRelease._semaphore.Release();
            }
        }
    }
}
