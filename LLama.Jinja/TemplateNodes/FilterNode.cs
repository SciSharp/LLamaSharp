namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class FilterNode : TemplateNode
{
    private readonly Expression _filter;
    private readonly TemplateNode _body;

    public FilterNode(Location location, Expression filter, TemplateNode body)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(body);
        _filter = filter;
        _body = body;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        var filterValue = _filter.Evaluate(context);
        if (!filterValue.IsCallable)
            throw new JinjaException($"Filter must be a callable: {filterValue.Dump()}");
        var renderedBody = _body.Render(context);
        var filterArgs = new ArgumentsValue();
        filterArgs.Args.Add(new Value(renderedBody));
        var result = filterValue.Call(context, filterArgs);
        writer.Write(result.ToString());
    }

    public override string? ToString()
    {
        return $"{_filter} | {_body}";
    }
}

