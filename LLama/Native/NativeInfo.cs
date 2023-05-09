using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Native
{
    internal class NativeInfo
    {
        internal static readonly int LLAMA_FILE_VERSION = 1;
        internal static readonly string LLAMA_FILE_MAGIC = "ggjt";
        internal static readonly string LLAMA_FILE_MAGIC_UNVERSIONED = "ggml";
        internal static readonly string LLAMA_SESSION_MAGIC = "ggsn";
        internal static readonly int LLAMA_SESSION_VERSION = 1;
    }
}
