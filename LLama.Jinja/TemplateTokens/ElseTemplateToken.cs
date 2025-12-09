namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class ElseTemplateToken : TemplateToken
{
    public ElseTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.Else, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "ElseTemplateToken";
    }
}

