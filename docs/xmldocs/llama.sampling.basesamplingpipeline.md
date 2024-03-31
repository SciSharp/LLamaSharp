# BaseSamplingPipeline

Namespace: LLama.Sampling

Base class for implementing custom sampling pipelines. This provides a helpful framework for implementing `ISamplingPipeline`.

```csharp
public abstract class BaseSamplingPipeline : ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Grammar**

Grammar to constrain valid tokens

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

## Methods

### **Sample(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

```csharp
public LLamaToken Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Accept(SafeLLamaContextHandle, LLamaToken)**

```csharp
public void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **ProcessLogits(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

Process the raw logit values

```csharp
protected abstract void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The logits produced by the model

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A list of tokens recently returned by the model

### **ProcessTokenDataArray(SafeLLamaContextHandle, LLamaTokenDataArray, ReadOnlySpan&lt;LLamaToken&gt;)**

Process the LLamaTokenDataArray and select a single token

```csharp
protected abstract LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
The LLamaTokenDataArray data produced by the model

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A list of tokens recently returned by the model

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Reset()**

```csharp
public void Reset()
```

### **Clone()**

```csharp
public abstract ISamplingPipeline Clone()
```

#### Returns

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

### **Dispose()**

```csharp
public void Dispose()
```
