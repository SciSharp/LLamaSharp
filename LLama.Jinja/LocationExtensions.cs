using System.Text;

namespace LLamaSharp.Jinja;

internal static class LocationExtensions
{
    private const int MaxPrefixLength = 80;
    private const int MaxSuffixLength = 40;
    private const string Ellipsis = "...";

    public static string ToString(string? source, int position)
    {
        if (source is null)
            return string.Empty;

        var sb = new StringBuilder();
        var start = position;
        while (start > 0 && position - start < MaxPrefixLength && source![start - 1] != '\n')
            --start;
        if (start > 0 && source![start - 1] != '\n')
            sb.Append(Ellipsis);
        sb.Append(source, start, position - start);

        var end = position;
        while (end < source.Length && end - position < MaxSuffixLength && source[end] != '\n')
            ++end;
        if (end < source.Length && source[end] != '\n')
            sb.Append(Ellipsis);
        sb.Append(source, position, end - position);
        sb.AppendLine();

        if (start > 1)
            sb.Append(' ', start - 1);
        sb.Append('^');
        sb.AppendLine();
        return sb.ToString();
    }
}

