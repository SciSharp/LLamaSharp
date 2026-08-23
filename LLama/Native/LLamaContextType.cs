namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_context_type</remarks>
public enum LLamaContextType
{
    /// <summary>
    /// Default context type
    /// </summary>
    Default = 0,
    
    /// <summary>
    /// Multi token prediction context
    /// </summary>
    Mtp = 1,
}