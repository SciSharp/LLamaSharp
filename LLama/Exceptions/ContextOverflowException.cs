using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Exceptions
{
    /// <summary>
    /// Thrown when the KV cache context is full and the model architecture 
    /// cannot mathematically support native memory shifting.
    /// </summary>
    public class ContextOverflowException(string message) : Exception(message)
    {
    }
}