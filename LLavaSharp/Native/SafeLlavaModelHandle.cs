using System;
using System.Collections.Generic;
using System.Text;
using LLama;
using LLama.Exceptions;
using LLama.Native;

namespace LLava.Native
{
    /// <summary>
    /// A reference to a set of llava model weights
    /// </summary>
    public sealed class SafeLlavaModelHandle
        : SafeLLamaHandleBase
    {

        internal protected SafeLlavaModelHandle(IntPtr handle)
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
            var ctxContext =  NativeApi.clip_model_load(modelPath, verbosity );            
            if (ctxContext == IntPtr.Zero)
                throw new RuntimeError($"Failed to load LLaVa model {modelPath}.");

            return new SafeLlavaModelHandle(ctxContext);
        }

        public void LoadImage( string imagePath, int threads )
        {
            const string image = "/Users/jlsantiago/Documents/Models/llava/dos.jpg"; 
            unsafe
            {
                NativeApi.llava_image_embed_make_with_filename( this.handle, threads,  image);                   
            }
        }

        public bool EmbedImage(LLamaContext ctxLlama, Byte[] image, out int n_past )
        {
            unsafe
            {
                var ptrImageEmbed = NativeApi.llava_image_embed_make_with_bytes(this.handle, 1, image.ToArray(), image.Length);
                bool result = NativeApi.llava_eval_image_embed(ctxLlama.NativeHandle, ptrImageEmbed, (int)ctxLlama.Params.BatchSize, out n_past );
                NativeApi.llava_image_embed_free(ptrImageEmbed);
                return result;
            }
        }
    }
}
