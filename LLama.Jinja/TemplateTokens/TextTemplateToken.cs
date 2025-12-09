namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class TextTemplateToken : TemplateToken
{
    public readonly string Text;
    public TextTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, string text)
        : base(TemplateType.Text, location, preSpace, postSpace)
    {
        Text = text;
    }

    public override string ToString()
    {
        return $"Text: \"{Text}\"";
    }
}

