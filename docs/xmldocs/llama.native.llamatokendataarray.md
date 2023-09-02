# LLamaTokenDataArray

Namespace: LLama.Native

Contains an array of LLamaTokenData, potentially sorted.

```csharp
public struct LLamaTokenDataArray
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)

## Fields

### **data**

The LLamaTokenData

```csharp
public Memory<LLamaTokenData> data;
```

### **sorted**

Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.

```csharp
public bool sorted;
```

## Constructors

### **LLamaTokenDataArray(Memory&lt;LLamaTokenData&gt;, Boolean)**

Create a new LLamaTokenDataArray

```csharp
LLamaTokenDataArray(Memory<LLamaTokenData> tokens, bool isSorted)
```

#### Parameters

`tokens` [Memory&lt;LLamaTokenData&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`isSorted` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Create(ReadOnlySpan&lt;Single&gt;)**

Create a new LLamaTokenDataArray, copying the data from the given logits

```csharp
LLamaTokenDataArray Create(ReadOnlySpan<float> logits)
```

#### Parameters

`logits` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
