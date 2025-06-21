[`< Back`](./)

---

# ISamplingPipeline

Namespace: LLama.Sampling

Convert a span of logits into a single sampled token. This interface can be implemented to completely customise the sampling process.

```csharp
public interface ISamplingPipeline : System.IDisposable
```

Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Methods

### **Sample(SafeLLamaContextHandle, Int32)**

Sample a single token from the given context at the given position

```csharp
LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Position to sample logits from

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Apply(SafeLLamaContextHandle, LLamaTokenDataArray)**

Apply this pipeline to a set of token data

```csharp
void Apply(SafeLLamaContextHandle ctx, LLamaTokenDataArray data)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

### **Reset()**

Reset all internal state of the sampling pipeline

```csharp
void Reset()
```

### **Accept(LLamaToken)**

Update the pipeline, with knowledge that a particular token was just accepted

```csharp
void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
