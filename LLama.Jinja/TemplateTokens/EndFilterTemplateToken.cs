namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndFilterTemplateToken : TemplateToken
{
    public EndFilterTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndFilter, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndFilter";
    }
}

