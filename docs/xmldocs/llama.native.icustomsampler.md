[`< Back`](./)

---

# ICustomSampler

Namespace: LLama.Native

A custom sampler stage for modifying logits or selecting a token

```csharp
public interface ICustomSampler : System.IDisposable
```

Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **Name**

The human readable name of this stage

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Apply(LLamaTokenDataArrayNative&)**

Apply this stage to a set of logits.
 This can modify logits or select a token (or both).
 If logits are modified the Sorted flag must be set to false.

```csharp
void Apply(LLamaTokenDataArrayNative& tokenData)
```

#### Parameters

`tokenData` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

**Remarks:**

If the logits are no longer sorted after the custom sampler has run it is critically important to
 set Sorted=false. If unsure, always set it to false, this is a safe default.

### **Accept(LLamaToken)**

Update the internal state of the sampler when a token is chosen

```csharp
void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **Reset()**

Reset the internal state of this sampler

```csharp
void Reset()
```

### **Clone()**

Create a clone of this sampler

```csharp
ICustomSampler Clone()
```

#### Returns

[ICustomSampler](./llama.native.icustomsampler.md)<br>

---

[`< Back`](./)
