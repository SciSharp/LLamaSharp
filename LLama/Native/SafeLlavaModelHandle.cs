using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public SafeLlavaModelHandle(IntPtr handle)
            : base(handle, true)
        {
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            
            NativeApi.clip_free(DangerousGetHandle());
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
            
            var ctxContext =  NativeApi.clip_model_load(modelPath, verbosity ); 
            
            if (ctxContext == IntPtr.Zero)
                throw new RuntimeError($"Failed to load LLaVa model {modelPath}.");

            return new SafeLlavaModelHandle(ctxContext);
            
        }

        /// <summary>
        /// Load and embed image
        /// </summary>
        /// <param name="imagePath">Image path on jpeg format</param>
        /// <param name="threads"></param>
        public void LoadImage( string imagePath, int threads )
        {
            unsafe
            {
                NativeApi.llava_image_embed_make_with_filename( this.handle, threads,  imagePath);                   
            }
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
            unsafe
            {
                var ptrImageEmbed = NativeApi.llava_image_embed_make_with_filename(this.handle,  (int) ctxLlama.BatchThreads, image);
                bool result = NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, ptrImageEmbed, (int)ctxLlama.Params.BatchSize, ref n_past );
                NativeApi.llava_image_embed_free(ptrImageEmbed);
                return result;
            }            
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
            unsafe
            {
                var ptrImageEmbed = NativeApi.llava_image_embed_make_with_bytes(this.handle, (int) ctxLlama.BatchThreads, image.ToArray(), image.Length);
                bool result = NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, ptrImageEmbed, (int)ctxLlama.Params.BatchSize, ref n_past );
                NativeApi.llava_image_embed_free(ptrImageEmbed);
                return result;
            }
        }
    }
}
