using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace LLamaSharp.Jinja;

/// <summary>
/// Wrapper for a "Python" value.
/// </summary>
internal class Value : IEquatable<Value>, IComparable<Value>
{
    private readonly IList<Value>? _array;
    private readonly OrderedDictionary<Value, Value>? _object;
    private readonly CallableType? _callable;
    private readonly object? _primitive;
    public static readonly Value Null = new Value();

    private Value()
    {
    }

    private Value(IList<Value> array)
    {
        _array = array;
    }

    private Value(OrderedDictionary<Value, Value> @object)
    {
        _object = @object;
    }

    private Value(CallableType callable)
    {
        _object = [];
        _callable = callable;
    }

    public Value(Value value)
    {
        if (value.IsObject)
            _object = new OrderedDictionary<Value, Value>(value._object, value._object.Comparer);
        else if (value.IsArray)
            _array = [.. value._array];
        else
            _primitive = value._primitive;
    }

    public delegate Value CallableType(Context context, ArgumentsValue args);

    [MemberNotNullWhen(true, nameof(_object))]
    public bool IsObject => _object is not null;

    [MemberNotNullWhen(true, nameof(_callable))]
    public bool IsCallable => _callable is not null;

    [MemberNotNullWhen(true, nameof(_primitive))]
    public bool IsString => _primitive is string;

    [MemberNotNullWhen(true, nameof(_primitive))]

    [MemberNotNullWhen(true, nameof(_primitive))]
    public bool IsBoolean => _primitive is bool;
    public bool IsNull => _object is null && _array is null && _primitive is null && _callable is null;

    public bool IsNumber => IsInteger || IsDouble;

    [MemberNotNullWhen(true, nameof(_primitive))]
    public bool IsInteger => _primitive is long;

    [MemberNotNullWhen(true, nameof(_primitive))]
    public bool IsDouble => _primitive is double;
    public bool IsPrimitive => _object is null && _array is null && _callable is null;

    #region iterable support
    public bool IsIterable => IsArray || IsObject || IsString;

    [MemberNotNullWhen(true, nameof(_array))]
    public bool IsArray => _array is not null;

    private bool IsHashable => IsPrimitive;

    public int Count
    {
        get
        {
            if (IsObject)
                return _object.Count;
            if (IsArray)
                return _array.Count;
            if (IsString)
                return ((string)_primitive).Length;
            throw new JinjaException($"Value is not an array or object: {Dump()}");
        }
    }

    public Value this[int index]
    {
        get
        {
            if (IsNull)
                throw new JinjaException("Undefined value or reference");
            if (IsArray)
                return _array[index];
            if (IsObject)
                return _object[index];
            throw new JinjaException($"Value is not an array or object: {Dump()}");
        }
    }

    public IEnumerable<Value> Keys
    {
        get
        {
            if (!IsObject)
                throw new JinjaException($"Value is not an object: {Dump()}");
            return _object.Keys;
        }
    }

    public void ForEach(Action<Value> action)
    {
        if (IsNull)
            throw new JinjaException("Undefined value or reference");
        if (_array is not null)
            foreach (var item in _array)
                action(item);
        else if (_object is not null)
            foreach (var item in _object)
                action(item.Key);
        else if (IsString)
            foreach (var ch in (string)_primitive)
                action(new Value(ch.ToString()));
        else
            throw new JinjaException($"Value is not iterable: {Dump()}");
    }

    #endregion

    public Value(string value)
    {
        _primitive = value;
    }
    public Value(long value)
    {
        _primitive = value;
    }
    public Value(bool value)
    {
        _primitive = value;
    }

    public Value(double value)
    {
        _primitive = value;
    }

    public bool ToBoolean()
    {
        if (IsNull)
            return false;
        if (IsBoolean)
            return Get<bool>();
        if (IsInteger)
            return Get<long>() != 0;
        if (IsDouble)
            return Get<double>() != 0;
        if (IsString)
            return !string.IsNullOrEmpty(Get<string>());
        if (IsArray)
            return Count > 0;
        return true;
    }

    public void Set(Value key, Value value)
    {
        if (_object is null)
            throw new JinjaException($"Value is not an object: {Dump()}");
        if (!key.IsHashable)
            throw new JinjaException($"Unhashable type: {Dump()}");
        _object[key] = value;
    }

    public Value Get(Value key)
    {
        if (_array is not null)
        {
            if (!key.IsInteger)
                return Null;
            var index = key.Get<long>();
            return _array[(int)(index < 0 ? _array.Count + index : index)];
        }
        else if (_object is not null)
        {
            if (!key.IsHashable)
                throw new JinjaException($"Unhashable type: {key.Dump()}");
            if (_object.TryGetValue(key, out var value))
                return value;
        }
        return Null;
    }

    public Value Get(string key) => Get(new Value(key));

    public bool Contains(Value value)
    {
        if (IsNull)
            throw new JinjaException("Undefined value or reference");
        if (_array is not null)
        {
            foreach (var item in _array)
                if (item.ToBoolean() && item.Equals(value))
                    return true;
            return false;

        }
        else if (_object is not null)
        {
            if (!value.IsHashable)
                throw new JinjaException($"Unhashable type: {value.Dump()}");
            return _object.ContainsKey(value);
        }
        else
            throw new JinjaException($"Contains can only be called on arrays or objects: {Dump()}");
    }


    public void Set(string key, long value) => Set(new Value(key), new Value(value));
    public void Set(string key, CallableType value) => Set(new Value(key), new Value(value));
    public void Set(string key, bool value) => Set(new Value(key), new Value(value));

    public void Set(string key, Value value) => Set(new Value(key), value);
    public T Get<T>()
    {
        if (IsPrimitive)
            return (T)_primitive!;
        throw new JinjaException($"Get<T> is not defined for this value type {Dump()}");
    }

    public string Dump(int indent = -1, bool toJson = false)
    {
        using var writer = new StringWriter();
        Dump(writer, indent, toJson: toJson);
        return writer.ToString();
    }

    public static Value FromArray(IEnumerable<Value>? values = null)
    {
        var array = new List<Value>();
        if (values is not null)
            array.AddRange(values);
        return new Value(array);
    }

    public static Value Callable(CallableType callable)
    {
        return new Value(callable);
    }

    public static Value Object()
    {
        return new Value(new OrderedDictionary<Value, Value>());
    }

    public Value Call(Context context, ArgumentsValue args)
    {
        if (_callable is null)
            throw new JinjaException($"Value is not callable: {Dump()}");
        return _callable(context, args);
    }

    public void Add(Value item)
    {
        if (_array is null)
            throw new JinjaException($"Value is not an array {Dump()}");
        _array.Add(item);
    }

    private void Dump(TextWriter writer, int indent = -1, int level = 0, bool toJson = false)
    {

        void PrintIndent(int level)
        {
            if (indent > 0)
            {
                writer.Write('\n');
                for (int i = 0, n = level * indent; i < n; ++i)
                    writer.Write(' ');
            }
        }

        void PrintSubSeparator()
        {
            writer.Write(',');
            if (indent < 0)
                writer.Write(' ');
            else
                PrintIndent(level + 1);
        }

        var stringQuote = toJson ? '"' : '\'';

        if (IsNull)
            writer.Write("null");
        else if (_array is not null)
        {
            writer.Write('[');
            PrintIndent(level + 1);
            for (var i = 0; i < _array.Count; ++i)
            {
                if (i > 0)
                    PrintSubSeparator();
                _array[i].Dump(writer, indent, level + 1, toJson);
            }
            PrintIndent(level);
            writer.Write(']');
        }
        else if (_object is not null)
        {
            writer.Write('{');
            PrintIndent(level + 1);
            var first = true;
            foreach (var kvp in _object)
            {
                if (!first)
                    PrintSubSeparator();
                first = false;
                if (kvp.Key.IsString)
                    DumpString(kvp.Key, writer, stringQuote);
                else
                {
                    writer.Write(stringQuote);
                    writer.Write(kvp.Key.Dump());
                    writer.Write(stringQuote);
                }
                writer.Write(": ");
                kvp.Value.Dump(writer, indent, level + 1, toJson);
            }
            PrintIndent(level);
            writer.Write('}');
        }
        else if (_callable is not null)
            throw new JinjaException("Cannot dump callable to JSON");
        else if (IsBoolean && !toJson)
            writer.Write(ToBoolean() ? "True" : "False");
        else if (IsString && !toJson)
            DumpString(this, writer, stringQuote);
        else
            writer.Write(JsonSerializer.Serialize(_primitive));
    }

    private static void DumpString(Value primitive, TextWriter writer, char stringQuote = '\'')
    {
        if (primitive._primitive is not string s)
            throw new JinjaException($"Value is not a string: {primitive._primitive}");
        writer.Write(stringQuote);
        for (var i = 0; i < s.Length; ++i)
        {
            if (s[i] == stringQuote)
                writer.Write('\\');
            writer.Write(s[i]);
        }
        writer.Write(stringQuote);
    }


    public bool Contains(string value)
    {
        if (_array is not null)
            return false;
        else if (_object is not null)
            return _object.ContainsKey(new Value(value));
        else
            throw new JinjaException($"Contains can only be called on arrays or objects: {Dump()}");
    }

    public Value Pop(Value index)
    {
        if (IsArray)
        {
            if (_array.Count == 0)
                throw new JinjaException("pop from empty list");
            if (index.IsNull)
            {
                var ret = _array[_array.Count - 1];
                _array.RemoveAt(_array.Count - 1);
                return ret;
            }
            else if (!index.IsInteger)
                throw new JinjaException($"pop index must be an integer: {index.Dump()}");
            else
            {
                var i = index.Get<long>();
                if (i < 0 || i >= _array.Count)
                    throw new JinjaException($"pop index out of range: {index.Dump()}");
                if (i < 0)
                    i += _array.Count;
                var ret = _array[(int)i];
                _array.RemoveAt((int)i);
                return ret;
            }
        }
        else if (IsObject)
        {
            if (!index.IsHashable)
                throw new JinjaException($"Unhashable type: {index.Dump()}");
            if (!_object.TryGetValue(index, out var ret))
                throw new JinjaException($"Key not found: {index.Dump()}");
            _object.Remove(index);
            return ret;
        }
        else
            throw new JinjaException($"Value is not an array or object: {Dump()}");
    }

    public void Insert(int index, Value item)
    {
        if (!IsArray)
            throw new JinjaException($"Value is not an array: {Dump()}");
        _array.Insert(index, item);
    }

    #region IEquatable<Value> Members
    public bool Equals(Value? other)
    {
        if (other is null)
            return false;
        if ((_callable is not null || other._callable is not null) && _callable != other._callable)
            return false;
        if (_array is not null)
        {
            if (other._array is null)
                return false;
            if (_array.Count != other._array.Count)
                return false;
            for (var i = 0; i < _array.Count; ++i)
                if (!_array[i].ToBoolean() || !other._array[i].ToBoolean() || !_array[i].Equals(other._array[i]))
                    return false;
            return true;
        }
        else if (_object is not null)
        {
            if (other._object is null)
                return false;
            if (_object.Count != other._object.Count)
                return false;
            foreach (var kvp in _object)
                if (!kvp.Value.ToBoolean() || !other._object.TryGetValue(kvp.Key, out var otherValue) || !otherValue.ToBoolean() || !kvp.Value.Equals(otherValue))
                    return false;
            return true;
        }
        else
        {
            if (_primitive is null || other._primitive is null)
                return _primitive is null && other._primitive is null;
            return _primitive.Equals(other._primitive);
        }
    }

    #endregion

    #region Overridden from Object
    public override bool Equals(object? obj)
    {
        return obj is Value value && Equals(value);
    }

    public override int GetHashCode()
    {
        // we can't compute hash codes for individual array or object elements, since this would not match 
        // the semantics of Equals(Value?)
        if (_array is not null)
            return _array.Count;
        else if (_object is not null)
            return _object.Count;
        else if (_primitive is not null)
            return _primitive.GetHashCode();
        else if (_callable is not null)
            return _callable.GetHashCode();
        return 0;
    }

    public override string ToString()
    {
        if (IsString)
            return Get<string>();
        if (IsInteger)
            return Get<long>().ToString();
        if (IsDouble)
            return Get<double>().ToString();
        if (IsBoolean)
            return ToBoolean() ? "True" : "False";
        if (IsNull)
            return "none";
        return Dump();
    }

    #endregion

    #region IComparable<Value> Members  
    public int CompareTo(Value? other)
    {
        if (IsNull)
            throw new JinjaException("Undefined value or reference");
        if (other is not null)
        {
            if (IsInteger && other.IsInteger)
                return Get<long>().CompareTo(other.Get<long>());
            if (IsDouble && other.IsDouble)
                return Get<double>().CompareTo(other.Get<double>());
            if (IsString && other.IsString)
                return string.Compare(Get<string>(), other.Get<string>(), StringComparison.InvariantCulture);
        }
        throw new JinjaException($"Cannot compare values: {Dump()} and {other?.Dump() ?? "null"}");
    }

    #endregion

    #region Relational operators

    public static bool operator ==(Value left, Value right) => left.Equals(right);

    public static bool operator !=(Value left, Value right) => !left.Equals(right);

    public static bool operator <(Value left, Value right) => left.CompareTo(right) < 0;

    public static bool operator >(Value left, Value right) => left.CompareTo(right) > 0;

    public static bool operator <=(Value left, Value right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Value left, Value right) => left.CompareTo(right) >= 0;

    #endregion

    #region Arithmetic

    public static Value operator -(Value value)
    {
        if (value.IsInteger)
            return new Value(-value.Get<long>());
        if (value.IsDouble)
            return new Value(-value.Get<double>());
        throw new JinjaException($"Unary minus not supported for type: {value.Dump()}");
    }

    public static Value operator +(Value left, Value right)
    {
        if (left.IsString && right.IsString)
            return new Value(left.Get<string>() + right.Get<string>());
        if (left.IsInteger && right.IsInteger)
            return new Value(left.Get<long>() + right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(left.Get<double>() + right.Get<double>());
        if (left.IsArray && right.IsArray)
        {
            var result = FromArray();
            left.ForEach(item => result.Add(item));
            right.ForEach(item => result.Add(item));
            return result;
        }
        throw new JinjaException($"Cannot add values: {left.Dump()} + {right.Dump()}");
    }

    public static Value operator -(Value left, Value right)
    {
        if (left.IsInteger && right.IsInteger)
            return new Value(left.Get<long>() - right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(left.Get<double>() - right.Get<double>());
        throw new JinjaException($"Cannot subtract values: {left.Dump()} - {right.Dump()}");
    }

    public static Value operator *(Value left, Value right)
    {
        if (left.IsString && right.IsInteger)
        {
            var str = left.Get<string>();
            var count = right.Get<long>();
            if (count < 0)
                count = 0;
            return new Value(string.Concat(Enumerable.Repeat(str, (int)count)));
        }
        if (left.IsInteger && right.IsInteger)
            return new Value(left.Get<long>() * right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(left.Get<double>() * right.Get<double>());
        throw new JinjaException($"Cannot multiply values: {left.Dump()} * {right.Dump()}");
    }


    public static Value operator %(Value left, Value right)
    {
        if (left.IsInteger && right.IsInteger)
            return new Value(left.Get<long>() % right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(left.Get<double>() % right.Get<double>());
        throw new JinjaException($"Cannot compute modulus of values: {left.Dump()} % {right.Dump()}");
    }

    /// <summary>
    /// Python real division operator: always returns a float
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    /// <exception cref="JinjaException"></exception>
    public static Value operator /(Value left, Value right)
    {
        if (left.IsInteger && right.IsInteger)
            return new Value((double)left.Get<long>() / right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(left.Get<double>() / right.Get<double>());
        throw new JinjaException($"Cannot divide values: {left.Dump()} / {right.Dump()}");
    }

    public static Value FloorDiv(Value left, Value right)
    {
        if (left.IsInteger && right.IsInteger)
            return new Value(left.Get<long>() / right.Get<long>());
        if (left.IsDouble && right.IsDouble)
            return new Value(Math.Floor(left.Get<double>() / right.Get<double>()));
        throw new JinjaException($"Cannot divide values: {left.Dump()} // {right.Dump()}");
    }

    public static Value Pow(Value left, Value right)
    {
        if (left.IsInteger && right.IsInteger)
            return new Value(Math.Pow(left.Get<long>(), right.Get<long>()));
        if (left.IsDouble && right.IsDouble)
            return new Value(Math.Pow(left.Get<double>(), right.Get<double>()));
        throw new JinjaException($"Cannot exponentiate values: {left.Dump()} ** {right.Dump()}");
    }

    #endregion
}

