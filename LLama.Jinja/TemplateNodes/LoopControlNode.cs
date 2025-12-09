namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class LoopControlNode : TemplateNode
{
    private readonly LoopControlType _controlType;

    public LoopControlNode(Location location, LoopControlType controlType)
        : base(location)
    {
        _controlType = controlType;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        throw new LoopControlException(_controlType);
    }

    public override string ToString()
    {
        return $"{_controlType}";
    }
}

