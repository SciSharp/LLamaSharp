# InferenceParams

Namespace: LLama.Common

The parameters used for inference.

```csharp
public class InferenceParams : LLama.Abstractions.IInferenceParams, System.IEquatable`1[[LLama.Common.InferenceParams, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [InferenceParams](./llama.common.inferenceparams.md)<br>
Implements [IInferenceParams](./llama.abstractions.iinferenceparams.md), [IEquatable&lt;InferenceParams&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

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
public Dictionary<LLamaToken, float> LogitBias { get; set; }
```

#### Property Value

[Dictionary&lt;LLamaToken, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **TopK**

```csharp
public int TopK { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TopP**

```csharp
public float TopP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MinP**

```csharp
public float MinP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TfsZ**

```csharp
public float TfsZ { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **TypicalP**

```csharp
public float TypicalP { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Temperature**

```csharp
public float Temperature { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatPenalty**

```csharp
public float RepeatPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RepeatLastTokensCount**

```csharp
public int RepeatLastTokensCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **FrequencyPenalty**

```csharp
public float FrequencyPenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PresencePenalty**

```csharp
public float PresencePenalty { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Mirostat**

```csharp
public MirostatType Mirostat { get; set; }
```

#### Property Value

[MirostatType](./llama.common.mirostattype.md)<br>

### **MirostatTau**

```csharp
public float MirostatTau { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MirostatEta**

```csharp
public float MirostatEta { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **PenalizeNL**

```csharp
public bool PenalizeNL { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Grammar**

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **SamplingPipeline**

```csharp
public ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

## Constructors

### **InferenceParams()**

```csharp
public InferenceParams()
```

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(InferenceParams)**

```csharp
public bool Equals(InferenceParams other)
```

#### Parameters

`other` [InferenceParams](./llama.common.inferenceparams.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public InferenceParams <Clone>$()
```

#### Returns

[InferenceParams](./llama.common.inferenceparams.md)<br>
