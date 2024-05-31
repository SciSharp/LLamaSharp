using System;
using System.IO;


namespace LLama.Native
{
    /// <summary>
    /// A Reference to a llava Image Embed handle
    /// </summary>
    public sealed class SafeLlavaImageEmbedHandle
        : SafeLLamaHandleBase
    {
        /// <summary>
        /// Get the model used to create this image embedding
        /// </summary>
        public SafeLlavaModelHandle Model { get; private set; } = null!;
        
        /// <summary>
        /// Get the number of dimensions in an embedding
        /// </summary>
        public int EmbeddingDimensions => Model.EmbeddingDimensions;
        
        /// <summary>
        /// Get the number of "patches" in an image embedding
        /// </summary>
        public int PatchCount => Model.PatchCount;

        #region embed
        /// <summary>
        /// Create an image embed from an image file
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="ctx"></param>
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
        public static SafeLlavaImageEmbedHandle CreateFromFileName(SafeLlavaModelHandle clip, LLamaContext ctx, string image)
        {
            if (!NativeApi.llava_validate_embed_size(ctx.NativeHandle, clip))
                throw new InvalidOperationException($"Cannot create image embed. Embedding dim of the multimodal projector ({clip.EmbeddingDimensions}) is not equal to embedding dim of model ({ctx.EmbeddingSize})");

            return CreateFromFileName(clip, image, (int)ctx.BatchThreads);
        }
        
        /// <summary>
        /// Create an image embed from an image file
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="image">Path to the image file. Supported formats:
        /// <list type="bullet">
        ///     <item>JPG</item>
        ///     <item>PNG</item>
        ///     <item>BMP</item>
        ///     <item>TGA</item>
        /// </list>
        /// </param>
        /// <param name="threads"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static SafeLlavaImageEmbedHandle CreateFromFileName(SafeLlavaModelHandle clip, string image, int threads = -1)
        {
            if (threads <= 0)
                threads = Environment.ProcessorCount / 2;

            // Try to open the image file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(image, FileMode.Open))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Llava image file '{image}' is not readable");
            
            var embed = NativeApi.llava_image_embed_make_with_filename(clip, threads, image);
            embed.Model = clip;
            return embed;
        }

        /// <summary>
        /// Create an image embed from the bytes of an image.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="ctx"></param>
        /// <param name="image">Image bytes. Supported formats:
        /// <list type="bullet">
        ///     <item>JPG</item>
        ///     <item>PNG</item>
        ///     <item>BMP</item>
        ///     <item>TGA</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public static SafeLlavaImageEmbedHandle CreateFromMemory(SafeLlavaModelHandle clip, LLamaContext ctx, byte[] image)
        {
            if (!NativeApi.llava_validate_embed_size(ctx.NativeHandle, clip))
                throw new InvalidOperationException($"Cannot create image embed. Embedding dim of the multimodal projector ({clip.EmbeddingDimensions}) is not equal to embedding dim of model ({ctx.EmbeddingSize})");
            
            return CreateFromMemory(clip, image, (int)ctx.BatchThreads);
        }
        
        /// <summary>
        /// Create an image embed from the bytes of an image.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="image">Image bytes. Supported formats:
        ///     <list type="bullet">
        ///         <item>JPG</item>
        ///         <item>PNG</item>
        ///         <item>BMP</item>
        ///         <item>TGA</item>
        ///     </list>
        /// </param>
        /// <param name="threads"></param>
        /// <returns></returns>
        public static SafeLlavaImageEmbedHandle CreateFromMemory(SafeLlavaModelHandle clip, byte[] image, int threads = -1)
        {
            if (threads <= 0)
                threads = Environment.ProcessorCount / 2;

            var embed = NativeApi.llava_image_embed_make_with_bytes(clip, threads, image, image.Length);
            embed.Model = clip;
            return embed;
        }
        #endregion

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llava_image_embed_free(DangerousGetHandle());
            SetHandle(IntPtr.Zero);
            return true;
        }
        
        /// <summary>
        /// Copy the embeddings data to the destination span
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="index"></param>
        public void GetEmbedding(Span<float> dest, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "index must be >= 0");
            if (index >= Model.PatchCount)
                throw new ArgumentOutOfRangeException(nameof(index), "index must be < Model.PatchCount");

            unsafe
            {
                var embed = (LLavaImageEmbed*)DangerousGetHandle();
                new Span<float>(
                    embed->embed + Model.EmbeddingDimensions * index,
                    Model.EmbeddingDimensions
                ).CopyTo(dest);
            }
        }
    }
}
