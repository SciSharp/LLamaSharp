using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class IfExpr : Expression
{
    private readonly Expression _condition;
    private readonly Expression _thenExpression;
    private readonly Expression? _elseExpression;

    public IfExpr(Location location, Expression condition, Expression thenExpression, Expression? elseExpression)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(thenExpression);
        _condition = condition;
        _thenExpression = thenExpression;
        _elseExpression = elseExpression;
    }

    protected override Value DoEvaluate(Context context)
    {
        if (_condition.Evaluate(context).ToBoolean())
            return _thenExpression.Evaluate(context);
        if (_elseExpression is not null)
            return _elseExpression.Evaluate(context);
        return Value.Null; // instead of null
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("if ");
        sb.Append(_condition);
        sb.Append(" then ");
        sb.Append(_thenExpression);
        if (_elseExpression is not null)
        {
            sb.Append(" else ");
            sb.Append(_elseExpression);
        }
        return sb.ToString();
    }
}

