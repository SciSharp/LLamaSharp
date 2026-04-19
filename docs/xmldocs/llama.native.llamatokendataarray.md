[`< Back`](./)

---

# LLamaTokenDataArray

Namespace: LLama.Native

Contains an array of LLamaTokenData, potentially sorted.

```csharp
public struct LLamaTokenDataArray
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)

## Fields

### **Data**

The LLamaTokenData

```csharp
public Memory<LLamaTokenData> Data;
```

### **Sorted**

Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.

```csharp
public bool Sorted;
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

### **Create(ReadOnlySpan&lt;Single&gt;, Memory&lt;LLamaTokenData&gt;)**

Create a new LLamaTokenDataArray, copying the data from the given logits into temporary memory.

```csharp
LLamaTokenDataArray Create(ReadOnlySpan<float> logits, Memory<LLamaTokenData> buffer)
```

#### Parameters

`logits` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`buffer` [Memory&lt;LLamaTokenData&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>
Temporary memory which will be used to work on these logits. Must be at least as large as logits array

#### Returns

[LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

**Remarks:**

The memory must not be modified while this [LLamaTokenDataArray](./llama.native.llamatokendataarray.md) is in use.

### **OverwriteLogits(ReadOnlySpan&lt;ValueTuple&lt;LLamaToken, Single&gt;&gt;)**

Overwrite the logit values for all given tokens

```csharp
void OverwriteLogits(ReadOnlySpan<ValueTuple<LLamaToken, float>> values)
```

#### Parameters

`values` [ReadOnlySpan&lt;ValueTuple&lt;LLamaToken, Single&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
tuples of token and logit value to overwrite

### **Softmax()**

Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.

```csharp
void Softmax()
```

---

[`< Back`](./)
