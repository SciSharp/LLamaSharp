using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

using llama_token = Int32;

/// <summary>
/// Information about a single beam in a beam search
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaBeamView
{
    private readonly unsafe llama_token* tokens;
    private readonly nint n_tokens;

    /// <summary>
    /// Cumulative beam probability (renormalized relative to all beams)
    /// </summary>
    public readonly float CumulativeProbability;

    /// <summary>
    /// Callback should set this to true when a beam is at end-of-beam.
    /// </summary>
    public bool EndOfBeam;

    /// <summary>
    /// Tokens in this beam
    /// </summary>
    public readonly Span<llama_token> Tokens
    {
        get
        {
            unsafe
            {
                if (n_tokens > int.MaxValue)
                    throw new InvalidOperationException("More than 2147483647 tokens is not supported");
                return new Span<llama_token>(tokens, (int)n_tokens);
            }
        }
    }
}