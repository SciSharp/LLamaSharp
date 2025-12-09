namespace LLamaSharp.Jinja.Expressions;

internal sealed class VariableExpr : Expression
{
    public readonly string Name;

    public VariableExpr(Location location, string name)
        : base(location)
    {
        Name = name;
    }
    protected override Value DoEvaluate(Context context)
    {
        return context.Get(Name);
    }

    public override string ToString()
    {
        return Name;
    }
}

