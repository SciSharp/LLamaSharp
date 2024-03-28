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
    /// A Reference to a llava Image Embed handle
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

        /// <summary>
        /// Create an image embed from an image file
        /// </summary>
        /// <param name="ctxLlava"></param>
        /// <param name="ctxLlama"></param>
        /// <param name="image">Path to the image file. Supported formats:
        /// <list type="bullet">
        ///     <item>JPG</item>
        ///     <item>PNG</item>
        ///     <item>BMP</item>
        ///     <item>TGA</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static SafeLlavaImageEmbedHandle CreateFromFileName( SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, string image )
        {
            // Try to open the image file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(image, FileMode.Open))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Llava image file '{image}' is not readable");
            return NativeApi.llava_image_embed_make_with_filename(ctxLlava,  (int) ctxLlama.BatchThreads, image);
        }
        
        /// <summary>
        /// Create an image embed from the bytes of an image.
        /// </summary>
        /// <param name="ctxLlava"></param>
        /// <param name="ctxLlama"></param>
        /// <param name="image">Image bytes. Supported formats:
        /// <list type="bullet">
        ///     <item>JPG</item>
        ///     <item>PNG</item>
        ///     <item>BMP</item>
        ///     <item>TGA</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public static SafeLlavaImageEmbedHandle CreateFromMemory( SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, byte[] image  )
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
