using System;

namespace LLama.Native;

public static partial class NativeApi
{
    /// <summary>
    /// Clear the memory contents. If data == true, the data buffers will also be cleared together with the metadata
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="data"></param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void llama_memory_clear(IntPtr /* llama_memory_t */ mem, [MarshalAs(UnmanagedType.U1)] bool data);

    /// <summary>
    /// Removes all tokens that belong to the specified sequence and have positions in [p0, p1)
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns>Returns false if a partial sequence cannot be removed. Removing a whole sequence never fails</returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool llama_memory_seq_rm(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq, LLamaPos p0, LLamaPos p1);

    /// <summary>
    /// Copy all tokens that belong to the specified sequence to another sequence
    /// Note that this does not allocate extra KV cache memory - it simply assigns the tokens to the new sequence
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    /// <param name="p0">p0 &lt; 0 : [0,  p1]</param>
    /// <param name="p1">p1 &lt; 0 : [p0, inf)</param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void llama_memory_seq_cp(IntPtr /* llama_memory_t */ mem, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1);

    /// <summary>
    /// Removes all tokens that do not belong to the specified sequence
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void llama_memory_seq_keep(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq);

    /// <summary>
    /// Adds relative position "delta" to all tokens that belong to the specified sequence and have positions in [p0, p1)
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    /// <param name="p0">p0 &lt; 0 : [0,  p1]</param>
    /// <param name="p1">p1 &lt; 0 : [p0, inf)</param>
    /// <param name="delta"></param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void llama_memory_seq_add(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta);

    /// <summary>
    /// Integer division of the positions by factor of `d > 1`
    /// <br />
    /// p0 &lt; 0 : [0,  p1]
    /// <br />
    /// p1 &lt; 0 : [p0, inf)
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    /// <param name="p0">p0 &lt; 0 : [0,  p1]</param>
    /// <param name="p1">p1 &lt; 0 : [p0, inf)</param>
    /// <param name="d"></param>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void llama_memory_seq_div(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d);

    /// <summary>
    /// Returns the smallest position present in the memory for the specified sequence.
    /// This is typically non-zero only for SWA caches.
    /// Note that all positions in the range [pos_min, pos_max] are guaranteed to be present in the memory.
    /// Return -1 if the sequence is empty.
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern LLamaPos llama_memory_seq_pos_min(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq);

    /// <summary>
    /// Returns the largest position present in the memory for the specified sequence.
    /// Note that all positions in the range [pos_min, pos_max] are guaranteed to be present in the memory.
    /// Return -1 if the sequence is empty.
    /// </summary>
    /// <param name="mem"></param>
    /// <param name="seq"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern LLamaPos llama_memory_seq_pos_max(IntPtr /* llama_memory_t */ mem, LLamaSeqId seq);

    /// <summary>
    /// Check if the memory supports shifting
    /// </summary>
    /// <param name="mem"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static extern bool llama_memory_can_shift(IntPtr /* llama_memory_t */ mem);
}