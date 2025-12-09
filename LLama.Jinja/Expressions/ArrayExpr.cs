using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class ArrayExpr : Expression
{
    private readonly IEnumerable<Expression> _elements;

    public ArrayExpr(Location location, IEnumerable<Expression> elements)
        : base(location)
    {
        _elements = elements;
    }

    protected override Value DoEvaluate(Context context)
    {
        var result = Value.FromArray();
        foreach (var e in _elements)
        {
            if (e is null)
                throw new JinjaException("Array element is null");
            result.Add(e.Evaluate(context));
        }
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        foreach (var element in _elements)
            sb.Append($"{element}, ");
        if (_elements.Any())
            sb.Length -= 2; // Remove trailing comma and space
        sb.Append(']');
        return sb.ToString();
    }
}

