[`< Back`](./)

---

# BaseSamplingPipeline

Namespace: LLama.Sampling

Convert a span of logits into a single sampled token. This interface can be implemented to completely customise the sampling process.

```csharp
public abstract class BaseSamplingPipeline : ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Constructors

### **BaseSamplingPipeline()**

Create a new sampler wrapping a llama.cpp sampler chain

```csharp
public BaseSamplingPipeline()
```

## Methods

### **CreateChain(SafeLLamaContextHandle)**

Create a sampling chain. This will be called once, the base class will automatically dispose the chain.

```csharp
protected abstract SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

### **Dispose()**

```csharp
public virtual void Dispose()
```

### **Sample(SafeLLamaContextHandle, Int32)**

Sample a single token from the given context at the given position

```csharp
public virtual LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Position to sample logits from

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Apply(SafeLLamaContextHandle, LLamaTokenDataArray)**

Apply this pipeline to a set of token data

```csharp
public virtual void Apply(SafeLLamaContextHandle ctx, LLamaTokenDataArray data)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

### **Apply(SafeLLamaContextHandle, ref LLamaTokenDataArrayNative)**

Apply this sampling chain to a LLamaTokenDataArrayNative

```csharp
public virtual void Apply(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative data)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`ref` `data` [LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)<br>

### **Reset()**

Reset all internal state of the sampling pipeline

```csharp
public virtual void Reset()
```

### **Accept(LLamaToken)**

Update the pipeline, with knowledge that a particular token was just accepted

```csharp
public virtual void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
