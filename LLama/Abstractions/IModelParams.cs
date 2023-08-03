namespace LLama.Abstractions
{
    public interface IModelParams
    {
        int BatchSize { get; set; }
        int ContextSize { get; set; }
        bool ConvertEosToNewLine { get; set; }
        bool EmbeddingMode { get; set; }
        int GpuLayerCount { get; set; }
        int GroupedQueryAttention { get; set; }
        string LoraAdapter { get; set; }
        string LoraBase { get; set; }
        bool LowVram { get; set; }
        int MainGpu { get; set; }
        string ModelPath { get; set; }
        string Name { get; set; }
        bool Perplexity { get; set; }
        float RmsNormEpsilon { get; set; }
        float RopeFrequencyBase { get; set; }
        float RopeFrequencyScale { get; set; }
        int Seed { get; set; }
        float[] TensorSplits { get; set; }
        int Threads { get; set; }
        bool UseFp16Memory { get; set; }
        bool UseMemoryLock { get; set; }
        bool UseMemorymap { get; set; }
    }
}