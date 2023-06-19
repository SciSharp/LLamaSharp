# DictionaryExtension

Namespace: LLama.Extensions

```csharp
public static class DictionaryExtension
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryExtension](./llama.extensions.dictionaryextension.md)

## Methods

### **Deconstruct&lt;T1, T2&gt;(KeyValuePair&lt;T1, T2&gt;, T1&, T2&)**

```csharp
public static void Deconstruct<T1, T2>(KeyValuePair<T1, T2> pair, T1& first, T2& second)
```

#### Type Parameters

`T1`<br>

`T2`<br>

#### Parameters

`pair` KeyValuePair&lt;T1, T2&gt;<br>

`first` T1&<br>

`second` T2&<br>

### **Update&lt;T1, T2&gt;(Dictionary&lt;T1, T2&gt;, IDictionary&lt;T1, T2&gt;)**

```csharp
public static void Update<T1, T2>(Dictionary<T1, T2> dic, IDictionary<T1, T2> other)
```

#### Type Parameters

`T1`<br>

`T2`<br>

#### Parameters

`dic` Dictionary&lt;T1, T2&gt;<br>

`other` IDictionary&lt;T1, T2&gt;<br>

### **GetOrDefault&lt;T1, T2&gt;(Dictionary&lt;T1, T2&gt;, T1, T2)**

```csharp
public static T2 GetOrDefault<T1, T2>(Dictionary<T1, T2> dic, T1 key, T2 defaultValue)
```

#### Type Parameters

`T1`<br>

`T2`<br>

#### Parameters

`dic` Dictionary&lt;T1, T2&gt;<br>

`key` T1<br>

`defaultValue` T2<br>

#### Returns

T2<br>
