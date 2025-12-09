namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class ForNode : TemplateNode
{
    private readonly IReadOnlyCollection<string> _variableNames;
    private readonly Expression _iterable;
    private readonly Expression _condition;
    private readonly TemplateNode _body;
    private readonly bool _recursive;
    private readonly TemplateNode? _elseBody;

    public ForNode(Location location, IReadOnlyCollection<string> variableNames, Expression iterable, Expression condition, TemplateNode body, bool recursive, TemplateNode? elseBody)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(iterable);
        ArgumentNullException.ThrowIfNull(body);
        _variableNames = variableNames;
        _iterable = iterable;
        _condition = condition;
        _body = body;
        _recursive = recursive;
        _elseBody = elseBody;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        var iterableValue = _iterable.Evaluate(context);

        Value loopFunction(Context _, ArgumentsValue args)
        {
            if (args.Args.Count != 1 || args.Kwargs.Count > 0 || !args.Args[0].IsArray)
                throw new JinjaException("loop() expects exactly one positional iterable argument");
            var items = args.Args[0];
            Visit(items);
            return new Value();
        }

        void Visit(Value iter)
        {
            var filteredItems = Value.FromArray(); // Placeholder for filtering logic
            if (!iter.IsNull)
            {
                if (!iterableValue.IsIterable)
                    throw new JinjaException($"For loop must be iterable: {iterableValue.Dump()}");
                iterableValue.ForEach(item =>
                {
                    context.DestructuringAssign(_variableNames, item);
                    if (_condition is null || _condition.Evaluate(context).ToBoolean())
                        filteredItems.Add(item);
                });
            }
            if (filteredItems.Count == 0)
            {
                _elseBody?.Render(writer, context);
            }
            else
            {
                var loop = _recursive ? Value.Callable(loopFunction) : Value.Object();
                loop.Set("length", (long)filteredItems.Count);

                var cycleIndex = 0;
                loop.Set("cycle", (_, args) =>
                {
                    if (args.Args.Count == 0 || args.Kwargs.Count > 0)
                        throw new JinjaException("cycle() expects at least 1 positional argument and no named arg");
                    var item = args.Args[cycleIndex];
                    cycleIndex = (cycleIndex + 1) % args.Args.Count;
                    return item;
                });
                var loopContext = Context.Make(Value.Object(), context);
                loopContext.Set("loop", loop);
                for (var i = 0; i < filteredItems.Count; ++i)
                {
                    var item = filteredItems[i];
                    loopContext.DestructuringAssign(_variableNames, item);
                    loop.Set("index", (long)i + 1);
                    loop.Set("index0", (long)i);
                    loop.Set("revindex", (long)(filteredItems.Count - i));
                    loop.Set("revindex0", (long)(filteredItems.Count - i - 1));
                    loop.Set("length", (long)filteredItems.Count);
                    loop.Set("first", i == 0);
                    loop.Set("last", i == filteredItems.Count - 1);
                    loop.Set("previtem", i > 0 ? filteredItems[i - 1] : new Value());
                    loop.Set("nextitem", i < filteredItems.Count - 1 ? filteredItems[i + 1] : new Value());
                    try
                    {
                        _body.Render(writer, loopContext);
                    }
                    catch (LoopControlException e)
                    {
                        if (e.ControlType == LoopControlType.Break)
                            break;
                        if (e.ControlType == LoopControlType.Continue)
                            continue;
                    }
                }
            }
        }
        Visit(iterableValue);
    }

    public override string ToString()
    {
        return $"ForNode(Variables=[{string.Join(", ", _variableNames)}], Iterable={_iterable}, Condition={_condition}, Body={_body}, Recursive={_recursive}, ElseBody={_elseBody})";
    }
}

