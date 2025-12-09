namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class ExpressionNode : TemplateNode
{
    private readonly Expression _expression;
    public ExpressionNode(Location location, Expression expression)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(expression);
        _expression = expression;
    }
    private protected override void DoRender(StringWriter writer, Context context)
    {
        var value = _expression.Evaluate(context);
        if (value.IsString)
            writer.Write(value.Get<string>());
        else if (value.IsBoolean)
            writer.Write(value.Get<bool>() ? "True" : "False");
        else if (!value.IsNull)
            writer.Write(value.Dump());
    }

    public override string? ToString()
    {
        return _expression.ToString();
    }
}

