[`< Back`](./)

---

# GreedySamplingPipeline

Namespace: LLama.Sampling

A sampling pipeline which always selects the most likely token

```csharp
public class GreedySamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [GreedySamplingPipeline](./llama.sampling.greedysamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Grammar**

Grammar to apply to constrain possible tokens

```csharp
public Grammar Grammar { get; set; }
```

#### Property Value

[Grammar](./llama.sampling.grammar.md)<br>

## Constructors

### **GreedySamplingPipeline()**

```csharp
public GreedySamplingPipeline()
```

## Methods

### **CreateChain(SafeLLamaContextHandle)**

```csharp
protected SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

---

[`< Back`](./)
