[`< Back`](./)

---

# LLamaLogitBias

Namespace: LLama.Native

A bias to apply directly to a logit

```csharp
public struct LLamaLogitBias
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaLogitBias](./llama.native.llamalogitbias.md)<br>
Implements [IEquatable&lt;LLamaLogitBias&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Token**

The token to apply the bias to

```csharp
public LLamaToken Token;
```

### **Bias**

The bias to add

```csharp
public float Bias;
```

## Methods

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(LLamaLogitBias)**

```csharp
bool Equals(LLamaLogitBias other)
```

#### Parameters

`other` [LLamaLogitBias](./llama.native.llamalogitbias.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
