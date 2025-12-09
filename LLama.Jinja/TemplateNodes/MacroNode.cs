using LLamaSharp.Jinja.Expressions;

namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class MacroNode : TemplateNode
{
    private readonly VariableExpr _macroName;
    private readonly IReadOnlyList<(string Name, Expression Expression)> _parameters;
    private readonly TemplateNode _body;
    private readonly Dictionary<string, int> _namedParameterPositions;

    public MacroNode(Location location, VariableExpr name, IReadOnlyList<(string Name, Expression)> parameters, TemplateNode body)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(body);
        _macroName = name;
        _parameters = parameters;
        _body = body;
        _namedParameterPositions = [];
        for (var i = 0; i < parameters.Count; ++i)
        {
            var n = parameters[i].Name;
            if (!string.IsNullOrEmpty(n))
                _namedParameterPositions[n] = i;
        }
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        var callable = Value.Callable((callContext, args) =>
        {
            var executionContext = Context.Make(Value.Object(), context);
            if (callContext.Contains("caller"))
                executionContext.Set("caller", callContext.Get("caller"));

            var paramSet = new bool[_parameters.Count];
            for (var i = 0; i < args.Args.Count; ++i)
            {
                var arg = args.Args[i];
                if (i >= _parameters.Count)
                    throw new JinjaException($"Too many positional arguments for macro {_macroName.Name}");
                paramSet[i] = true;
                var paramName = _parameters[i].Name;
                executionContext.Set(paramName, arg);
            }
            foreach (var (name, value) in args.Kwargs)
            {
                if (!_namedParameterPositions.TryGetValue(name, out var position))
                    throw new JinjaException($"Unknown parameter name for macro '{_macroName.Name}': {name}");
                executionContext.Set(name, value);
                paramSet[position] = true;  // shouldn't we test first if this is already true and report it as "multiple values"?
            }

            for (var i = 0; i < _parameters.Count; ++i)
                if (!paramSet[i] && _parameters[i].Expression is not null)
                {
                    var value = _parameters[i].Expression.Evaluate(callContext);
                    executionContext.Set(_parameters[i].Name, value);
                }

            return new Value(_body.Render(executionContext));
        });
        context.Set(_macroName.Name, callable);
    }

    public override string ToString()
    {
        return $"MacroNode(Name={_macroName}, Parameters=[{string.Join(", ", _parameters.Select(p => p.Name))}], Body={_body})";
    }
}

