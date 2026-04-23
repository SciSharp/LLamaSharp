using System;

namespace LLama.Exceptions
{
    /// <summary>
    /// Thrown when the KV cache context is full and the model architecture 
    /// cannot mathematically support native memory shifting, or when the 
    /// ContextOverflowStrategy.ThrowException is used.
    /// </summary>
    public class ContextOverflowException() : Exception("The context window is full and the current strategy is set to ThrowException. To automatically truncate and manage context, set InferenceParams.OverflowStrategy to ContextOverflowStrategy.TruncateAndReprefill.")
    {
    }
}