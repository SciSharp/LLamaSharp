using System;

namespace LLama.Native;

/// <summary>
/// A safe handle for a LLamaKvCacheView
/// </summary>
public sealed class LLamaKvCacheViewSafeHandle
    : SafeLLamaHandleBase
{
    private readonly SafeLLamaContextHandle _ctx;
    private NativeLLamaKvCacheView _view;

    /// <summary>
    /// Number of KV cache cells. This will be the same as the context size.
    /// </summary>
    public int CellCount => GetNativeView().n_cells;

    /// <summary>
    /// Get the total number of tokens in the KV cache.
    ///
    /// For example, if there are two populated
    /// cells, the first with 1 sequence id in it and the second with 2 sequence
    /// ids then you'll have 3 tokens.
    /// </summary>
    public int TokenCount => GetNativeView().token_count;
    
    /// <summary>
    /// Maximum number of sequences visible for a cell. There may be more sequences than this
    /// in reality, this is simply the maximum number this view can see.
    /// </summary>
    public int MaxSequenceCount => GetNativeView().n_seq_max;
    
    /// <summary>
    /// Number of populated cache cells
    /// </summary>
    public int UsedCellCount => GetNativeView().used_cells;

    /// <summary>
    /// Maximum contiguous empty slots in the cache.
    /// </summary>
    public int MaxContiguous => GetNativeView().max_contiguous;

    /// <summary>
    /// Index to the start of the MaxContiguous slot range. Can be negative when cache is full.
    /// </summary>
    public int MaxContiguousIdx => GetNativeView().max_contiguous;

    /// <summary>
    /// Initialize a LLamaKvCacheViewSafeHandle which will call `llama_kv_cache_view_free` when disposed
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="view"></param>
    private LLamaKvCacheViewSafeHandle(SafeLLamaContextHandle ctx, NativeLLamaKvCacheView view)
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
        // Allocate the view
        var view = llama_kv_cache_view_init(ctx, maxSequences);
        var handle = new LLamaKvCacheViewSafeHandle(ctx, view);

        // Update the view so it has valid data after allocation.
        handle.Update();

        return handle;
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        llama_kv_cache_view_free(ref _view);
        SetHandle(IntPtr.Zero);

        return true;
    }

    /// <summary>
    /// Read the current KV cache state into this view.
    /// </summary>
    public void Update()
    {
        llama_kv_cache_view_update(_ctx, ref _view);
    }

    /// <summary>
    /// Get the raw KV cache view
    /// </summary>
    /// <returns></returns>
    private ref NativeLLamaKvCacheView GetNativeView()
    {
        if (IsClosed)
            throw new ObjectDisposedException("Cannot access LLamaKvCacheViewSafeHandle after is has been disposed");

        return ref _view;
    }

    /// <summary>
    /// Get the cell at the given index
    /// </summary>
    /// <param name="index">The index of the cell [0, CellCount)</param>
    /// <returns>Data about the cell at the given index</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of range (0 &lt;= index &lt; CellCount)</exception>
    public LLamaPos GetCell(int index)
    {
        var view = GetNativeView();

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Cell index must be >= 0");
        if (index >= view.n_cells)
            throw new ArgumentOutOfRangeException(nameof(index), "Cell index must be < CellCount");
        
        unsafe
        {
            return view.cells[index].pos;
        }
    }

    /// <summary>
    /// Get all of the sequences assigned to the cell at the given index. This will contain <see cref="MaxSequenceCount"/> entries
    /// sequences even if the cell actually has more than that many sequences, allocate a new view with a larger maxSequences parameter
    /// if necessary. Invalid sequences will be negative values.
    /// </summary>
    /// <param name="index">The index of the cell [0, CellCount)</param>
    /// <returns>A span containing the sequences assigned to this cell</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of range (0 &lt;= index &lt; CellCount)</exception>
    public Span<LLamaSeqId> GetCellSequences(int index)
    {
        var view = GetNativeView();
        
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Cell index  must be >= 0");
        if (index >= view.n_cells)
            throw new ArgumentOutOfRangeException(nameof(index), "Cell index must be < CellCount");
        
        unsafe
        {
            return new Span<LLamaSeqId>(&view.cells_sequences[index * view.n_seq_max], view.n_seq_max);
        }
    }

    #region native API
    /// <summary>
    /// Create an empty KV cache view. (use only for debugging purposes)
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="n_seq_max"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern NativeLLamaKvCacheView llama_kv_cache_view_init(SafeLLamaContextHandle ctx, int n_seq_max);
    
    /// <summary>
    /// Free a KV cache view. (use only for debugging purposes)
    /// </summary>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_view_free(ref NativeLLamaKvCacheView view);
    
    /// <summary>
    /// Update the KV cache view structure with the current state of the KV cache. (use only for debugging purposes)
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="view"></param>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_view_update(SafeLLamaContextHandle ctx, ref NativeLLamaKvCacheView view);
    
    /// <summary>
    /// Information associated with an individual cell in the KV cache view (llama_kv_cache_view_cell)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeLLamaKvCacheViewCell
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
    private unsafe struct NativeLLamaKvCacheView
    {
        /// <summary>
        /// Number of KV cache cells. This will be the same as the context size.
        /// </summary>
        public int n_cells;
        
        /// <summary>
        /// Maximum number of sequences that can exist in a cell. It's not an error
        /// if there are more sequences in a cell than this value, however they will
        /// not be visible in the view cells_sequences.
        /// </summary>
        public int n_seq_max;
        
        /// <summary>
        /// Number of tokens in the cache. For example, if there are two populated
        /// cells, the first with 1 sequence id in it and the second with 2 sequence
        /// ids then you'll have 3 tokens.
        /// </summary>
        public int token_count;
        
        /// <summary>
        /// Number of populated cache cells.
        /// </summary>
        public int used_cells;
        
        /// <summary>
        /// Maximum contiguous empty slots in the cache.
        /// </summary>
        public int max_contiguous;
        
        /// <summary>
        /// Index to the start of the max_contiguous slot range. Can be negative
        /// when cache is full.
        /// </summary>
        public int max_contiguous_idx;
        
        /// <summary>
        /// Information for an individual cell.
        /// </summary>
        public NativeLLamaKvCacheViewCell* cells;
        
        /// <summary>
        /// The sequences for each cell. There will be n_seq_max items per cell.
        /// </summary>
        public LLamaSeqId* cells_sequences;
    }
    #endregion
}