namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndCallTemplateToken : TemplateToken
{
    public EndCallTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndCall, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "endcall";
    }
}

