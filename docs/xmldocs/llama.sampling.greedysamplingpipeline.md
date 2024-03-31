# GreedySamplingPipeline

Namespace: LLama.Sampling

A sampling pipeline which always selects the most likely token

```csharp
public class GreedySamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [GreedySamplingPipeline](./llama.sampling.greedysamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Grammar**

Grammar to constrain valid tokens

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

## Constructors

### **GreedySamplingPipeline()**

```csharp
public GreedySamplingPipeline()
```

## Methods

### **ProcessLogits(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

```csharp
protected void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **ProcessTokenDataArray(SafeLLamaContextHandle, LLamaTokenDataArray, ReadOnlySpan&lt;LLamaToken&gt;)**

```csharp
protected LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Clone()**

```csharp
public ISamplingPipeline Clone()
```

#### Returns

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>
