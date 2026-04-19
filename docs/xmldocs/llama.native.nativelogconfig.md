[`< Back`](./)

---

# NativeLogConfig

Namespace: LLama.Native

Configure llama.cpp logging

```csharp
public static class NativeLogConfig
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLogConfig](./llama.native.nativelogconfig.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Methods

### **llama_log_set(LLamaLogCallback)**

Register a callback to receive llama log messages

```csharp
public static void llama_log_set(LLamaLogCallback logCallback)
```

#### Parameters

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

### **llama_log_set(ILogger)**

Register a callback to receive llama log messages

```csharp
public static void llama_log_set(ILogger logger)
```

#### Parameters

`logger` ILogger<br>

---

[`< Back`](./)
