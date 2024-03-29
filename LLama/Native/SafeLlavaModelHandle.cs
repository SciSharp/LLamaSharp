using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LLama;
using LLama.Exceptions;


namespace LLama.Native
{
    /// <summary>
    /// A reference to a set of llava model weights.
    /// </summary>
    public sealed class SafeLlavaModelHandle
        : SafeLLamaHandleBase
    {

        private SafeLlavaModelHandle(IntPtr handle)
            : base(handle, true)
        {
        }

        private SafeLlavaModelHandle()
        {}
        
        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            clip_free(DangerousGetHandle());
            SetHandle(IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Load a model from the given file path into memory
        /// </summary>
        /// <param name="modelPath">MMP File (Multi-Modal Projections)</param>
        /// <param name="verbosity">Verbosity level</param>
        /// <returns>SafeHandle of the Clip Model</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlavaModelHandle LoadFromFile(string modelPath, int verbosity )
        {
            
            // Try to open the model file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(modelPath, FileMode.Open))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Llava MMP Model file '{modelPath}' is not readable");
          
            return clip_model_load(modelPath, verbosity)
                ?? throw new RuntimeError($"Failed to load LLaVa model {modelPath}.");          
        }

        /// <summary>
        /// Create the Image Embeddings.
        /// </summary>
        /// <param name="ctxLlama">LLama Context</param>
        /// <param name="image">Image filename (it supports jpeg  format only)</param>
        /// <returns>return the SafeHandle of these embeddings</returns>
        public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, string image)
        {
            return SafeLlavaImageEmbedHandle.CreateFromFileName(this, ctxLlama, image);
        }
        
        /// <summary>
        /// Create the Image Embeddings.
        /// </summary>
        /// <param name="ctxLlama">LLama Context</param>
        /// <param name="image">Image in binary format (it supports jpeg  format only)</param>
        /// <returns>return the SafeHandle of these embeddings</returns>
        public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, byte[] image )
        {
            return SafeLlavaImageEmbedHandle.CreateFromMemory(this, ctxLlama, image );
        }

        /// <summary>
        /// Evaluates the image embeddings. 
        /// </summary>
        /// <param name="ctxLlama">Llama Context</param>
        /// <param name="imageEmbed">The current embeddings to evaluate</param>
        /// <param name="n_past"></param>
        /// <returns>True on success</returns>
        public bool EvalImageEmbed(LLamaContext ctxLlama, SafeLlavaImageEmbedHandle imageEmbed, ref int n_past)
        {
            return NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, imageEmbed, (int)ctxLlama.Params.BatchSize, ref n_past );
        }
        
        /// <summary>
        /// Load MULTI MODAL PROJECTIONS model / Clip Model
        /// </summary>
        /// <param name="mmProj"> Model path/file</param>
        /// <param name="verbosity">Verbosity level</param>
        /// <returns>SafeLlavaModelHandle</returns>
        [DllImport(NativeApi.llavaLibraryName, EntryPoint = "clip_model_load", CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLlavaModelHandle clip_model_load(string mmProj, int verbosity);

        /// <summary>
        /// Frees MULTI MODAL PROJECTIONS model / Clip Model
        /// </summary>
        /// <param name="ctx">Internal Pointer to the model</param>
        [DllImport(NativeApi.llavaLibraryName, EntryPoint = "clip_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void clip_free(IntPtr ctx);
        
        
    }
}
