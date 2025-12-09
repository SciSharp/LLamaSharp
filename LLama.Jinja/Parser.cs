using System.Text.RegularExpressions;

namespace LLamaSharp.Jinja;

public sealed partial class Parser
{
    public static string NormalizeNewlines(string s)
    {
        // Replace Windows CRLF with LF
        return NewlinesRegex().Replace(s, "\n");
    }

    public static TemplateNode Parse(string templateStr, Options options)
    {
        var normalized = NormalizeNewlines(templateStr);
        var parser = new Tokenizer(normalized);
        var tokens = parser.Tokenize();
        var templateNodeParser = new TemplateNodeParser(templateStr, options, tokens);
        return templateNodeParser.ParseTemplate(fully: true);
    }

    [GeneratedRegex("\r\n")]
    private static partial Regex NewlinesRegex();
}
