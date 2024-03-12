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
    /// A reference to a set of llava model weights
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
        /// <param name="modelPath"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
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
        /// Embed the image from file in llama context
        /// </summary>
        /// <param name="ctxLlama"></param>
        /// <param name="image"></param>
        /// <param name="n_past"></param>
        /// <returns></returns>
        public bool EmbedImage(LLamaContext ctxLlama, string image, ref int n_past)
        {
            var ImageEmbed = SafeLlavaImageEmbedHandle.CreateFromFileName(this, ctxLlama, image);
            bool result = NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, ImageEmbed, (int)ctxLlama.Params.BatchSize, ref n_past );
            return result;
        }
        
        /// <summary>
        /// Embed the image from binary in llama context
        /// </summary>
        /// <param name="ctxLlama"></param>
        /// <param name="image">jpeg image</param>
        /// <param name="n_past"></param>
        /// <returns></returns>
        public bool EmbedImage(LLamaContext ctxLlama, Byte[] image, ref int n_past )
        {
            var ImageEmbed = SafeLlavaImageEmbedHandle.CreateFromMemory(this, ctxLlama, image );
            bool result = NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, ImageEmbed, (int)ctxLlama.Params.BatchSize, ref n_past );
            return result;
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
        /// <param name="ctx"></param>
        [DllImport(NativeApi.llavaLibraryName, EntryPoint = "clip_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void clip_free(IntPtr ctx);
        
        
    }
}
