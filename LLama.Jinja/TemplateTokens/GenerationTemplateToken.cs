namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class GenerationTemplateToken : TemplateToken
{
    public GenerationTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace)
        : base(TemplateType.Generation, location, preSpace, postSpace)
    {
    }

    public override string ToString()
    {
        return "Generation";
    }
}

