using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class FilterExpr : Expression
{
    private readonly IList<Expression> _parts;

    public FilterExpr(Location location, IList<Expression> parts)
        : base(location)
    {
        _parts = parts;
    }

    protected override Value DoEvaluate(Context context)
    {
        var result = Value.Null;
        var first = true;
        foreach (var part in _parts)
        {
            if (part is null)
                throw new JinjaException("FilterExpr.part is null");
            if (first)
            {
                first = false;
                result = part.Evaluate(context);
            }
            else
            {
                if (part is CallExpr ce)
                {
                    var target = ce.Object.Evaluate(context);
                    var args = ce.Arguments.Evaluate(context);
                    args.Args.Insert(0, result);
                    result = target.Call(context, args);
                }
                else
                {
                    var callable = part.Evaluate(context);
                    var args = new ArgumentsValue();
                    args.Args.Insert(0, result);
                    result = callable.Call(context, args);
                }
            }
        }
        return result;
    }

    public void Prepend(Expression expression)
    {
        _parts.Insert(0, expression);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('{');
        foreach (var e in _parts)
            sb.Append($"{e}, ");
        if (_parts.Any())
            sb.Length -= 2; // Remove trailing comma and space
        sb.Append('}');
        return sb.ToString();
    }

}

