using System.Text;

namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class SequenceNode : TemplateNode
{
    private readonly IEnumerable<TemplateNode> _children;

    public SequenceNode(Location location, IEnumerable<TemplateNode> children)
        : base(location)
    {
        _children = children;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        foreach (var child in _children)
            child.Render(writer, context);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var child in _children)
            sb.AppendLine($"{child}; ");
        if (sb.Length > 2)
            sb.Length -= 2; // Remove last ", "
        return sb.ToString();
    }
}

