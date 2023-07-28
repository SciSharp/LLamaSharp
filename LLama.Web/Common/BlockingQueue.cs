using System.Collections.Concurrent;

namespace LLama.Web.Common
{
    public class BlockingQueue<T, U>
    {
        private readonly Func<T, Task<U>> _processFunction;

        private readonly BlockingCollection<BlockingQueueItem<T, U>> _processQueue = new BlockingCollection<BlockingQueueItem<T, U>>();

        public BlockingQueue(Func<T, Task<U>> processFunction)
        {
            _processFunction = processFunction;
            Task.Factory.StartNew(async () => await ProcessQueue(), TaskCreationOptions.LongRunning);
        }

        public Task<U> QueueItem(T item)
        {
            var queueItem = new BlockingQueueItem<T, U>(item);
            _processQueue.TryAdd(queueItem);
            return queueItem.CompletionSource.Task;
        }

     
        private async Task ProcessQueue()
        {
            while (_processQueue.TryTake(out var queueItem, Timeout.Infinite))
            {
                var input = queueItem.Item;
                var tcs = queueItem.CompletionSource;

                try
                {
                    tcs.SetResult(await _processFunction(input));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
        }

        public async Task StopProcessing()
        {
            _processQueue.CompleteAdding();
            while (!_processQueue.IsCompleted)
                await Task.Delay(500);

            _processQueue.Dispose();
        }

        public int QueueCount()
        {
            return _processQueue.Count;
        }
    }

    public class BlockingQueue<T> : IDisposable
    {
        private readonly Func<T, Task> _processFunction;

        private readonly BlockingCollection<T> _processQueue = new BlockingCollection<T>();

        public BlockingQueue(Func<T, Task> processFunction)
        {
            _processFunction = processFunction;
            Task.Factory.StartNew(async () => await ProcessQueue(), TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            if (!IsCompleted)
                CloseQueue();
        }

        public void QueueItem(T item)
        {
            _processQueue.TryAdd(item);
        }

        public int QueueCount()
        {
            return _processQueue.Count;
        }

        public void CloseQueue()
        {
            _processQueue.CompleteAdding();
        }

        public bool IsCompleted
        {
            get { return _processQueue.IsCompleted; }
        }

        private async Task ProcessQueue()
        {
            while (_processQueue.TryTake(out var queueItem, Timeout.Infinite))
                await _processFunction(queueItem);
        }
    }

    public class BlockingQueueItem<T, U>
    {
        public BlockingQueueItem(T item)
        {
            Item = item;
            CompletionSource = new TaskCompletionSource<U>();
        }

        public T Item { get; set; }

        public TaskCompletionSource<U> CompletionSource { get; set; }
    }
}
