using System;
using System.Collections.Generic;

namespace LLama.Native;

/// <summary>
/// Disposes all contained disposables when this class is disposed
/// </summary>
internal sealed class GroupDisposable
    : IDisposable
{
    private bool _disposed;

    private readonly List<MemoryHandle> _handles = new();
    private readonly List<IDisposable> _disposables = new();

    /// <inheritdoc />
    ~GroupDisposable()
    {
        Dispose();
    }

    public MemoryHandle Add(MemoryHandle handle)
    {
        if (_disposed)
            throw new ObjectDisposedException("Cannot add new handle, already disposed");
        _handles.Add(handle);

        return handle;
    }

    public T Add<T>(T disposable)
        where T : class, IDisposable
    {
        if (_disposed)
            throw new ObjectDisposedException("Cannot add new IDisposable, already disposed");
        _disposables.Add(disposable);

        return disposable;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var memoryHandle in _handles)
            memoryHandle.Dispose();
        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}