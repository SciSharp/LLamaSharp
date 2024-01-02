using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Information about a single beam in a beam search
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaBeamView
{
    private unsafe LLamaToken* tokens;
    private nint n_tokens;

    /// <summary>
    /// Cumulative beam probability (renormalized relative to all beams)
    /// </summary>
    public float CumulativeProbability;

    /// <summary>
    /// Callback should set this to true when a beam is at end-of-beam.
    /// </summary>
    public bool EndOfBeam;

    /// <summary>
    /// Tokens in this beam
    /// </summary>
    public readonly Span<LLamaToken> Tokens
    {
        get
        {
            unsafe
            {
                if (n_tokens > int.MaxValue)
                    throw new InvalidOperationException("More than 2147483647 tokens is not supported");
                return new Span<LLamaToken>(tokens, (int)n_tokens);
            }
        }
    }
}