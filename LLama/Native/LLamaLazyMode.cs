#nullable enable

namespace LLama.Native;

/// <summary>
/// Controls on-demand reading of model tensors when memory mapping is enabled.
/// </summary>
/// <remarks>llama_lazy_mode</remarks>
public enum LLamaLazyMode
{
    /// <summary>
    /// Read each tensor in full when loading the model.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Read marked tensors larger than 4 GiB on demand.
    /// </summary>
    Auto = 1,

    /// <summary>
    /// Read rows of marked tensors on demand.
    /// </summary>
    On = 2,
}
