# LLamaTokenDataArray

Namespace: LLama.Native

```csharp
public struct LLamaTokenDataArray
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)

## Fields

### **data**

```csharp
public Memory<LLamaTokenData> data;
```

### **size**

```csharp
public ulong size;
```

### **sorted**

```csharp
public bool sorted;
```

## Constructors

### **LLamaTokenDataArray(LLamaTokenData[], UInt64, Boolean)**

```csharp
LLamaTokenDataArray(LLamaTokenData[] data, ulong size, bool sorted)
```

#### Parameters

`data` [LLamaTokenData[]](./llama.native.llamatokendata.md)<br>

`size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`sorted` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
