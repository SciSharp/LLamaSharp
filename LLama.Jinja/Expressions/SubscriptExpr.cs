using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class SubscriptExpr : Expression
{
    private readonly Expression _base;
    private readonly Expression _index;
    public SubscriptExpr(Location location, Expression value, Expression index)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(index);
        _base = value;
        _index = index;
    }
    protected override Value DoEvaluate(Context context)
    {
        var targetValue = _base.Evaluate(context);
        if (_index is SliceExpr slice)
        {
            var len = targetValue.Count;

            long Wrap(long i)
            {
                return i < 0 ? i + len : i;
            }

            var step = slice.Step is null ? 1 : slice.Step.Evaluate(context).Get<long>();
            if (step == 0)
                throw new JinjaException("Slice step cannot be zero");
            var start = slice.Start is null ? (step < 0 ? len - 1 : 0) : Wrap(slice.Start.Evaluate(context).Get<long>());
            var end = slice.End is null ? (step < 0 ? -1 : len) : Wrap(slice.End.Evaluate(context).Get<long>());
            if (targetValue.IsString)
            {
                var s = targetValue.Get<string>();
                var sb = new StringBuilder();
                if (start < end && step == 1)
                    sb.Append(s, (int)start, (int)(end - start));
                else
                    for (var i = start; step > 0 ? i < end : i > end; i += step)
                        sb.Append(s[(int)i]);
                return new Value(sb.ToString());
            }
            else if (targetValue.IsArray)
            {
                var result = Value.FromArray();
                for (var i = start; step > 0 ? i < end : i > end; i += step)
                    result.Add(targetValue[(int)i]);
                return result;
            }
            else
                throw new JinjaException(targetValue.IsNull ? "Cannot subscript null" : "Subscripting only supported on arrays and strings");
        }
        else
        {
            var indexValue = _index.Evaluate(context);
            if (targetValue.IsNull)
            {
                if (_base is VariableExpr varExpr)
                    throw new JinjaException($"'{varExpr.Name}' is {(context.Contains(varExpr.Name) ? "null" : "not defined")}");
                throw new JinjaException($"Trying to access property '{indexValue.Dump()}' on null!");
            }
            return targetValue.Get(indexValue);
        }
    }

    public override string ToString()
    {
        return $"{_base}[{_index}]";
    }
}

