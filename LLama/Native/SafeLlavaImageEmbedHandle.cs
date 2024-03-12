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
    /// A Reference to a set of llava Image Embed handle
    /// </summary>
    public sealed class SafeLlavaImageEmbedHandle
        : SafeLLamaHandleBase
    {

        private SafeLlavaImageEmbedHandle(IntPtr handle)
            : base(handle, true)
        {
        }
        
        private SafeLlavaImageEmbedHandle()
        {}

        public static SafeLlavaImageEmbedHandle CreateFromFileName( SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, string image )
        {
            return NativeApi.llava_image_embed_make_with_filename(ctxLlava,  (int) ctxLlama.BatchThreads, image);
        }
        
        public static SafeLlavaImageEmbedHandle CreateFromMemory( SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, Byte[] image  )
        {
            return NativeApi.llava_image_embed_make_with_bytes(ctxLlava,  (int) ctxLlama.BatchThreads, image, image.Length);
        }
        
        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llava_image_embed_free(DangerousGetHandle());
            SetHandle(IntPtr.Zero);
            return true;
        }
    }
}
