namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class SetTemplateNode : TemplateNode
{
    private readonly string _name;
    private readonly TemplateNode _templateValues;

    public SetTemplateNode(Location location, string name, TemplateNode templateValues)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(templateValues);
        _name = name;
        _templateValues = templateValues;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        context.Set(_name, _templateValues.Render(context));
    }

    public override string ToString()
    {
        return $"SetTemplateNode(Name={_name}, Values={_templateValues})";
    }
}

