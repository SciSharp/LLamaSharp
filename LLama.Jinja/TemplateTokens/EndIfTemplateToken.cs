namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndIfTemplateToken : TemplateToken
{
    public EndIfTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndIf, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndIf";
    }
}

