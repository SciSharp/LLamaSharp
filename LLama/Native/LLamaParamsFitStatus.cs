namespace LLama.Native;

public enum LLamaParamsFitStatus
{
    /// <summary>
    /// Found allocations that are projected to fit
    /// </summary>
    LLAMA_PARAMS_FIT_STATUS_SUCCESS = 0,

    /// <summary>
    /// Could not find allocations that are projected to fit
    /// </summary>
    LLAMA_PARAMS_FIT_STATUS_FAILURE = 1,

    /// <summary>
    /// A hard error occurred, e.g. because no model could be found at the specified path
    /// </summary>
    LLAMA_PARAMS_FIT_STATUS_ERROR = 2,
}