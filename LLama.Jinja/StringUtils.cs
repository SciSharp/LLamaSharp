using System.Text;

namespace LLamaSharp.Jinja;

internal static class StringUtils
{
    public static string Strip(string s, string chars = "", bool left = true, bool right = true)
    {
        if (string.IsNullOrEmpty(chars))
            chars = " \t\n\r";
        var start = 0;
        var end = s.Length;
        if (left)
            while (start < end && chars.Contains(s[start]))
                start++;
        if (right)
            while (end > start && chars.Contains(s[end - 1]))
                end--;
        return s[start..end];
    }

    public static List<string> Split(string s, string sep)
    {
        var result = new List<string>();
        var start = 0;
        while (start < s.Length)
        {
            var index = s.IndexOf(sep, start);
            if (index < 0)
            {
                result.Add(s[start..]);
                break;
            }
            result.Add(s[start..index]);
            start = index + sep.Length;
        }
        return result;
    }

    public static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        if (s.Length == 1)
            return s.ToUpperInvariant();
        return char.ToUpperInvariant(s[0]) + s[1..];
    }

    public static string HtmlEscape(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
            switch (ch)
            {
                case '&':
                    sb.Append("&amp;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '"':
                    sb.Append("&#34;");
                    break;
                case '\'':
                    sb.Append("&apos;");
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        return sb.ToString();
    }
}

