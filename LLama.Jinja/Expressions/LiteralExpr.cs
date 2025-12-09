namespace LLamaSharp.Jinja.Expressions;

internal sealed class LiteralExpr : Expression
{
    private readonly Value _value;

    public LiteralExpr(Location location, Value value)
        : base(location)
    {
        _value = value;
    }

    protected override Value DoEvaluate(Context context)
    {
        return _value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}

