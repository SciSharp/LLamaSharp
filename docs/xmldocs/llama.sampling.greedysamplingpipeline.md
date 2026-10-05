[`< Back`](./)

---

# GreedySamplingPipeline

Namespace: LLama.Sampling

A sampling pipeline which always selects the most likely token

```csharp
public class GreedySamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [GreedySamplingPipeline](./llama.sampling.greedysamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Grammar**

Grammar to apply to constrain possible tokens

```csharp
public Grammar? Grammar { get; init; }
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

Create a sampling chain. This will be called once, the base class will automatically dispose the chain.

```csharp
protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

---

[`< Back`](./)
