namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class FilterTemplateToken : TemplateToken
{
    public readonly Expression Filter;
    public FilterTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, Expression filter)
        : base(TemplateType.Filter, location, preSpace, postSpace)
    {
        Filter = filter;
    }

    public override string ToString()
    {
        return $"Filter({Filter})";
    }
}

