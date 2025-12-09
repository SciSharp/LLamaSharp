using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LLamaSharp.Jinja;

/// <summary>
/// Represents a collection of key/value pairs that are accessible by the key or index.
/// A "real" ordered dictionary has been impemented in .NET 9 and later. This is a simplified implementation good enough for .NET 8.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>, IReadOnlyList<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _inner;
    private readonly List<TKey> _order;

    public OrderedDictionary()
    {
        _inner = [];
        _order = [];
    }

    public OrderedDictionary(IEqualityComparer<TKey>? comparer)
    {
        _inner = new(comparer);
        _order = [];
    }

    public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
    {
        _inner = new(dictionary, comparer);
        _order = [.. dictionary.Keys];
    }

    public IEqualityComparer<TKey> Comparer => _inner.Comparer;

    public TValue this[TKey key]
    {
        get
        {
            return _inner[key];
        }
        set
        {
            _inner[key] = value;
            var index = IndexOf(key);
            if (index < 0)
                _order.Add(key);
        }

    }

    public TValue this[int index]
    {
        get
        {
            var key = _order[index];
            return _inner[key];
        }
        set
        {
            var key = _order[index];
            _inner[key] = value;
        }
    }

    public ICollection<TKey> Keys => _order;

    public ICollection<TValue> Values => [.. _order.Select(key => _inner[key])];

    public int Count => _inner.Count;

    public bool IsReadOnly => false;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    KeyValuePair<TKey, TValue> IReadOnlyList<KeyValuePair<TKey, TValue>>.this[int index] => ((IList<KeyValuePair<TKey, TValue>>)this)[index];

    KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
    {
        get
        {
            var key = _order[index];
            return new KeyValuePair<TKey, TValue>(key, _inner[key]);
        }
        set
        {
            var key = _order[index];
            _order[index] = value.Key;
            _inner.Remove(key);
            _inner.Add(value.Key, value.Value);
        }

    }

    public void Add(TKey key, TValue value)
    {
        _inner.Add(key, value);
        _order.Add(key);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _inner.Add(item.Key, item.Value);
        _order.Add(item.Key);
    }

    public void Clear()
    {
        _inner.Clear();
        _order.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return _inner.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var key in _order)
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, _inner[key]);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var key in _order)
            yield return new KeyValuePair<TKey, TValue>(key, _inner[key]);
    }

    private int IndexOf(TKey key)
    {
        for (var i = 0; i < _order.Count; ++i)
            if (_inner.Comparer.Equals(_order[i], key))
                return i;
        return -1;
    }

    private void RemoveKeyFromOrderList(TKey key)
    {
        var index = IndexOf(key);
        if (index < 0)
            throw new InvalidOperationException($"Key {key} not found in order list");
        _order.RemoveAt(index);
    }

    public bool Remove(TKey key)
    {
        if (_inner.Remove(key))
        {
            RemoveKeyFromOrderList(key);
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (((ICollection<KeyValuePair<TKey, TValue>>)_inner).Remove(item))
        {
            RemoveKeyFromOrderList(item.Key);
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _inner.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(KeyValuePair<TKey, TValue> item)
    {
        var index = IndexOf(item.Key);
        if (index >= 0)
        {
            var value = _inner[item.Key];
            if (EqualityComparer<TValue>.Default.Equals(value, item.Value))
                return index;
        }
        return -1;
    }

    public void Insert(int index, KeyValuePair<TKey, TValue> item)
    {
        _inner.Add(item.Key, item.Value);
        _order.Insert(index, item.Key);
    }

    public void RemoveAt(int index)
    {
        var key = _order[index];
        _order.RemoveAt(index);
        _inner.Remove(key);
    }
}

