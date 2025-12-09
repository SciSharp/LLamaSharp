namespace LLamaSharp.Jinja;

public readonly struct Options
{
    /// <summary>
    /// Removes the first newline after a block. This is Jinja's trim_blocks.
    /// </summary>
    public required bool TrimBlocks { get; init; }

    /// <summary>
    /// Removes leading whitespace on the line of the block. This is Jinja's lstrip_blocks.
    /// </summary>
    public required bool LStripBlocks { get; init; }

    /// <summary>
    /// Don't remove last newline. This is Jinja's keep_trailing_newline.
    /// </summary>
    public required bool KeepTrailingNewline { get; init; }

    public static Options Default => new()
    {
        TrimBlocks = false,
        LStripBlocks = false,
        KeepTrailingNewline = false,
    };
};

