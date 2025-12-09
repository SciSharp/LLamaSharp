namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class ExpressionTemplateToken : TemplateToken
{
    public readonly Expression Expression;
    public ExpressionTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, Expression expression)
        : base(TemplateType.Expression, location, preSpace, postSpace)
    {
        ArgumentNullException.ThrowIfNull(expression);
        Expression = expression;
    }

    public override string ToString()
    {
        return $"Expression: {Expression}";
    }
}

