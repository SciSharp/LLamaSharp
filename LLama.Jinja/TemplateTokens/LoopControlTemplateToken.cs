namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class LoopControlTemplateToken : TemplateToken
{
    public readonly LoopControlType ControlType;
    public LoopControlTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, LoopControlType controlType)
        : base(TemplateType.Break, location, preSpace, postSpace)
    {
        ControlType = controlType;
    }

    public override string ToString()
    {
        return $"LoopToken({ControlType})";
    }
}

