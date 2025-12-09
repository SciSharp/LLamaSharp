namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndForTemplateToken : TemplateToken
{
    public EndForTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndFor, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndFor";
    }
}

