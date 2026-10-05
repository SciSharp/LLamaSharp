[`< Back`](./)

---

# LoraAdapter

Namespace: LLama.Native

A LoRA adapter which can be applied to a context for a specific model

```csharp
public class LoraAdapter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LoraAdapter](./llama.native.loraadapter.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Model**

The model which this LoRA adapter was loaded with.

```csharp
public SafeLlamaModelHandle Model { get; }
```

#### Property Value

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

### **Path**

The full path of the file this adapter was loaded from

```csharp
public string Path { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Unload()**

Unload this adapter

```csharp
public void Unload()
```

---

[`< Back`](./)
