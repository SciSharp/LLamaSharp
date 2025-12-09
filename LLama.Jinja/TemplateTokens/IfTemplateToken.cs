namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class IfTemplateToken : TemplateToken
{
    public readonly Expression Condition;
    public IfTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, Expression condition)
        : base(TemplateType.If, location, preSpace, postSpace)
    {
        ArgumentNullException.ThrowIfNull(condition);
        Condition = condition;
    }

    public override string ToString()
    {
        return $"IfToken(Condition: {Condition})";
    }
}

