namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class IfNode : TemplateNode
{
    private readonly IEnumerable<(Expression? Expression, TemplateNode TemplateNode)> _cascade;

    public IfNode(Location location, IEnumerable<(Expression? Expression, TemplateNode TemplateNode)> cascade)
        : base(location)
    {
        _cascade = cascade;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        foreach (var branch in _cascade)
        {
            var enterBranch = true;
            if (branch.Expression is not null)
                enterBranch = branch.Expression.Evaluate(context).ToBoolean();
            if (enterBranch)
            {
                if (branch.TemplateNode is null)
                    throw new JinjaException($"If branch at {Location} has no template node.");
                branch.TemplateNode.Render(writer, context);
                return;
            }
        }
    }

    public override string ToString()
    {
        return $"IfNode(Location: {Location}, Branches: {_cascade.Count()})";
    }
}

