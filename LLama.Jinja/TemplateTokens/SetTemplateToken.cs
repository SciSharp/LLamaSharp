namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class SetTemplateToken : TemplateToken
{
    public readonly string Namespace;
    public readonly IReadOnlyCollection<string> VariableNames;
    public readonly Expression Value;

    public SetTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, string @namespace, IReadOnlyCollection<string> variableNames, Expression value)
        : base(TemplateType.Set, location, preSpace, postSpace)
    {
        Namespace = @namespace;
        VariableNames = variableNames;
        Value = value;
    }

    public override string ToString()
    {
        return $"Set: {Namespace}.{string.Join(", ", VariableNames)} = {Value}";
    }
}

