# InferenceParams

Namespace: LLama.Common

The paramters used for inference.

```csharp
public class InferenceParams : LLama.Abstractions.IInferenceParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [InferenceParams](./llama.common.inferenceparams.md)<br>
Implements [IInferenceParams](./llama.abstractions.iinferenceparams.md)

## Properties

### **TokensKeep**

number of tokens to keep from initial prompt

```csharp
public int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
 until it complete.

```csharp
public int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LogitBias**

logit bias for specific tokens

```csharp
public Dictionary<int, float> LogitBias { get; set; }
```

#### Property Value

[Dictionary&lt;Int32, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public IEnumerable<string> AntiPrompts { get; set; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **PathSession**

path to file for saving/loading model eval state

```csharp
public string PathSession { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InputSuffix**

string to suffix user inputs with

```csharp
public string InputSuffix { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InputPrefix**

string to prefix user inputs with

```csharp
public string InputPrefix { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TopK**

0 or lower to use vocab size

```csharp
public int TopK { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TopP**

1.0 = disabled

```csharp
public float TopP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TfsZ**

1.0 = disabled

```csharp
public float TfsZ { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TypicalP**

1.0 = disabled

```csharp
public float TypicalP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Temperature**

1.0 = disabled

```csharp
public float Temperature { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatPenalty**

1.0 = disabled

```csharp
public float RepeatPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatLastTokensCount**

last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)

```csharp
public int RepeatLastTokensCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **FrequencyPenalty**

frequency penalty coefficient
 0.0 = disabled

```csharp
public float FrequencyPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PresencePenalty**

presence penalty coefficient
 0.0 = disabled

```csharp
public float PresencePenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Mirostat**

Mirostat uses tokens instead of words.
 algorithm described in the paper https://arxiv.org/abs/2007.14966.
 0 = disabled, 1 = mirostat, 2 = mirostat 2.0

```csharp
public MirostatType Mirostat { get; set; }
```

#### Property Value

[MirostatType](./llama.common.mirostattype.md)<br>

### **MirostatTau**

target entropy

```csharp
public float MirostatTau { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MirostatEta**

learning rate

```csharp
public float MirostatEta { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenalizeNL**

consider newlines as a repeatable token (penalize_nl)

```csharp
public bool PenalizeNL { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Grammar**

A grammar to constrain the possible tokens

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

## Constructors

### **InferenceParams()**

```csharp
public InferenceParams()
```
