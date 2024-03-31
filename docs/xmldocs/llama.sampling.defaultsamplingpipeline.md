# DefaultSamplingPipeline

Namespace: LLama.Sampling

An implementation of ISamplePipeline which mimics the default llama.cpp sampling

```csharp
public sealed class DefaultSamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [DefaultSamplingPipeline](./llama.sampling.defaultsamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **LogitBias**

Bias values to add to certain logits

```csharp
public Dictionary<int, float> LogitBias { get; }
```

#### Property Value

[Dictionary&lt;Int32, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **RepeatPenalty**

Repetition penalty, as described in https://arxiv.org/abs/1909.05858

```csharp
public float RepeatPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **AlphaFrequency**

Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
 so far, decreasing the model's likelihood to repeat the same line verbatim.

```csharp
public float AlphaFrequency { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **AlphaPresence**

Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
 text so far, increasing the model's likelihood to talk about new topics.

```csharp
public float AlphaPresence { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Temperature**

Temperature to apply (higher temperature is more "creative")

```csharp
public float Temperature { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TopK**

Number of tokens to keep in TopK sampling

```csharp
public int TopK { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TailFreeZ**

Z value for tail free sampling

```csharp
public float TailFreeZ { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TypicalP**

P value for locally typical sampling

```csharp
public float TypicalP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TopP**

P value for TopP sampling

```csharp
public float TopP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MinP**

P value for MinP sampling

```csharp
public float MinP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenalizeNewline**

Whether the newline value should be protected from being modified by logit bias and repeat penalty

```csharp
public bool PenalizeNewline { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Grammar**

Grammar to constrain valid tokens

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

## Constructors

### **DefaultSamplingPipeline()**

```csharp
public DefaultSamplingPipeline()
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

### **Accept(SafeLLamaContextHandle, LLamaToken)**

```csharp
public void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **Clone()**

```csharp
public ISamplingPipeline Clone()
```

#### Returns

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>
