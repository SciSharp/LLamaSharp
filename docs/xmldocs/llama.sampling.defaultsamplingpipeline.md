[`< Back`](./)

---

# DefaultSamplingPipeline

Namespace: LLama.Sampling

An implementation of ISamplePipeline which mimics the default llama.cpp sampling

```csharp
public class DefaultSamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [DefaultSamplingPipeline](./llama.sampling.defaultsamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **LogitBias**

Bias values to add to certain logits

```csharp
public IReadOnlyDictionary<LLamaToken, float> LogitBias { get; init; }
```

#### Property Value

[IReadOnlyDictionary&lt;LLamaToken, Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>

### **RepeatPenalty**

Repetition penalty, as described in https://arxiv.org/abs/1909.05858

```csharp
public float RepeatPenalty { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **FrequencyPenalty**

Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
 so far, decreasing the model's likelihood to repeat the same line verbatim.

```csharp
public float FrequencyPenalty { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PresencePenalty**

Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
 text so far, increasing the model's likelihood to talk about new topics.

```csharp
public float PresencePenalty { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenaltyCount**

How many tokens should be considered for penalties

```csharp
public int PenaltyCount { get; init; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **PenalizeNewline**

Whether the newline token should be protected from being modified by penalty

```csharp
public bool PenalizeNewline { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **PreventEOS**

Whether the EOS token should be suppressed. Setting this to 'true' prevents EOS from being sampled

```csharp
public bool PreventEOS { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Temperature**

Temperature to apply (higher temperature is more "creative")

```csharp
public float Temperature { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TopK**

Number of tokens to keep in TopK sampling

```csharp
public int TopK { get; init; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TypicalP**

P value for locally typical sampling

```csharp
public float TypicalP { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TopP**

P value for TopP sampling

```csharp
public float TopP { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MinP**

P value for MinP sampling

```csharp
public float MinP { get; init; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Grammar**

Grammar to apply to constrain possible tokens

```csharp
public Grammar? Grammar { get; init; }
```

#### Property Value

[Grammar](./llama.sampling.grammar.md)<br>

### **MinKeep**

The minimum number of tokens to keep for samplers which remove tokens

```csharp
public int MinKeep { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Seed**

Seed to use for random sampling

```csharp
public uint Seed { get; set; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **GrammarOptimization**

Selected grammar optimization mode

```csharp
public GrammarOptimizationMode GrammarOptimization { get; init; }
```

#### Property Value

[GrammarOptimizationMode](./llama.sampling.defaultsamplingpipeline.grammaroptimizationmode.md)<br>

## Constructors

### **DefaultSamplingPipeline()**

```csharp
public DefaultSamplingPipeline()
```

## Methods

### **Dispose()**

```csharp
public override void Dispose()
```

### **Reset()**

Reset all internal state of the sampling pipeline

```csharp
public override void Reset()
```

### **Accept(LLamaToken)**

Update the pipeline, with knowledge that a particular token was just accepted

```csharp
public override void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **CreateChain(SafeLLamaContextHandle)**

Create a sampling chain. This will be called once, the base class will automatically dispose the chain.

```csharp
protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

### **Sample(SafeLLamaContextHandle, Int32)**

Sample a single token from the given context at the given position

```csharp
public override LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
The context being sampled from

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Position to sample logits from

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
