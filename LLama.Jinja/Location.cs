namespace LLamaSharp.Jinja;

/// <summary>
/// A location in a template source.
/// </summary>
internal class Location
{
    public required string? Source { get; init; }
    public required int Position { get; init; }

    public override string ToString()
    {
        return LocationExtensions.ToString(Source, Position);
    }
}

