using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

public static unsafe partial class NativeApi
{
    /// <summary>
    /// Sanity check for clip &lt;-&gt; llava embed size match
    /// </summary>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_validate_embed_size", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool llava_validate_embed_size( SafeLLamaContextHandle ctxLlama, SafeLlavaModelHandle ctxClip);

    /// <summary>
    /// Build an image embed from image file bytes
    /// </summary>
    /// <param name="ctx_clip"></param>
    /// <param name="n_threads"></param>
    /// <param name="image_bytes"></param>
    /// <param name="image_bytes_length"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_make_with_bytes",
        CallingConvention = CallingConvention.Cdecl)]
    public static extern 
        SafeLlavaImageEmbedHandle llava_image_embed_make_with_bytes(SafeLlavaModelHandle ctx_clip, int n_threads,
                                                                    byte[] image_bytes, int image_bytes_length);

    /// <summary>
    /// Build an image embed from a path to an image filename
    /// </summary>
    /// <param name="ctx_clip"></param>
    /// <param name="n_threads"></param>
    /// <param name="image_path"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_make_with_filename", CallingConvention = CallingConvention.Cdecl)]
    public static extern 
        SafeLlavaImageEmbedHandle llava_image_embed_make_with_filename(SafeLlavaModelHandle ctx_clip, int n_threads,
                                                                       [MarshalAs(UnmanagedType.LPStr)] string image_path);

    /// <summary>
    /// Free an embedding made with llava_image_embed_make_*
    /// </summary>
    /// <param name="embed"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void llava_image_embed_free(IntPtr embed);

    /// <summary>
    /// Write the image represented by embed into the llama context with batch size n_batch, starting at context
    /// pos n_past. on completion, n_past points to the next position in the context after the image embed.
    /// </summary>
    /// <param name="ctc_llama"></param>
    /// <param name="embed">ctx_llama</param>
    /// <param name="n_batch"></param>
    /// <param name="n_past"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_eval_image_embed", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool llava_eval_image_embed(SafeLLamaContextHandle ctc_llama, SafeLlavaImageEmbedHandle embed,
        int n_batch, ref int n_past);
    
}