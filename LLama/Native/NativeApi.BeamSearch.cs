using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

public static partial class NativeApi
{
    /// <summary>
    /// Type of pointer to the beam_search_callback function.
    /// </summary>
    /// <param name="callback_data">callback_data is any custom data passed to llama_beam_search, that is subsequently passed back to beam_search_callbac</param>
    /// <param name="state"></param>
    public delegate void LLamaBeamSearchCallback(IntPtr callback_data, LLamaBeamsState state);

    /// <summary>Deterministically returns entire sentence constructed by a beam search.</summary>
    /// <param name="ctx">Pointer to the llama_context.</param>
    /// <param name="callback">Invoked for each iteration of the beam_search loop, passing in beams_state.</param>
    /// <param name="callback_data">A pointer that is simply passed back to callback.</param>
    /// <param name="n_beams">Number of beams to use.</param>
    /// <param name="n_past">Number of tokens already evaluated.</param>
    /// <param name="n_predict">Maximum number of tokens to predict. EOS may occur earlier.</param>
    /// <param name="n_threads">Number of threads.</param>
    [DllImport(libraryName, EntryPoint = "llama_beam_search", CallingConvention = CallingConvention.Cdecl)]
    public static extern void llama_beam_search(SafeLLamaContextHandle ctx, LLamaBeamSearchCallback callback, IntPtr callback_data, ulong n_beams, int n_past, int n_predict, int n_threads);
}