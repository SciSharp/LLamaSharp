# ISamplingPipeline

Namespace: LLama.Sampling

Convert a span of logits into a single sampled token. This interface can be implemented to completely customise the sampling process.

```csharp
public interface ISamplingPipeline : System.IDisposable
```

Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Methods

### **Sample(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

Sample a single token from the given logits

```csharp
LLamaToken Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The logits produced by the model

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A span of tokens recently returned by the model

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Accept(SafeLLamaContextHandle, LLamaToken)**

Update the pipeline, with knowledge that a particular token was just accepted

```csharp
void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **Reset()**

Reset all internal state of the sampling pipeline

```csharp
void Reset()
```

### **Clone()**

Create a copy of this sampling pipeline

```csharp
ISamplingPipeline Clone()
```

#### Returns

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>
