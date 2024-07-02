namespace LLama.Model;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
/// Types of supported model files
/// </summary>
public enum ModelFileType
{
    GGUF
}

/// <summary>
/// Metadata about available models
/// </summary>
public class ModelFileMetadata
{
    public string ModelFileName { get; init; } = string.Empty;
    public string ModelFileUri { get; init; } = string.Empty;
    public ModelFileType ModelType { get; init; }
    public long ModelFileSizeInBytes { get; init; } = 0;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

