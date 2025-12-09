namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class SetNode : TemplateNode
{
    private readonly string _namespace;
    private readonly IReadOnlyCollection<string> _variableNames;
    private readonly Expression _valueExpression;
    public SetNode(Location location, string @namespace, IReadOnlyCollection<string> variableNames, Expression valueExpression)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(valueExpression);
        _namespace = @namespace;
        _variableNames = variableNames;
        _valueExpression = valueExpression;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        if (!string.IsNullOrEmpty(_namespace))
        {
            if (_variableNames.Count != 1)
                throw new JinjaException("Namespaced set only supports a single variable name");
            var name = _variableNames.Single();
            var nsValue = context.Get(_namespace);
            if (!nsValue.IsObject)
                throw new JinjaException($"Namespace '{_namespace}' is not an object");
            nsValue.Set(name, _valueExpression.Evaluate(context));
        }
        else
        {
            var value = _valueExpression.Evaluate(context);
            context.DestructuringAssign(_variableNames, value);
        }
    }

    public override string ToString()
    {
        return $"SetNode(Namespace={_namespace}, VariableNames=[{string.Join(", ", _variableNames)}], ValueExpression={_valueExpression})";
    }
}

