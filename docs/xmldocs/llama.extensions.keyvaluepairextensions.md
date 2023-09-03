# KeyValuePairExtensions

Namespace: LLama.Extensions

Extensions to the KeyValuePair struct

```csharp
public static class KeyValuePairExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [KeyValuePairExtensions](./llama.extensions.keyvaluepairextensions.md)

## Methods

### **Deconstruct&lt;TKey, TValue&gt;(KeyValuePair&lt;TKey, TValue&gt;, TKey&, TValue&)**

Deconstruct a KeyValuePair into it's constituent parts.

```csharp
public static void Deconstruct<TKey, TValue>(KeyValuePair<TKey, TValue> pair, TKey& first, TValue& second)
```

#### Type Parameters

`TKey`<br>
Type of the Key

`TValue`<br>
Type of the Value

#### Parameters

`pair` KeyValuePair&lt;TKey, TValue&gt;<br>
The KeyValuePair to deconstruct

`first` TKey&<br>
First element, the Key

`second` TValue&<br>
Second element, the Value
