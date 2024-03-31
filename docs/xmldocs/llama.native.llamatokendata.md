# LLamaTokenData

Namespace: LLama.Native

A single token along with probability of this token being selected

```csharp
public struct LLamaTokenData
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenData](./llama.native.llamatokendata.md)

## Fields

### **id**

token id

```csharp
public LLamaToken id;
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

### **LLamaTokenData(LLamaToken, Single, Single)**

Create a new LLamaTokenData

```csharp
LLamaTokenData(LLamaToken id, float logit, float p)
```

#### Parameters

`id` [LLamaToken](./llama.native.llamatoken.md)<br>

`logit` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
