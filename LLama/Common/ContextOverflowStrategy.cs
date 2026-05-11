using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Common
{
    /// <summary>
    /// Defines how the executor should behave when the context window fills up 
    /// on a model that does not support native memory shifting (e.g., 2D RoPE models).
    /// </summary>
    public enum ContextOverflowStrategy
    {
        /// <summary>
        /// The engine will throw a ContextOverflowException. 
        /// Use this to manually manage context pruning in your application layer.
        /// (Equivalent to llama-cli's --no-context-shift).
        /// </summary>
        ThrowException,

        /// <summary>
        /// The engine will silently drop a percentage of the oldest tokens 
        /// (preserving the system prompt) and completely re-prefill the context.
        /// </summary>
        TruncateAndReprefill
    }
}
