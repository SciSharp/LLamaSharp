using System;

namespace LLama.Pooling;

/// <summary>
/// A memory rental which can be stored on the heap
/// </summary>
/// <typeparam name="T"></typeparam>
internal readonly struct MemoryRental<T>
    : IDisposable
{
    public readonly Memory<T> Memory;
    private readonly T[] _arr;

    private MemoryRental(T[] arr, Memory<T> mem)
    {
        _arr = arr;
        Memory = mem;
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static MemoryRental<T> Rent(int length)
    {
        return Rent(length, out _);
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <param name="memory"></param>
    /// <returns></returns>
    public static MemoryRental<T> Rent(int length, out Memory<T> memory)
    {
        var arr = ArrayPool<T>.Shared.Rent(length);
        memory = arr.AsMemory(0, length);

        return new(arr, memory);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_arr);
    }
}

/// <summary>
/// A rented span, cannot be stored on the heap. Use <see cref="MemoryRental{T}"/> if the
/// rental must be stored on the heap
/// </summary>
/// <typeparam name="T"></typeparam>
internal readonly ref struct SpanRental<T>
{
    public readonly Span<T> Span;
    private readonly bool _clear;
    private readonly T[] _arr;

    private SpanRental(T[] arr, Span<T> span, bool clear)
    {
        _arr = arr;
        Span = span;
        _clear = clear;
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <param name="clear"></param>
    /// <returns></returns>
    public static SpanRental<T> Rent(int length, bool clear = false)
    {
        return Rent(length, out _, clear);
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <param name="span"></param>
    /// <param name="clear"></param>
    /// <returns></returns>
    public static SpanRental<T> Rent(int length, out Span<T> span, bool clear = false)
    {
        var arr = ArrayPool<T>.Shared.Rent(length);
        span = arr.AsSpan(0, length);

        return new SpanRental<T>(arr, span, clear);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_arr, _clear);
    }
}