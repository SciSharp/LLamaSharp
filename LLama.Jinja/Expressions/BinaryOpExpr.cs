using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class BinaryOpExpr : Expression
{
    public enum Op
    {
        StrConcat,
        Add,
        Sub,
        Mul,
        MulMul,
        Div,
        DivDiv,
        Mod,
        Eq,
        Ne,
        Lt,
        Gt,
        Le,
        Ge,
        And,
        Or,
        In,
        NotIn,
        Is,
        IsNot
    }
    private readonly Op _operator;
    private readonly Expression _left;
    private readonly Expression _right;
    public BinaryOpExpr(Location location, Op @operator, Expression left, Expression right)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        _operator = @operator;
        _left = left;
        _right = right;
    }
    protected override Value DoEvaluate(Context context)
    {
        var l = _left.Evaluate(context);

        Value DoEval(Value l)
        {
            if (_operator == Op.Is || _operator == Op.IsNot)
            {
                if (_right is not VariableExpr t)
                    throw new JinjaException("Right operand of 'is' operator must be a variable");

                bool Eval()
                {
                    var name = t.Name;
                    return name switch
                    {
                        "none" => l.IsNull,
                        "boolean" => l.IsBoolean,
                        "integer" => l.IsInteger,
                        "float" => l.IsDouble,
                        "number" => l.IsNumber,
                        "string" => l.IsString,
                        "mapping" => l.IsObject,
                        "iterable" => l.IsIterable,
                        "sequence" => l.IsArray,
                        "defined" => !l.IsNull,
                        "true" => l.ToBoolean(),
                        "false" => !l.ToBoolean(),
                        _ => throw new JinjaException($"Unknown type for 'is' operator {name}"),
                    };
                }
                ;
                var value = Eval();
                return new Value(_operator == Op.Is ? value : !value);
            }


            else if (_operator == Op.And)
            {
                if (!l.ToBoolean())
                    return new Value(false);
                return _right.Evaluate(context);
            }
            else if (_operator == Op.Or)
            {
                if (l.ToBoolean())
                    return l;
                return _right.Evaluate(context);
            }
            else
            {
                var r = _right.Evaluate(context);

                static bool In(Value value, Value container)
                {
                    return ((container.IsArray || container.IsObject) && container.Contains(value)) ||
                        (value.IsString && container.IsString &&
                          container.ToString().Contains(value.ToString()));
                }

                return _operator switch
                {
                    Op.StrConcat => new Value(l.ToString() + r.ToString()),
                    Op.Add => l + r,
                    Op.Sub => l - r,
                    Op.Mul => l * r,
                    Op.MulMul => Value.Pow(l, r),
                    Op.Div => l / r,
                    Op.DivDiv => Value.FloorDiv(l, r),
                    Op.Mod => l % r,
                    Op.Eq => new Value(l.Equals(r)),
                    Op.Ne => new Value(!l.Equals(r)),
                    Op.Lt => new Value(l < r),
                    Op.Gt => new Value(l > r),
                    Op.Le => new Value(l <= r),
                    Op.Ge => new Value(l >= r),
                    Op.In => new Value(In(l, r)),
                    Op.NotIn => new Value(!In(l, r)),
                    _ => throw new JinjaException("Unknown binary operator"),
                };
            }
        }
        ;

        if (l.IsCallable)
        {
            return Value.Callable((callContext, args) =>
            {
                var ll = l.Call(callContext, args);
                return DoEval(ll);
            });
        }
        else
            return DoEval(l);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"({_left}) {_operator} ({_right})");
        return sb.ToString();
    }
}

