namespace LLama.Model;

/// <summary>
/// Types of supported model files
/// </summary>
public enum ModelFileType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or membe
    GGUF
}

/// <summary>
/// Metadata about available models
/// </summary>
public class ModelFileMetadata
{
    public string FileName { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public ModelFileType ModelType { get; init; }
    public long SizeInBytes { get; init; } = 0;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

