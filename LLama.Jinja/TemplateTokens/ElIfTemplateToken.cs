namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class ElIfTemplateToken : TemplateToken
{
    public readonly Expression Condition;
    public ElIfTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, Expression condition)
        : base(TemplateType.Elif, location, preSpace, postSpace)
    {
        ArgumentNullException.ThrowIfNull(condition);
        Condition = condition;
    }

    public override string ToString()
    {
        return $"ElIf(Condition: {Condition})";
    }
}

