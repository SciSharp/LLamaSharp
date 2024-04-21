using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Config
{
    /// <summary>
    /// Configuration for the KV cache.
    /// </summary>
    public class KvCacheConfig
    {
        /// <summary>
        /// The maximum CPU memory space used for saving kv cache swapped from GPU.
        /// </summary>
        public int MaxSwapSpace { get; set; }
    }
}
