namespace LLamaSharp.Jinja.TemplateNodes;

internal sealed class TextNode : TemplateNode
{
    private readonly string _text;
    public TextNode(Location location, string text)
        : base(location)
    {
        _text = text;
    }

    private protected override void DoRender(StringWriter writer, Context context)
    {
        writer.Write(_text);
    }

    public override string ToString()
    {
        return $"TextNode(\"{_text}\")";
    }
}

