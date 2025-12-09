namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndSetTemplateToken : TemplateToken
{
    public EndSetTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndSet, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndSet";
    }
}

