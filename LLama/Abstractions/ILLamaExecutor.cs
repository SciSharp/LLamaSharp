using System.Collections.Generic;
using System.Threading;

namespace LLama.Abstractions
{
    /// <summary>
    /// A high level interface for LLama models.
    /// </summary>
    public interface ILLamaExecutor
    {
        /// <summary>
        /// The loaded context for this executor.
        /// </summary>
        public LLamaContext Context { get; }
        
        // LLava Section
        //
        /// <summary>
        /// Identify if it's a multi-modal model and there is a image to process.
        /// </summary>
        public bool IsMultiModal { get; }
        /// <summary>
        /// Muti-Modal Projections / Clip Model weights
        /// </summary>
        public LLavaWeights? ClipModel { get;  }

        /// <summary>
        /// List of images: Image filen path, uri or image byte array. See ImageData.
        /// </summary>
        public List<ImageData> Images { get; }

        /// <summary>
        /// Asynchronously infers a response from the model.
        /// </summary>
        /// <param name="text">Your prompt</param>
        /// <param name="inferenceParams">Any additional parameters</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns></returns>
        IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken token = default);
    }

    /// <summary>
    /// Holds image data
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public ImageData(DataType type, object data) { Type = type; Data = data; }

        /// <summary>
        /// the possible types of image data
        /// </summary>
        public enum DataType 
        { 
            /// <summary>
            /// file path
            /// </summary>
            ImagePath, 
            /// <summary>
            /// byte array
            /// </summary>
            ImageBytes, 
            /// <summary>
            /// uri
            /// </summary>
            ImageURL 
        }

        /// <summary>
        /// the type of this image data
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// the image data (string, byte array or uri)
        /// </summary>
        public object? Data { get; set; }
    }
}
