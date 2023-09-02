# LLamaTokenDataArrayNative

Namespace: LLama.Native

Contains a pointer to an array of LLamaTokenData which is pinned in memory.

```csharp
public struct LLamaTokenDataArrayNative
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)

## Fields

### **data**

A pointer to an array of LlamaTokenData

```csharp
public IntPtr data;
```

**Remarks:**

Memory must be pinned in place for all the time this LLamaTokenDataArrayNative is in use

### **size**

Number of LLamaTokenData in the array

```csharp
public ulong size;
```

## Properties

### **sorted**

Indicates if the items in the array are sorted

```csharp
public bool sorted { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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
