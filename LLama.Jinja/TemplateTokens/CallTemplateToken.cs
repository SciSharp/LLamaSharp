namespace LLamaSharp.Jinja.TemplateTokens;

internal sealed class CallTemplateToken : TemplateToken
{
    public readonly Expression Callee;
    public CallTemplateToken(Location location, SpaceHandling preSpace, SpaceHandling postSpace, Expression callee)
        : base(TemplateType.Call, location, preSpace, postSpace)
    {
        Callee = callee;
    }

    public override string ToString()
    {
        return $"CallTemplateToken(Callee={Callee})";
    }
}

