using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Information associated with an individual cell in the KV cache view (llama_kv_cache_view_cell)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaKvCacheViewCell
{
    /// <summary>
    /// The position for this cell. Takes KV cache shifts into account.
    /// May be negative if the cell is not populated.
    /// </summary>
    public LLamaPos pos;
}

/// <summary>
/// An updateable view of the KV cache (llama_kv_cache_view)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLamaKvCacheView
{
    // Number of KV cache cells. This will be the same as the context size.
    int n_cells;

    // Maximum number of sequences that can exist in a cell. It's not an error
    // if there are more sequences in a cell than this value, however they will
    // not be visible in the view cells_sequences.
    int n_max_seq;

    // Number of tokens in the cache. For example, if there are two populated
    // cells, the first with 1 sequence id in it and the second with 2 sequence
    // ids then you'll have 3 tokens.
    int token_count;

    // Number of populated cache cells.
    int used_cells;

    // Maximum contiguous empty slots in the cache.
    int max_contiguous;

    // Index to the start of the max_contiguous slot range. Can be negative
    // when cache is full.
    int max_contiguous_idx;

    // Information for an individual cell.
    LLamaKvCacheViewCell* cells;

    // The sequences for each cell. There will be n_max_seq items per cell.
    LLamaSeqId* cells_sequences;
}

/// <summary>
/// A safe handle for a LLamaKvCacheView
/// </summary>
public class LLamaKvCacheViewSafeHandle
    : SafeLLamaHandleBase
{
    private readonly SafeLLamaContextHandle _ctx;
    private LLamaKvCacheView _view;

    /// <summary>
    /// Initialize a LLamaKvCacheViewSafeHandle which will call `llama_kv_cache_view_free` when disposed
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="view"></param>
    public LLamaKvCacheViewSafeHandle(SafeLLamaContextHandle ctx, LLamaKvCacheView view)
        : base((IntPtr)1, true)
    {
        _ctx = ctx;
        _view = view;
    }

    /// <summary>
    /// Allocate a new KV cache view which can be used to inspect the KV cache
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="maxSequences">The maximum number of sequences visible in this view per cell</param>
    /// <returns></returns>
    public static LLamaKvCacheViewSafeHandle Allocate(SafeLLamaContextHandle ctx, int maxSequences)
    {
        var result = NativeApi.llama_kv_cache_view_init(ctx, maxSequences);
        return new LLamaKvCacheViewSafeHandle(ctx, result);
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        NativeApi.llama_kv_cache_view_free(ref _view);
        SetHandle(IntPtr.Zero);

        return true;
    }

    /// <summary>
    /// Update this view
    /// </summary>
    public void Update()
    {
        NativeApi.llama_kv_cache_view_update(_ctx, ref _view);
    }

    /// <summary>
    /// Get the raw KV cache view
    /// </summary>
    /// <returns></returns>
    public ref LLamaKvCacheView GetView()
    {
        return ref _view;
    }
}

public static partial class NativeApi
{
    /// <summary>
    /// Create an empty KV cache view. (use only for debugging purposes)
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="n_max_seq"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern LLamaKvCacheView llama_kv_cache_view_init(SafeLLamaContextHandle ctx, int n_max_seq);

    /// <summary>
    /// Free a KV cache view. (use only for debugging purposes)
    /// </summary>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void llama_kv_cache_view_free(ref LLamaKvCacheView view);

    /// <summary>
    /// Update the KV cache view structure with the current state of the KV cache. (use only for debugging purposes)
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="view"></param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void llama_kv_cache_view_update(SafeLLamaContextHandle ctx, ref LLamaKvCacheView view);

    /// <summary>
    /// Returns the number of tokens in the KV cache (slow, use only for debug)
    /// If a KV cell has multiple sequences assigned to it, it will be counted multiple times
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx);

    /// <summary>
    /// Returns the number of used KV cells (i.e. have at least one sequence assigned to them)
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int llama_get_kv_cache_used_cells(SafeLLamaContextHandle ctx);
}