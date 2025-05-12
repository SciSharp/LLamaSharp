using System;

namespace LLama.Abstractions
{
    /// <summary>
    /// Represents a mapping between a tensor name pattern and a specific buffer type
    /// </summary>
    public class TensorBufferOverride
    {
        /// <summary>
        /// Pattern to match tensor names. This is a regular expression. You can check the tensor names via the model.Metadata.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Buffer type to use for matching tensors. Examples: CPU, GPU0, GPU1
        /// </summary>
        public string BufferType { get; set; }

        /// <summary>
        /// Creates a new tensor buffer override
        /// </summary>
        /// <param name="pattern">Pattern to match tensor names</param>
        /// <param name="bufferType">Buffer type to use for matching tensors</param>
        public TensorBufferOverride(string pattern, string bufferType)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
            if (string.IsNullOrEmpty(bufferType))
                throw new ArgumentException("Buffer type cannot be null or empty", nameof(bufferType));

            Pattern = pattern;
            BufferType = bufferType;
        }
    }
}
