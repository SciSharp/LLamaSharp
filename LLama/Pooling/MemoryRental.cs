using System;

namespace LLama.Pooling;

/// <summary>
/// A memory rental which can be stored on the heap
/// </summary>
/// <typeparam name="T"></typeparam>
internal readonly struct LongMemoryRental<T>
    : IDisposable
{
    public readonly Memory<T> Memory;
    private readonly T[] _arr;

    private LongMemoryRental(T[] arr, Memory<T> mem)
    {
        _arr = arr;
        Memory = mem;
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static LongMemoryRental<T> Rent(int length)
    {
        return Rent(length, out _);
    }

    /// <summary>
    /// Borrow a slice of memory which is the given length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static LongMemoryRental<T> Rent(int length, out Memory<T> memory)
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
/// A memory rental in a ref struct, cannot be stored on the heap
/// </summary>
/// <typeparam name="T"></typeparam>
internal readonly ref struct MemoryRental<T>
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
    /// <returns></returns>
    public static MemoryRental<T> Rent(int length, out Memory<T> memory)
    {
        var arr = ArrayPool<T>.Shared.Rent(length);
        memory = arr.AsMemory(0, length);

        return new MemoryRental<T>(arr, memory);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_arr);
    }
}