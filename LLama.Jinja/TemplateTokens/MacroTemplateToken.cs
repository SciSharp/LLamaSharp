using LLamaSharp.Jinja.Expressions;

namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class MacroTemplateToken : TemplateToken
{
    public readonly VariableExpr Name;
    public readonly IReadOnlyList<(string Name, Expression Expression)> Parameters;

    public MacroTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, VariableExpr name, IReadOnlyList<(string Name, Expression Expression)> parameters)
        : base(TemplateType.Macro, location, preSpace, postSpace)
    {
        Name = name;
        Parameters = parameters;
    }

    public override string ToString()
    {
        return $"Macro: {Name}({string.Join(", ", Parameters.Select(p => $"{p.Name}={p.Expression}"))})";
    }
}

