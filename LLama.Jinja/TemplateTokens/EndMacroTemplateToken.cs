namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndMacroTemplateToken : TemplateToken
{
    public EndMacroTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndMacro, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndMacro";
    }
}

