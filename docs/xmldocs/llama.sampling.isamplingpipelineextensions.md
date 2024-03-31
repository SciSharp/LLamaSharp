# ISamplingPipelineExtensions

Namespace: LLama.Sampling

Extensions methods for ISamplingPipeline

```csharp
public static class ISamplingPipelineExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ISamplingPipelineExtensions](./llama.sampling.isamplingpipelineextensions.md)

## Methods

### **Sample(ISamplingPipeline, SafeLLamaContextHandle, Span&lt;Single&gt;, List&lt;LLamaToken&gt;)**

Sample a single token from the given logits

```csharp
public static LLamaToken Sample(ISamplingPipeline pipeline, SafeLLamaContextHandle ctx, Span<float> logits, List<LLamaToken> lastTokens)
```

#### Parameters

`pipeline` [ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The logits produced by the model

`lastTokens` [List&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
A list of tokens recently returned by the model

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>
