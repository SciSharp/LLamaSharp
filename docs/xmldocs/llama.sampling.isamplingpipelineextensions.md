[`< Back`](./)

---

# ISamplingPipelineExtensions

Namespace: LLama.Sampling

Extension methods for [ISamplingPipeline](./llama.sampling.isamplingpipeline.md)

```csharp
public static class ISamplingPipelineExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ISamplingPipelineExtensions](./llama.sampling.isamplingpipelineextensions.md)<br>
Attributes [ExtensionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **Sample(ISamplingPipeline, LLamaContext, Int32)**

Sample a single token from the given context at the given position

```csharp
public static LLamaToken Sample(ISamplingPipeline pipe, LLamaContext ctx, int index)
```

#### Parameters

`pipe` [ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

`ctx` [LLamaContext](./llama.llamacontext.md)<br>
The context being sampled from

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Position to sample logits from

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
