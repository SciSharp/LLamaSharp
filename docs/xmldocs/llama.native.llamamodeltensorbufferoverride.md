[`< Back`](./)

---

# LLamaModelTensorBufferOverride

Namespace: LLama.Native

Represents a mapping between a tensor name pattern and a backend buffer type<br>
 Original type: llama_model_tensor_buft_override

```csharp
public struct LLamaModelTensorBufferOverride
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelTensorBufferOverride](./llama.native.llamamodeltensorbufferoverride.md)

## Fields

### **Pattern**

Tensor name pattern to match

```csharp
public Byte* Pattern;
```

### **BufferType**

Backend buffer type to use for matching tensors, as obtained via ggml_backend_dev_buffer_type

```csharp
public IntPtr BufferType;
```

---

[`< Back`](./)
