using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

using clip_ctx = IntPtr;
public static unsafe partial class NativeApi
{
    
    /*/// <summary>
    /// Clip Vision Parameters
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct clip_vision_hparams
    {
        public Int32 image_size;
        public Int32 patch_size;
        public Int32 hidden_size;
        public Int32 n_intermediate;
        public Int32 projection_dim;
        public Int32 n_head;
        public Int32 n_layer;
        public float eps;
    };*/

    /// <summary>
    /// LLaVa Image embeddings 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct llava_image_embed
    {
        public float* embed;
        public int n_image_pos;
    }

    /// <summary>
    /// Load MULTI MODAL PROJECTIONS model / Clip Model
    /// </summary>
    /// <param name="mmProj"> Model path/file</param>
    /// <param name="verbosity">Verbosity level</param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "clip_model_load", CallingConvention = CallingConvention.Cdecl)]
    public static extern clip_ctx clip_model_load(string mmProj, int verbosity);

    /// <summary>
    /// Frees MULTI MODAL PROJECTIONS model / Clip Model
    /// </summary>
    /// <param name="ctx"></param>
    [DllImport(llavaLibraryName, EntryPoint = "clip_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern void clip_free(clip_ctx ctx);

  
    /// <summary>
    /// Sanity check for clip <-> llava embed size match
    /// </summary>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_validate_embed_size", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool llava_validate_embed_size( SafeLLamaContextHandle ctxLlama, clip_ctx ctxClip);

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
    public static extern llava_image_embed* llava_image_embed_make_with_bytes(clip_ctx ctx_clip, int n_threads,
        byte[] image_bytes, int image_bytes_length);

    /// <summary>
    /// Build an image embed from a path to an image filename
    /// </summary>
    /// <param name="ctx_clip"></param>
    /// <param name="n_threads"></param>
    /// <param name="image_path"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_make_with_filename",
        CallingConvention = CallingConvention.Cdecl)]
    public static extern llava_image_embed* llava_image_embed_make_with_filename(clip_ctx ctx_clip, int n_threads,
        [MarshalAs(UnmanagedType.LPStr)] string image_path);

    /// <summary>
    /// Free an embedding made with llava_image_embed_make_*
    /// </summary>
    /// <param name="embed"></param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_free", CallingConvention = CallingConvention.Cdecl)]
    public static extern llava_image_embed* llava_image_embed_free(llava_image_embed* embed);

    /// <summary>
    /// Write the image represented by embed into the llama context with batch size n_batch, starting at context
    /// pos n_past. on completion, n_past points to the next position in the context after the image embed.
    /// </summary>
    /// <param name="embed">ctx_llama</param>
    /// <returns></returns>
    [DllImport(llavaLibraryName, EntryPoint = "llava_eval_image_embed", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool llava_eval_image_embed(SafeLLamaContextHandle ctc_llama, llava_image_embed* embed,
        int n_batch, out int n_past);
    
}