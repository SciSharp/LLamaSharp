using LLamaSharp.Jinja.Expressions;

namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class CallNode : TemplateNode
{
    private readonly Expression _expression;
    private readonly TemplateNode _body;

    public CallNode(Location location, Expression expression, TemplateNode body)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(body);
        _expression = expression;
        _body = body;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        var caller = Value.Callable((_, argumentsValue) => new Value(_body.Render(context)));

        context.Set("caller", caller);

        if (_expression is not CallExpr callExpr)
            throw new JinjaException("Invalid call block syntax - expected function call");

        var function = callExpr.Object.Evaluate(context);
        if (!function.IsCallable)
            throw new JinjaException($"Call target must be callable: {function.Dump()}");
        var args = callExpr.Arguments.Evaluate(context);
        var result = function.Call(context, args);
        writer.Write(result.ToString());
    }

    public override string ToString()
    {
        return $"{_expression} {{{_body}}}";
    }
}

