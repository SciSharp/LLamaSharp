# LLamaTokenData

Namespace: LLama.Native

```csharp
public struct LLamaTokenData
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenData](./llama.native.llamatokendata.md)

## Fields

### **id**

token id

```csharp
public int id;
```

### **logit**

log-odds of the token

```csharp
public float logit;
```

### **p**

probability of the token

```csharp
public float p;
```

## Constructors

### **LLamaTokenData(Int32, Single, Single)**

```csharp
LLamaTokenData(int id, float logit, float p)
```

#### Parameters

`id` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logit` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
