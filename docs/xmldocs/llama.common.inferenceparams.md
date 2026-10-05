[`< Back`](./)

---

# InferenceParams

Namespace: LLama.Common

The parameters used for inference.

```csharp
public record class InferenceParams : LLama.Abstractions.IInferenceParams, System.IEquatable`1[[LLama.Common.InferenceParams, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InferenceParams](./llama.common.inferenceparams.md)<br>
Implements [IInferenceParams](./llama.abstractions.iinferenceparams.md), [IEquatable&lt;InferenceParams&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **TokensKeep**

number of tokens to keep from initial prompt when applying context shifting

```csharp
public int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to infinitely generate response
 until it complete.

```csharp
public int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **SamplingPipeline**

Set a custom sampling pipeline to use.

```csharp
public ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

### **DecodeSpecialTokens**

If true, special characters will be converted to text. If false they will be invisible.

```csharp
public bool DecodeSpecialTokens { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **OverflowStrategy**

Defines the strategy the executor should use when the context window is full 
 and the model architecture does not support native memory shifting. 
 Defaults to [ContextOverflowStrategy.ThrowException](./llama.common.contextoverflowstrategy.md#throwexception) to prevent 
 unintended data loss and latency spikes.

```csharp
public ContextOverflowStrategy OverflowStrategy { get; set; }
```

#### Property Value

[ContextOverflowStrategy](./llama.common.contextoverflowstrategy.md)<br>

### **ContextTruncationPercentage**

The percentage of past tokens to discard when [InferenceParams.OverflowStrategy](./llama.common.inferenceparams.md#overflowstrategy) 
 is set to [ContextOverflowStrategy.TruncateAndReprefill](./llama.common.contextoverflowstrategy.md#truncateandreprefill). 
 Defaults to 0.1f (10%). Valid range is typically between 0.01f and 0.99f.

```csharp
public float ContextTruncationPercentage { get; set; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

## Constructors

### **InferenceParams(InferenceParams)**

```csharp
protected InferenceParams(InferenceParams original)
```

#### Parameters

`original` [InferenceParams](./llama.common.inferenceparams.md)<br>

### **InferenceParams()**

```csharp
public InferenceParams()
```

---

[`< Back`](./)
