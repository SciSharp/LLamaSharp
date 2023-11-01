using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Passed to beam_search_callback function.
/// Whenever 0 &lt; common_prefix_length, this number of tokens should be copied from any of the beams
/// (e.g. beams[0]) as they will be removed (shifted) from all beams in all subsequent callbacks.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaBeamsState
{
    /// <summary>
    /// The state of each individual beam
    /// </summary>
    private unsafe LLamaBeamView* beam_views;

    /// <summary>
    /// Number of elements in beam_views
    /// </summary>
    private nint n_beams;

    /// <summary>
    /// Current max length of prefix tokens shared by all beams.
    /// </summary>
    public ulong CommonPrefixLength;

    /// <summary>
    /// True iff this is the last callback invocation.
    /// </summary>
    public bool LastCall;

    /// <summary>
    /// The current state of each beam
    /// </summary>
    public Span<LLamaBeamView> Beams
    {
        get
        {
            unsafe
            {
                if (n_beams > int.MaxValue)
                    throw new InvalidOperationException("More than 2147483647 beams is not supported");
                return new Span<LLamaBeamView>(beam_views, (int)n_beams);
            }
        }
    }
}