namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class EndGenerationTemplateToken : TemplateToken
{
    public EndGenerationTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.EndGeneration, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "EndGeneration";
    }
}

