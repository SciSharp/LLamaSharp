using System;

namespace LLama.Exceptions
{
    /// <summary>
    /// Thrown when the KV cache context is full and the model architecture 
    /// cannot mathematically support native memory shifting, or when the 
    /// ContextOverflowStrategy.ThrowException is used.
    /// </summary>
    public class ContextOverflowException : Exception
    {
        private const string DefaultMessage = "The context window is full and the current strategy is set to ThrowException. To automatically truncate and manage context, set InferenceParams.OverflowStrategy to ContextOverflowStrategy.TruncateAndReprefill.";

        /// <summary>
        /// Initializes a new instance of the ContextOverflowException class with a default error message.
        /// </summary>
        public ContextOverflowException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContextOverflowException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ContextOverflowException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContextOverflowException class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ContextOverflowException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}