# LLamaModelMetadataOverride

Namespace: LLama.Native

Override a key/value pair in the llama model metadata (llama_model_kv_override)

```csharp
public struct LLamaModelMetadataOverride
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelMetadataOverride](./llama.native.llamamodelmetadataoverride.md)

## Fields

### **key**

Key to override

```csharp
public <key>e__FixedBuffer key;
```

### **Tag**

Type of value

```csharp
public LLamaModelKvOverrideType Tag;
```

### **IntValue**

Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_INT

```csharp
public long IntValue;
```

### **FloatValue**

Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_FLOAT

```csharp
public double FloatValue;
```

### **BoolValue**

Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_BOOL

```csharp
public long BoolValue;
```
