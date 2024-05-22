# IInferenceParams

Namespace: LLama.Abstractions

The parameters used for inference.

```csharp
public interface IInferenceParams
```

## Properties

### **TokensKeep**

number of tokens to keep from initial prompt

```csharp
public abstract int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
 until it complete.

```csharp
public abstract int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LogitBias**

logit bias for specific tokens

```csharp
public abstract Dictionary<LLamaToken, float> LogitBias { get; set; }
```

#### Property Value

[Dictionary&lt;LLamaToken, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public abstract IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **TopK**

0 or lower to use vocab size

```csharp
public abstract int TopK { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TopP**

1.0 = disabled

```csharp
public abstract float TopP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MinP**

0.0 = disabled

```csharp
public abstract float MinP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TfsZ**

1.0 = disabled

```csharp
public abstract float TfsZ { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TypicalP**

1.0 = disabled

```csharp
public abstract float TypicalP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Temperature**

1.0 = disabled

```csharp
public abstract float Temperature { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatPenalty**

1.0 = disabled

```csharp
public abstract float RepeatPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatLastTokensCount**

last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)

```csharp
public abstract int RepeatLastTokensCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **FrequencyPenalty**

frequency penalty coefficient
 0.0 = disabled

```csharp
public abstract float FrequencyPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PresencePenalty**

presence penalty coefficient
 0.0 = disabled

```csharp
public abstract float PresencePenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Mirostat**

Mirostat uses tokens instead of words.
 algorithm described in the paper https://arxiv.org/abs/2007.14966.
 0 = disabled, 1 = mirostat, 2 = mirostat 2.0

```csharp
public abstract MirostatType Mirostat { get; set; }
```

#### Property Value

[MirostatType](./llama.common.mirostattype.md)<br>

### **MirostatTau**

target entropy

```csharp
public abstract float MirostatTau { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MirostatEta**

learning rate

```csharp
public abstract float MirostatEta { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenalizeNL**

consider newlines as a repeatable token (penalize_nl)

```csharp
public abstract bool PenalizeNL { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Grammar**

Grammar to constrain possible tokens

```csharp
public abstract SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **SamplingPipeline**

Set a custom sampling pipeline to use. If this is set All other sampling parameters are ignored!

```csharp
public abstract ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>
