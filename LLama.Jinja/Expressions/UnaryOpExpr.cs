namespace LLamaSharp.Jinja.Expressions;

internal sealed class UnaryOpExpr : Expression
{
    public enum Op
    {
        Plus,
        Minus,
        LogicalNot,
        Expansion,
        ExpansionDict
    }
    public readonly Op Operator;
    public readonly Expression Expression;

    public UnaryOpExpr(Location location, Op @operator, Expression expression)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(expression);
        Operator = @operator;
        Expression = expression;
    }

    protected override Value DoEvaluate(Context context)
    {
        var e = Expression.Evaluate(context);
        return Operator switch
        {
            Op.Plus => e,
            Op.Minus => -e,
            Op.LogicalNot => new Value(!e.ToBoolean()),
            Op.Expansion or Op.ExpansionDict => throw new JinjaException("Expansion operator is only supported in function calls and collections"),
            _ => throw new JinjaException("Unknown unary operator"),
        };
    }

    public override string ToString()
    {
        return $"{Operator} {Expression}";
    }
}

