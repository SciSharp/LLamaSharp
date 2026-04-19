[`< Back`](./)

---

# DefaultSamplingPipeline

Namespace: LLama.Sampling

An implementation of ISamplePipeline which mimics the default llama.cpp sampling

```csharp
public class DefaultSamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [DefaultSamplingPipeline](./llama.sampling.defaultsamplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **LogitBias**

Bias values to add to certain logits

```csharp
public IReadOnlyDictionary<LLamaToken, float> LogitBias { get; set; }
```

#### Property Value

[IReadOnlyDictionary&lt;LLamaToken, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>

### **RepeatPenalty**

Repetition penalty, as described in https://arxiv.org/abs/1909.05858

```csharp
public float RepeatPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **FrequencyPenalty**

Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
 so far, decreasing the model's likelihood to repeat the same line verbatim.

```csharp
public float FrequencyPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PresencePenalty**

Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br>
 Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
 text so far, increasing the model's likelihood to talk about new topics.

```csharp
public float PresencePenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenaltyCount**

How many tokens should be considered for penalties

```csharp
public int PenaltyCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **PenalizeNewline**

Whether the newline token should be protected from being modified by penalty

```csharp
public bool PenalizeNewline { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **PreventEOS**

Whether the EOS token should be suppressed. Setting this to 'true' prevents EOS from being sampled

```csharp
public bool PreventEOS { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **Grammar**

Grammar to apply to constrain possible tokens

```csharp
public Grammar Grammar { get; set; }
```

#### Property Value

[Grammar](./llama.sampling.grammar.md)<br>

### **MinKeep**

The minimum number of tokens to keep for samplers which remove tokens

```csharp
public int MinKeep { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Seed**

Seed to use for random sampling

```csharp
public uint Seed { get; set; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **GrammarOptimization**

Selected grammar optimization mode

```csharp
public GrammarOptimizationMode GrammarOptimization { get; set; }
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
public void Dispose()
```

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

### **CreateChain(SafeLLamaContextHandle)**

```csharp
protected SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

### **Sample(SafeLLamaContextHandle, Int32)**

```csharp
public LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
