namespace LLama.Abstractions
{
    /// <summary>
    /// Convenience interface for implementing both type of parameters.
    /// </summary>
    /// <remarks>Mostly exists for backwards compatibility reasons, when these two were not split.</remarks>
    public interface ILLamaParams
        : IModelParams, IContextParams
    {
    }
}
