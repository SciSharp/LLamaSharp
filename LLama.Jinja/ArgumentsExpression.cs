using LLamaSharp.Jinja.Expressions;
using System.Text;

namespace LLamaSharp.Jinja;

internal sealed class ArgumentsExpression
{
    public readonly List<Expression> Args;
    public readonly List<(string Name, Expression Expression)> KwArgs;

    public ArgumentsExpression()
    {
        Args = [];
        KwArgs = [];
    }

    public ArgumentsValue Evaluate(Context context)
    {
        var result = new ArgumentsValue();
        foreach (var arg in Args)
        {
            if (arg is UnaryOpExpr unary)
            {
                if (unary.Operator == UnaryOpExpr.Op.Expansion)
                {
                    var array = unary.Expression.Evaluate(context);
                    if (!array.IsArray)
                        throw new JinjaException("Expansion operator only supported on arrays");
                    array.ForEach(item => result.Args.Add(item));
                    continue;
                }
                else if (unary.Operator == UnaryOpExpr.Op.ExpansionDict)
                {
                    var dict = unary.Expression.Evaluate(context);
                    if (!dict.IsObject)
                        throw new JinjaException("ExpansionDict operator only supported on objects");
                    dict.ForEach(key => result.Kwargs.Add((key.Get<string>(), dict.Get(key))));
                    continue;
                }
            }
            result.Args.Add(arg.Evaluate(context));
        }
        foreach (var (name, expr) in KwArgs)
            result.Kwargs.Add((name, expr.Evaluate(context)));
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("Args: [");
        for (var i = 0; i < Args.Count; i++)
        {
            sb.Append(Args[i].ToString());
            if (i < Args.Count - 1)
                sb.Append(", ");
        }
        sb.Append("], KwArgs: {");
        for (var i = 0; i < KwArgs.Count; i++)
        {
            sb.Append(KwArgs[i].Name);
            sb.Append(": ");
            sb.Append(KwArgs[i].Expression.ToString());
            if (i < KwArgs.Count - 1)
                sb.Append(", ");
        }
        return sb.ToString();
    }
}

