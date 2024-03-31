# LLamaCache

Namespace: LLama

```csharp
public class LLamaCache
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaCache](./llama.llamacache.md)

## Properties

### **CacheSize**

```csharp
public int CacheSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**

```csharp
public LLamaState Item { get; set; }
```

#### Property Value

[LLamaState](./llama.llamastate.md)<br>

## Constructors

### **LLamaCache(Int32)**

```csharp
public LLamaCache(int capacity)
```

#### Parameters

`capacity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **Contains(Int32[])**

```csharp
public bool Contains(Int32[] key)
```

#### Parameters

`key` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
