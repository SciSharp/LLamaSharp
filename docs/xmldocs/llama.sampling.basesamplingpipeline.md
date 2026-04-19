[`< Back`](./)

---

# BaseSamplingPipeline

Namespace: LLama.Sampling

```csharp
public abstract class BaseSamplingPipeline : ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

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
public void Dispose()
```

### **Sample(SafeLLamaContextHandle, Int32)**

```csharp
public LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Apply(SafeLLamaContextHandle, LLamaTokenDataArray)**

```csharp
public void Apply(SafeLLamaContextHandle ctx, LLamaTokenDataArray data)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

### **Apply(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Apply this sampling chain to a LLamaTokenDataArrayNative

```csharp
public void Apply(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& data)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

### **Reset()**

```csharp
public void Reset()
```

### **Accept(LLamaToken)**

```csharp
public void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
