using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class DictExpr : Expression
{
    private readonly IEnumerable<(Expression Key, Expression Value)> _elements;

    public DictExpr(Location location, IEnumerable<(Expression Key, Expression Value)> elements)
        : base(location)
    {
        foreach (var e in elements)
        {
            ArgumentNullException.ThrowIfNull(e.Key);
            ArgumentNullException.ThrowIfNull(e.Value);
        }
        _elements = elements;
    }

    protected override Value DoEvaluate(Context context)
    {
        var result = Value.Object();
        foreach (var e in _elements)
            result.Set(e.Key.Evaluate(context), e.Value.Evaluate(context));
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('{');
        foreach (var e in _elements)
            sb.Append($"{e.Key} : {e.Value}, ");
        if (_elements.Any())
            sb.Length -= 2; // Remove trailing comma and space
        sb.Append('}');
        return sb.ToString();
    }
}

