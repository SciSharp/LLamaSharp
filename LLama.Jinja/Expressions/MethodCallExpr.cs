using System.Text;

namespace LLamaSharp.Jinja.Expressions;

internal sealed class MethodCallExpr : Expression
{
    private readonly Expression _object;
    private readonly VariableExpr _method;
    private readonly ArgumentsExpression _arguments;

    public MethodCallExpr(Location location, Expression @object, VariableExpr method, ArgumentsExpression arguments)
        : base(location)
    {
        ArgumentNullException.ThrowIfNull(@object);
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(arguments);
        _object = @object;
        _method = method;
        _arguments = arguments;
    }


    protected override Value DoEvaluate(Context context)
    {
        var obj = _object.Evaluate(context);
        var vargs = _arguments.Evaluate(context);
        if (obj.IsNull)
            throw new JinjaException($"Trying to call method '{_method.Name}' on null");
        if (obj.IsArray)
        {
            switch (_method.Name)
            {
                case "append":
                    vargs.ExpectArgs("append method", (1, 1));
                    obj.Add(vargs.Args[0]);
                    return new Value();
                case "pop":
                    vargs.ExpectArgs("pop method", (0, 1));
                    return obj.Pop(vargs.Args.Count == 0 ? new Value() : vargs.Args[0]);
                case "insert":
                    vargs.ExpectArgs("insert method", (2, 2));
                    var index = vargs.Args[0].Get<long>();
                    if (index < 0 || index > obj.Count)
                        throw new JinjaException("Index out of range for insert method");
                    obj.Insert((int)index, vargs.Args[1]);
                    return new Value();
            }
        }
        else if (obj.IsObject)
        {
            switch (_method.Name)
            {
                case "items":
                    {
                        vargs.ExpectArgs("items method", (0, 0));
                        var result = Value.FromArray();
                        obj.ForEach(key =>
                        {
                            result.Add(Value.FromArray([key, obj.Get(key)]));
                        });
                        return result;
                    }

                case "pop":
                    vargs.ExpectArgs("pop method", (1, 1));
                    return obj.Pop(vargs.Args[0]);
                case "keys":
                    {
                        vargs.ExpectArgs("keys method", (0, 0));
                        var result = Value.FromArray();
                        foreach (var key in obj.Keys)
                            result.Add(new Value(key));
                        return result;
                    }

                case "get":
                    {
                        vargs.ExpectArgs("get method", (1, 2));
                        var key = vargs.Args[0];
                        if (vargs.Args.Count == 1)
                            return obj.Contains(key) ? obj.Get(key) : new Value();
                        else
                            return obj.Contains(key) ? obj.Get(key) : vargs.Args[1];
                    }

                default:
                    {
                        var methodName = new Value(_method.Name);
                        if (obj.Contains(methodName))
                        {
                            var callable = obj.Get(methodName);
                            if (!callable.IsCallable)
                                throw new JinjaException($"Property '{_method.Name}' is not callable");
                            return callable.Call(context, vargs);
                        }
                        break;
                    }
            }
        }
        else if (obj.IsString)
        {
            var str = obj.Get<string>();
            switch (_method.Name)
            {
                case "strip":
                    {
                        vargs.ExpectArgs("strip method", (0, 1));
                        var chars = vargs.Args.Count == 0 ? "" : vargs.Args[0].Get<string>();
                        return new Value(StringUtils.Strip(str, chars));
                    }
                case "lstrip":
                    {
                        vargs.ExpectArgs("lstrip method", (0, 1));
                        var chars = vargs.Args.Count == 0 ? "" : vargs.Args[0].Get<string>();
                        return new Value(StringUtils.Strip(str, chars, left: true, right: false));
                    }
                case "rstrip":
                    {
                        vargs.ExpectArgs("rstrip method", (0, 1));
                        var chars = vargs.Args.Count == 0 ? "" : vargs.Args[0].Get<string>();
                        return new Value(StringUtils.Strip(str, chars, left: false, right: true));
                    }

                case "split":
                    {
                        vargs.ExpectArgs("split method", (1, 1));
                        var sep = vargs.Args[0].Get<string>();
                        var parts = StringUtils.Split(str, sep);
                        var result = Value.FromArray();
                        foreach (var part in parts)
                            result.Add(new Value(part));
                        return result;
                    }

                case "capitalize":
                    vargs.ExpectArgs("capitalize method", (0, 0));
                    return new Value(StringUtils.Capitalize(str));

                case "upper":
                    vargs.ExpectArgs("upper method", (0, 0));
                    return new Value(str.ToUpperInvariant());

                case "lower":
                    vargs.ExpectArgs("lower method", (0, 0));
                    return new Value(str.ToLowerInvariant());

                case "endswith":
                    {
                        vargs.ExpectArgs("endswith method", (1, 1));
                        var suffix = vargs.Args[0].Get<string>();
                        return new Value(str.EndsWith(suffix, StringComparison.InvariantCulture));
                    }

                case "startswith":
                    {
                        vargs.ExpectArgs("startswith method", (1, 1));
                        var prefix = vargs.Args[0].Get<string>();
                        return new Value(str.StartsWith(prefix, StringComparison.InvariantCulture));
                    }

                case "title":
                    {
                        vargs.ExpectArgs("title method", (0, 0));
                        var res = new StringBuilder(str);
                        for (var i = 0; i < res.Length; ++i)
                            if (i == 0 || char.IsWhiteSpace(res[i - 1]))
                                res[i] = char.ToUpperInvariant(res[i]);
                            else
                                res[i] = char.ToLowerInvariant(res[i]);
                        return new Value(res.ToString());
                    }

                case "replace":
                    {
                        vargs.ExpectArgs("replace method", (2, 3));
                        var before = vargs.Args[0].Get<string>();
                        var after = vargs.Args[1].Get<string>();
                        var count = vargs.Args.Count == 3 ? vargs.Args[2].Get<long>() : str.Length;
                        var startPos = 0;
                        int nextPos;
                        while ((nextPos = str.IndexOf(before, startPos)) >= 0 && count-- > 0)
                        {
                            str = str[0..nextPos]
                                + after
                                + str[(nextPos + before.Length)..];
                            startPos = nextPos + after.Length;
                        }
                        return new Value(str);
                    }
            }
        }
        throw new JinjaException($"Unknown method {_method.Name}");
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(_object);
        sb.Append('.');
        sb.Append(_method.ToString());
        sb.Append('(');
        sb.Append(_arguments);
        sb.Append(')');
        return sb.ToString();
    }
}

