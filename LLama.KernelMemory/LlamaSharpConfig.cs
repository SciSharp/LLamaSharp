using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    public class LlamaSharpConfig
    {
        public LlamaSharpConfig(string modelPath)
        {
            ModelPath = modelPath;
        }

        public string ModelPath { get; set; }
        public uint? ContextSize { get; set; }
        public uint? Seed { get; set; }
        public int? GpuLayerCount { get; set; }
    }
}
