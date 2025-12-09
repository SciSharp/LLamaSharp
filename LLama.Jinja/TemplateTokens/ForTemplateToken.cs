namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class ForTemplateToken : TemplateToken
{
    public readonly IReadOnlyCollection<string> VariableNames;
    public readonly Expression Iterable;
    public readonly Expression Condition;
    public readonly bool Recursive;

    public ForTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, IReadOnlyCollection<string> variableNames, Expression iterable, Expression condition, bool recursive)
        : base(TemplateType.For, location, preSpace, postSpace)
    {
        VariableNames = variableNames;
        Iterable = iterable;
        Condition = condition;
        Recursive = recursive;
    }

    public override string ToString()
    {
        return $"For(Variables=[{string.Join(", ", VariableNames)}], Iterable={Iterable}, Condition={Condition}, Recursive={Recursive})";
    }
}

