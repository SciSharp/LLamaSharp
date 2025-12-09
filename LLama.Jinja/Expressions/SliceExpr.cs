namespace LLamaSharp.Jinja.Expressions;

internal sealed class SliceExpr : Expression
{
    public readonly Expression Start;
    public readonly Expression End;
    public readonly Expression Step;

    public SliceExpr(Location location, Expression start, Expression end, Expression step)
        : base(location)
    {
        Start = start;
        End = end;
        Step = step;
    }

    protected override Value DoEvaluate(Context context)
    {
        throw new NotImplementedException("SliceExpr not implemented");
    }

    public override string ToString()
    {
        return $"{Start}:{End}:{Step}";
    }
}

