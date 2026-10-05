[`< Back`](./)

---

# IInferenceParams

Namespace: LLama.Abstractions

The parameters used for inference.

```csharp
public interface IInferenceParams
```

Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **TokensKeep**

number of tokens to keep from initial prompt

```csharp
int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to infinitely generate response
 until it complete.

```csharp
int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **SamplingPipeline**

Set a custom sampling pipeline to use.

```csharp
ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

### **DecodeSpecialTokens**

If true, special characters will be converted to text. If false they will be invisible.

```csharp
bool DecodeSpecialTokens { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **OverflowStrategy**

Defines the strategy the executor should use when the context window is full 
 and the model architecture (e.g., models with 2D RoPE embeddings) does not 
 support native memory shifting.

```csharp
ContextOverflowStrategy OverflowStrategy { get; set; }
```

#### Property Value

[ContextOverflowStrategy](./llama.common.contextoverflowstrategy.md)<br>

### **ContextTruncationPercentage**

The percentage of past tokens to discard when [IInferenceParams.OverflowStrategy](./llama.abstractions.iinferenceparams.md#overflowstrategy) 
 is set to [ContextOverflowStrategy.TruncateAndReprefill](./llama.common.contextoverflowstrategy.md#truncateandreprefill). 
 For example, 0.1f represents dropping the oldest 10% of the conversational context.

```csharp
float ContextTruncationPercentage { get; set; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

---

[`< Back`](./)
