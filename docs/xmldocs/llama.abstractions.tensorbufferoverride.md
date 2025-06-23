[`< Back`](./)

---

# TensorBufferOverride

Namespace: LLama.Abstractions

Represents a mapping between a tensor name pattern and a specific buffer type

```csharp
public class TensorBufferOverride
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TensorBufferOverride](./llama.abstractions.tensorbufferoverride.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Pattern**

Pattern to match tensor names. This is a regular expression. You can check the tensor names via the model.Metadata.

```csharp
public string Pattern { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **BufferType**

Buffer type to use for matching tensors. Examples: CPU, GPU0, GPU1

```csharp
public string BufferType { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **TensorBufferOverride(String, String)**

Creates a new tensor buffer override

```csharp
public TensorBufferOverride(string pattern, string bufferType)
```

#### Parameters

`pattern` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Pattern to match tensor names

`bufferType` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Buffer type to use for matching tensors

---

[`< Back`](./)
