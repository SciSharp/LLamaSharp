using LLama.Abstractions;

namespace LLama.Web.Common
{
    public class ModelOptions : IModelParams
    {
        public int MaxInstances { get; set; }

        public string Name { get; set; } = "unknown";
        public int ContextSize { get; set; } = 512;
        public int MainGpu { get; set; } = 0;
        public bool LowVram { get; set; } = false;
        public int GpuLayerCount { get; set; } = 20;
        public int Seed { get; set; } = 1686349486;
        public bool UseFp16Memory { get; set; } = true;
        public bool UseMemorymap { get; set; } = true;
        public bool UseMemoryLock { get; set; } = false;
        public bool Perplexity { get; set; } = false;
        public string ModelPath { get; set; }
        public string LoraAdapter { get; set; } = string.Empty;
        public string LoraBase { get; set; } = string.Empty;
        public int Threads { get; set; } = Math.Max(Environment.ProcessorCount / 2, 1);
        public int BatchSize { get; set; } = 512;
        public bool ConvertEosToNewLine { get; set; } = false;
        public bool EmbeddingMode { get; set; } = false;
        public float[] TensorSplits { get; set; } = new float[] { 0 };
        public int GroupedQueryAttention { get; set; } = 1;
        public float RmsNormEpsilon { get; set; } = 5e-6f;
        public float RopeFrequencyBase { get; set; } = 10000.0f;
        public float RopeFrequencyScale { get; set; } = 1.0f;
    }
}
