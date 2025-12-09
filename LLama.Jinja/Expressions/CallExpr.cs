using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class CallExpr : Expression
{
    public readonly Expression Object;
    public readonly ArgumentsExpression Arguments;

    public CallExpr(Location location, Expression @object, ArgumentsExpression arguments)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(@object);
        Object = @object;
        Arguments = arguments;
    }

    protected override Value DoEvaluate(Context context)
    {
        var obj = Object.Evaluate(context);
        if (!obj.IsCallable)
            throw new JinjaException($"Object is not callable: {obj.Dump(2)}");
        var vargs = Arguments.Evaluate(context);
        return obj.Call(context, vargs);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Object}({Arguments})");
        return sb.ToString();
    }
}

