using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaSharp.Jinja;

/// <summary>
/// The evaluation context
/// </summary>
public sealed class Context
{
    private readonly Value _values;
    private readonly Context? _parent;

    internal Context(Value values, Context? parent = null)
    {
        if (!values.IsObject)
            throw new JinjaException($"Context values must be an object: {values.Dump()}");
        _values = values;
        _parent = parent;
    }

    private bool Contains(Value value)
    {
        if (_values.Contains(value))
            return true;
        else if (_parent is not null)
            return _parent.Contains(value);
        else
            return false;
    }

    internal bool Contains(string value) => Contains(new Value(value));

    internal Value Get(Value value)
    {
        if (_values.Contains(value))
            return _values.Get(value);
        else if (_parent is not null)
            return _parent.Get(value);
        else
            return new Value();
    }

    internal Value Get(string value) => Get(new Value(value));


    internal void Set(Value name, Value value)
    {
        _values.Set(name, value);
    }

    internal void Set(string name, Value value) => Set(new Value(name), value);
    public void Set(string name, string value) => Set(new Value(name), new Value(value));

    internal void DestructuringAssign(IReadOnlyCollection<string> variableNames, Value item)
    {
        if (variableNames.Count == 1)
        {
            var name = new Value(variableNames.Single());
            Set(name, item);
        }
        else
        {
            if (!item.IsArray || item.Count != variableNames.Count)
                throw new JinjaException("Mismatched number of variables and items in destructuring assignment");
            var i = 0;
            foreach (var variableName in variableNames)
                Set(variableName, item[i++]);
        }
    }

    internal static Context BuiltIns()
    {
        var globals = Value.Object();

        // raise_exception
        globals.Set("raise_exception", SimpleFunction("raise_exception", ["message"], (context, argsObj) =>
        {
            throw new JinjaException(argsObj.Get("message").Get<string>());
        }));

        // tojson
        // Accept optional keyword arguments to match common Jinja usage such as
        // tojson(indent=2) and tojson(ensure_ascii=False). The ensure_ascii flag
        // is ignored since nlohmann::json dumps UTF-8 by default, but it is accepted for compatibility to avoid "Unknown argument" errors.
        globals.Set("tojson", SimpleFunction("tojson", ["value", "indent", "ensure_ascii"], (context, argsObj) =>
        {
            var indent = argsObj.Contains("indent") ? argsObj.Get("indent").Get<long>() : -1;
            return new Value(argsObj.Get("value").Dump((int)indent, toJson: true));
        }));

        // items
        globals.Set("items", SimpleFunction("items", ["object"], (context, argsObj) =>
        {
            var items = Value.FromArray();
            var obj = argsObj.Get("object");
            if (!obj.IsObject)
                throw new JinjaException("Can only get item pairs from a mapping");
            foreach (var key in obj.Keys)
                items.Add(Value.FromArray([new Value(key), obj.Get(new Value(key))]));
            return items;
        }));

        // last
        globals.Set("last", SimpleFunction("last", ["items"], (context, argsObj) =>
        {
            var items = argsObj.Get("items");
            if (!items.IsArray)
                throw new JinjaException("object is not a list");
            if (items.Count == 0)
                return new Value();
            return items[^1];
        }));

        // trim
        globals.Set("trim", SimpleFunction("trim", ["text"], (context, argsObj) =>
        {
            var text = argsObj.Get("text");
            return text.IsNull ? text : new Value(StringUtils.Strip(text.Get<string>()));
        }));

        // char_transform_function
        Value CharTransformFunction(string name, Func<char, char> fn)
        {
            return SimpleFunction(name, ["text"], (context, argsObj) =>
            {
                var text = argsObj.Get("text");
                if (text.IsNull) return text;
                var str = text.Get<string>();
                var res = new string([.. str.Select(fn)]);
                return new Value(res);
            });
        }
        globals.Set("lower", CharTransformFunction("lower", char.ToLowerInvariant));
        globals.Set("upper", CharTransformFunction("upper", char.ToUpperInvariant));

        // default
        globals.Set("default", Value.Callable((context, args) =>
        {
            args.ExpectArgs("default", (2, 3), (0, 1));
            var value = args.Args[0];
            var defaultValue = args.Args[1];
            var boolean = false;
            if (args.Args.Count == 3)
                boolean = args.Args[2].Get<bool>();
            else
            {
                var bv = args.Kwargs.FirstOrDefault(k => k.Name == "boolean").Value;
                if (bv is not null && !bv.IsNull)
                    boolean = bv.Get<bool>();
            }
            return boolean ? (value.ToBoolean() ? value : defaultValue) : value.IsNull ? defaultValue : value;
        }));

        // escape
        var escape = SimpleFunction("escape", ["text"], (context, argsObj) =>
        {
            return new Value(StringUtils.HtmlEscape(argsObj.Get("text").Get<string>()));
        });
        globals.Set("e", escape);
        globals.Set("escape", escape);

        // joiner
        globals.Set("joiner", SimpleFunction("joiner", ["sep"], (context, argsObj) =>
        {
            var sep = argsObj.Contains(new Value("sep")) ? argsObj.Get("sep").Get<string>() : "";
            var first = true;
            return SimpleFunction("", [], (ctx, _) =>
            {
                if (first)
                {
                    first = false;
                    return new Value("");
                }
                return new Value(sep);
            });
        }));

        // count
        globals.Set("count", SimpleFunction("count", ["items"], (context, argsObj) =>
        {
            return new Value((long)argsObj.Get("items").Count);
        }));

        // dictsort
        globals.Set("dictsort", SimpleFunction("dictsort", ["value"], (context, argsObj) =>
        {
            var value = argsObj.Get("value");
            var res = Value.FromArray();
            foreach (var key in value.Keys.OrderBy(k => k))
                res.Add(Value.FromArray([new Value(key), value.Get(new Value(key))]));
            return res;
        }));

        // join
        globals.Set("join", SimpleFunction("join", ["items", "d"], (context, argsObj) =>
        {
            Value DoJoin(Value items, string sep)
            {
                if (!items.IsArray)
                    throw new JinjaException($"object is not iterable: {items.Dump()}");
                var sb = new StringBuilder();
                for (var i = 0; i < items.Count; ++i)
                {
                    if (i > 0) sb.Append(sep);
                    sb.Append(items[i].ToString());
                }
                return new Value(sb.ToString());
            }
            var sep = argsObj.Contains(new Value("d")) ? argsObj.Get("d").Get<string>() : "";
            if (argsObj.Contains(new Value("items")))
            {
                var items = argsObj.Get("items");
                return DoJoin(items, sep);
            }
            else
            {
                return SimpleFunction("", ["items"], (ctx, argsObj2) =>
                {
                    var items = argsObj2.Get("items");
                    if (!items.ToBoolean() || !items.IsArray)
                        throw new JinjaException($"join expects an array for items, got: {items.Dump()}");
                    return DoJoin(items, sep);
                });
            }
        }));

        // namespace
        globals.Set("namespace", Value.Callable((context, args) =>
        {
            var ns = Value.Object();
            args.ExpectArgs("namespace", (0, 0), (0, int.MaxValue));
            foreach (var (name, value) in args.Kwargs)
                ns.Set(name, value);
            return ns;
        }));

        // equalto
        var equalto = SimpleFunction("equalto", ["expected", "actual"], (context, argsObj) =>
        {
            return new Value(argsObj.Get("actual") == argsObj.Get("expected"));
        });
        globals.Set("equalto", equalto);
        globals.Set("==", equalto);

        // length
        globals.Set("length", SimpleFunction("length", ["items"], (context, argsObj) =>
        {
            var items = argsObj.Get("items");
            return new Value((long)items.Count);
        }));

        // safe
        globals.Set("safe", SimpleFunction("safe", ["value"], (context, argsObj) =>
        {
            return new Value(argsObj.Get("value").ToString());
        }));

        // string
        globals.Set("string", SimpleFunction("string", ["value"], (context, argsObj) =>
        {
            return new Value(argsObj.Get("value").ToString());
        }));

        // int
        globals.Set("int", SimpleFunction("int", ["value"], (context, argsObj) =>
        {
            var value = argsObj.Get("value");
            long result;
            if (value.IsNull)
                result = 0;
            else if (value.IsBoolean)
                result = value.Get<bool>() ? 1 : 0;
            else if (value.IsInteger)
                result = value.Get<long>();
            else if (value.IsDouble)
                result = (long)value.Get<double>();
            else if (value.IsString)
            {
                if (!long.TryParse(value.Get<string>(), CultureInfo.InvariantCulture, out result))
                    result = 0;
            }
            else
                result = 0;
            return new Value(result);
        }));

        // list
        globals.Set("list", SimpleFunction("list", ["items"], (context, argsObj) =>
        {
            var items = argsObj.Get("items");
            if (!items.IsArray)
                throw new JinjaException("object is not iterable");
            return items;
        }));

        // in
        globals.Set("in", SimpleFunction("in", ["item", "items"], (context, argsObj) =>
        {
            return new Value(ValueUtils.In(argsObj.Get("item"), argsObj.Get("items")));
        }));

        // unique
        globals.Set("unique", SimpleFunction("unique", ["items"], (context, argsObj) =>
        {
            var items = argsObj.Get("items");
            if (!items.IsArray)
                throw new JinjaException("object is not iterable");
            var seen = new HashSet<Value>();
            var result = Value.FromArray();
            for (var i = 0; i < items.Count; i++)
            {
                if (seen.Add(items[i]))
                    result.Add(items[i]);
            }
            return result;
        }));

        // make_filter
        Value MakeFilter(Value filter, Value extraArgs)
        {
            return SimpleFunction("", ["value"], (context, argsObj) =>
            {
                var value = argsObj.Get("value");
                var actualArgs = new ArgumentsValue();
                actualArgs.Args.Add(value);
                for (var i = 0; i < extraArgs.Count; i++)
                    actualArgs.Args.Add(extraArgs[i]);
                return filter.Call(context, actualArgs);
            });
        }

        // select/reject
        Value SelectOrReject(bool isSelect)
        {
            return Value.Callable((context, args) =>
            {
                args.ExpectArgs(isSelect ? "select" : "reject", (2, int.MaxValue), (0, 0));
                var items = args.Args[0];
                if (items.IsNull)
                    return Value.FromArray();
                if (!items.IsArray)
                    throw new JinjaException($"object is not iterable: {items.Dump()}");
                var filterFn = context.Get(args.Args[1].ToString() ?? "");
                if (filterFn.IsNull)
                    throw new JinjaException($"Undefined filter: {args.Args[1].Dump()}");
                var filterArgs = Value.FromArray();
                for (var i = 2; i < args.Args.Count; i++)
                    filterArgs.Add(args.Args[i]);
                var filter = MakeFilter(filterFn, filterArgs);
                var res = Value.FromArray();
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var filterCallArgs = new ArgumentsValue();
                    filterCallArgs.Args.Add(item);
                    var predRes = filter.Call(context, filterCallArgs);
                    if (predRes.ToBoolean() == isSelect)
                        res.Add(item);
                }
                return res;
            });
        }
        globals.Set("select", SelectOrReject(true));
        globals.Set("reject", SelectOrReject(false));

        // map
        globals.Set("map", Value.Callable((context, args) =>
        {
            var res = Value.FromArray();
            if (args.Args.Count == 1 &&
                (args.Kwargs.Any(k => k.Name == "attribute") && args.Kwargs.Count == 1 ||
                 args.Kwargs.Any(k => k.Name == "default") && args.Kwargs.Count == 2))
            {
                var items = args.Args[0];
                var attrName = args.Kwargs.FirstOrDefault(k => k.Name == "attribute").Value;
                var defaultValue = args.Kwargs.FirstOrDefault(k => k.Name == "default").Value;
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var attr = item.Get(attrName);
                    res.Add(attr.IsNull ? defaultValue : attr);
                }
            }
            else if (args.Kwargs.Count == 0 && args.Args.Count >= 2)
            {
                var fn = context.Get(args.Args[1].ToString() ?? "");
                if (fn.IsNull)
                    throw new JinjaException($"Undefined filter: {args.Args[1].Dump()}");
                var filterArgs = new ArgumentsValue();
                filterArgs.Args.Add(new Value());
                for (var i = 2; i < args.Args.Count; i++)
                    filterArgs.Args.Add(args.Args[i]);
                for (var i = 0; i < args.Args[0].Count; i++)
                {
                    var item = args.Args[0][i];
                    filterArgs.Args[0] = item;
                    res.Add(fn.Call(context, filterArgs));
                }
            }
            else
            {
                throw new JinjaException("Invalid or unsupported arguments for map");
            }
            return res;
        }));

        // indent
        globals.Set("indent", SimpleFunction("indent", ["text", "indent", "first"], (context, argsObj) =>
        {
            var text = argsObj.Get("text").Get<string>();
            var first = argsObj.Contains(new Value("first")) && argsObj.Get("first").Get<bool>();
            var indentStr = new string(' ', argsObj.Contains(new Value("indent")) ? (int)argsObj.Get("indent").Get<long>() : 0);
            var sb = new StringBuilder();
            var isFirst = true;
            var reader = new StringReader(text);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                var needsIndent = !isFirst || first;
                if (isFirst) isFirst = false;
                else sb.Append('\n');
                if (needsIndent) sb.Append(indentStr);
                sb.Append(line);
            }
            if (!string.IsNullOrEmpty(text) && text.EndsWith('\n'))
                sb.Append('\n');
            return new Value(sb.ToString());
        }));

        // selectattr/rejectattr
        Value SelectOrRejectAttr(bool isSelect)
        {
            return Value.Callable((context, args) =>
            {
                args.ExpectArgs(isSelect ? "selectattr" : "rejectattr", (2, int.MaxValue), (0, 0));
                var items = args.Args[0];
                if (items.IsNull)
                    return Value.FromArray();
                if (!items.IsArray)
                    throw new JinjaException($"object is not iterable: {items.Dump()}");
                var attrName = args.Args[1].Get<string>();
                var hasTest = args.Args.Count >= 3;
                Value testFn = null!;
                var testArgs = new ArgumentsValue();
                testArgs.Args.Add(new Value());
                if (hasTest)
                {
                    testFn = context.Get(args.Args[2].ToString() ?? "");
                    if (testFn.IsNull)
                        throw new JinjaException($"Undefined test: {args.Args[2].Dump()}");
                    for (var i = 3; i < args.Args.Count; i++)
                        testArgs.Args.Add(args.Args[i]);
                    testArgs.Kwargs = args.Kwargs;
                }
                var res = Value.FromArray();
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var attr = item.Get(new Value(attrName));
                    if (hasTest)
                    {
                        testArgs.Args[0] = attr;
                        if (testFn.Call(context, testArgs).ToBoolean() == isSelect)
                            res.Add(item);
                    }
                    else
                        res.Add(attr);
                }
                return res;
            });
        }
        globals.Set("selectattr", SelectOrRejectAttr(true));
        globals.Set("rejectattr", SelectOrRejectAttr(false));

        // range
        globals.Set("range", Value.Callable((context, args) =>
        {
            var startEndStep = new long[3];
            var paramSet = new bool[3];
            if (args.Args.Count == 1)
            {
                startEndStep[1] = args.Args[0].Get<long>();
                paramSet[1] = true;
            }
            else
            {
                for (var i = 0; i < args.Args.Count; i++)
                {
                    startEndStep[i] = args.Args[i].Get<long>();
                    paramSet[i] = true;
                }
            }
            foreach (var (name, value) in args.Kwargs)
            {
                var i = name switch
                {
                    "start" => 0,
                    "end" => 1,
                    "step" => 2,
                    _ => throw new JinjaException($"Unknown argument {name} for function range")
                };
                if (paramSet[i])
                    throw new JinjaException($"Duplicate argument {name} for function range");
                startEndStep[i] = value.Get<long>();
                paramSet[i] = true;
            }
            if (!paramSet[1])
                throw new JinjaException("Missing required argument 'end' for function range");
            var start = paramSet[0] ? startEndStep[0] : 0;
            var end = startEndStep[1];
            var step = paramSet[2] ? startEndStep[2] : 1;
            var res = Value.FromArray();
            if (step > 0)
                for (var i = start; i < end; i += step)
                    res.Add(new Value(i));
            else
                for (var i = start; i > end; i += step)
                    res.Add(new Value(i));
            return res;
        }));

        globals.Set("strftime_now", Value.Callable((context, args) =>
        {
            args.ExpectArgs("strftime_now", (1, 1));
            var unixFormat = args.Args[0].Get<string>();
            var localTime = DateTimeOffset.Now;

            var sb = new StringBuilder();
            var i = 0;
            while (i < unixFormat.Length)
            {
                var ch = unixFormat[i++];
                if (ch == '%')
                {
                    ch = unixFormat[i++];
                    switch (ch)
                    {
                        // literals
                        case '%':
                            sb.Append(ch);
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;

                        // year
                        case 'Y':
                            sb.Append("yyyy");
                            break;
                        case 'y':
                            sb.Append("yy");
                            break;

                        // month
                        case 'm':
                            sb.Append("MM");
                            break;
                        case 'b':
                            sb.Append("MMM");
                            break;
                        case 'B':
                            sb.Append("MMMM");
                            break;

                        // day
                        case 'd':
                            sb.Append("dd");
                            break;
                        case 'j':
                            sb.Append("ddd");
                            break;
                        case 'a':
                            sb.Append("ddd");
                            break;
                        case 'A':
                            sb.Append("dddd");
                            break;

                        // hour
                        case 'H':
                            sb.Append("HH");
                            break;
                        case 'I':
                            sb.Append("hh");
                            break;

                        // minute
                        case 'M':
                            sb.Append("mm");
                            break;

                        // second
                        case 'S':
                            sb.Append("ss");
                            break;

                        // microsecond
                        case 'f':
                            sb.Append("ffffff");
                            break;

                        // AM/PM
                        case 'p':
                            sb.Append("tt");
                            break;

                        // timezones
                        case 'z':
                            sb.Append("zzz");
                            break;
                        case 'Z':
                            sb.Append('K');
                            break;

                        // mapped by computation
                        case 'w':
                            sb.Append((int)localTime.DayOfWeek);    // weekday as decimal
                            break;
                        case 'V':
                            sb.Append(GetIsoWeekNumber(localTime));    // iso week number
                            break;
                        case 'U':
                            sb.Append(GetWeekOfYear(localTime, DayOfWeek.Sunday));    // week number sunday
                            break;
                        case 'W':
                            sb.Append(GetWeekOfYear(localTime, DayOfWeek.Monday));     // week number monday
                            break;

                        // Unmapped
                        case 'c':
                            sb.Append('G');    // locale date and time
                            break;
                        case 'x':
                            sb.Append('d');    // locale date, short date pattern
                            break;
                        case 'X':
                            sb.Append('T');    // locale time, long date pattern
                            break;

                        default:
                            AppendSafe(ch);
                            break;
                    }
                }
                else
                    AppendSafe(ch);
            }
            var formatted = localTime.ToString(sb.ToString(), CultureInfo.InvariantCulture);
            return new Value(formatted);

            void AppendSafe(char ch)
            {
                // See https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#Literals
                const string Reserved = "FHKMdfghmstyz%:/\"'\\";
                if (Reserved.Contains(ch))
                    sb.Append('\\');
                sb.Append(ch);
            }

            static int GetIsoWeekNumber(DateTimeOffset date)
            {
                // Use ISO 8601: week starts Monday, week 1 contains Jan 4
                // CultureInfo.InvariantCulture's CalendarWeekRule.FirstFourDayWeek matches ISO rule.
                var ci = CultureInfo.InvariantCulture;
                var calendar = ci.Calendar;

                return calendar.GetWeekOfYear(date.UtcDateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            static int GetWeekOfYear(DateTimeOffset date, DayOfWeek firstDayOfWeek)
            {
                var ci = CultureInfo.InvariantCulture;
                var calendar = ci.Calendar;

                return calendar.GetWeekOfYear(date.UtcDateTime, CalendarWeekRule.FirstFullWeek, firstDayOfWeek);
            }

        }));
        return new Context(globals);
    }

    internal static Context Make(Value values, Context? parent = null)
    {
        return new Context(values.IsNull ? new Value() : values, parent ?? BuiltIns());
    }

    private static Value CreateValueFromJsonObject(JsonElement element)
    {
        var obj = Value.Object();
        foreach (var kv in element.EnumerateObject())
            obj.Set(kv.Name, CreateValue(kv.Value));
        return obj;
    }

    private static bool IsKeyValuePairType(Type? type)
    {
        return type is not null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }

    private static bool IsKeyValueEnumerable(object bindings)
    {
        var type = bindings.GetType();
        var interfaces = type.GetInterfaces();
        var ienumerable = interfaces.FirstOrDefault(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (ienumerable is not null)
        {
            var ienumerableArgument = ienumerable.GetGenericArguments()[0];
            return IsKeyValuePairType(ienumerableArgument);
        }
        return false;
    }

    private static Value CreateValueFromKeyValuePairEnumeration(System.Collections.IEnumerable enumerable)
    {
        var obj = Value.Object();
        foreach (var item in enumerable)
        {
            var type = item.GetType();
            // we need the Key and Value properties, and nameof doesn't yet support open generics
            var keyProp = type.GetProperty(nameof(KeyValuePair<string, object>.Key))!;
            var valueProp = type.GetProperty(nameof(KeyValuePair<string, object>.Value))!;
            var k = keyProp.GetValue(item);
            var v = valueProp.GetValue(item);
            obj.Set(CreateValue(k), CreateValue(v));
        }
        return obj;
    }


    private static Value CreateValue(object? value)
    {
        if (value is null)
            return new Value();
        else if (value is string s)
            return new Value(s);
        else if (value is long l)
            return new Value(l);
        else if (value is int i)
            return new Value((long)i);
        else if (value is bool b)
            return new Value(b);
        else if (value is double d)
            return new Value(d);
        else if (value is float f)
            return new Value((double)f);
        else if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
                return CreateValueFromJsonObject(element);
            else if (element.ValueKind == JsonValueKind.Array)
            {
                var arr = Value.FromArray();
                foreach (var item in element.EnumerateArray())
                    arr.Add(CreateValue(item));
                return arr;
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                var str = element.GetString();
                return str is null ? new Value() : new Value(str);
            }
            else if (element.ValueKind == JsonValueKind.Number)
                if (element.TryGetInt64(out var longVal))
                    return new Value(longVal);
                else if (element.TryGetDouble(out var doubleVal))
                    return new Value(doubleVal);
                else
                    return new Value();
            else if (element.ValueKind == JsonValueKind.True)
                return new Value(true);
            else if (element.ValueKind == JsonValueKind.False)
                return new Value(false);
            else // JsonValueKind.Null or JsonValueKind.Undefined
                return new Value();
        }
        else if (IsKeyValueEnumerable(value))
            // This handles e.g. IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> and similar by treating them as enumerables of key-value pairs and turning them into object properties.
            return CreateValueFromKeyValuePairEnumeration((System.Collections.IEnumerable)value);
        else if (value is Array array)
            if (IsKeyValuePairType(array.GetType().GetElementType()))
                return CreateValueFromKeyValuePairEnumeration(array);
            else
            {
                var arr = Value.FromArray();
                foreach (var item in array)
                    arr.Add(CreateValue(item));
                return arr;
            }
        else
        {
            var obj = Value.Object();
            var type = value.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
            {
                var item1 = type.GetField("Item1")!.GetValue(value);
                var item2 = type.GetField("Item2")!.GetValue(value);
                obj.Set(CreateValue(item1), CreateValue(item2));
            }
            else
                foreach (var propertyInfo in type.GetProperties())
                {
                    var jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
                    obj.Set(jsonPropertyName?.Name ?? propertyInfo.Name, CreateValue(propertyInfo.GetValue(value)));
                }
            return obj;
        }
    }

    public static Context Make(object? bindings, Context? parent = null)
    {
        Value values;

        if (bindings is null)
            values = Value.Object();
        else
            values = CreateValue(bindings);
        return Make(values, parent);
    }

    public static Context Make(JsonElement bindings, Context? parent = null)
    {
        if (bindings.ValueKind != JsonValueKind.Object)
            throw new JinjaException("Context bindings must be a JSON object");
        var values = CreateValueFromJsonObject(bindings);
        return Make(values, parent);
    }

    public static Context Make(IEnumerable<JsonElement> bindings, Context? parent = null)
    {
        var values = Value.Object();
        foreach (var element in bindings)
        {
            if (element.ValueKind != JsonValueKind.Object)
                throw new JinjaException("Context bindings must be a JSON object");
            foreach (var kv in element.EnumerateObject())
                values.Set(kv.Name, CreateValue(kv.Value));
        }
        return Make(values, parent);
    }

    private static Value SimpleFunction(string fnName, IReadOnlyList<string> paramsList, Func<Context, Value, Value> fn)
    {
        var namedPositions = new Dictionary<string, int>(paramsList.Count);
        for (var i = 0; i < paramsList.Count; i++)
            namedPositions[paramsList[i]] = i;

        return Value.Callable((context, args) =>
        {
            var argsObj = Value.Object();
            var providedArgs = new bool[paramsList.Count];

            for (var i = 0; i < args.Args.Count; i++)
            {
                var arg = args.Args[i];
                if (i < paramsList.Count)
                {
                    argsObj.Set(paramsList[i], arg);
                    providedArgs[i] = true;
                }
                else
                    throw new JinjaException($"Too many positional params for {fnName}");
            }

            foreach (var (name, value) in args.Kwargs)
            {
                if (!namedPositions.TryGetValue(name, out var pos))
                    throw new JinjaException($"Unknown argument {name} for function {fnName}");
                providedArgs[pos] = true;
                argsObj.Set(name, value);
            }

            return fn(context, argsObj);
        });
    }
}

