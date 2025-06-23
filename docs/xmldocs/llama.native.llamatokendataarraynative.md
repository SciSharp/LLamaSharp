[`< Back`](./)

---

# LLamaTokenDataArrayNative

Namespace: LLama.Native

Contains a pointer to an array of LLamaTokenData which is pinned in memory.

```csharp
public struct LLamaTokenDataArrayNative
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)

**Remarks:**

C# equivalent of llama_token_data_array

## Properties

### **Data**

A pointer to an array of LlamaTokenData

```csharp
public Span<LLamaTokenData> Data { get; }
```

#### Property Value

[Span&lt;LLamaTokenData&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **Sorted**

Indicates if the items in the array are sorted, so the most likely token is first

```csharp
public bool Sorted { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Selected**

The index of the selected token (i.e. not the token id)

```csharp
public long Selected { get; set; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Size**

Number of LLamaTokenData in the array. Set this to shrink the array

```csharp
public ulong Size { get; set; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

## Methods

### **Create(LLamaTokenDataArray, LLamaTokenDataArrayNative&)**

Create a new LLamaTokenDataArrayNative around the data in the LLamaTokenDataArray

```csharp
MemoryHandle Create(LLamaTokenDataArray array, LLamaTokenDataArrayNative& native)
```

#### Parameters

`array` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Data source

`native` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Created native array

#### Returns

[MemoryHandle](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.memoryhandle)<br>
A memory handle, pinning the data in place until disposed

---

[`< Back`](./)
