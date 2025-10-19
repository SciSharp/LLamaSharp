[`< Back`](./)

---

# IInferenceParams

Namespace: LLama.Abstractions

The parameters used for inference.

```csharp
public interface IInferenceParams
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **TokensKeep**

number of tokens to keep from initial prompt

```csharp
public abstract int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to infinitely generate response
 until it complete.

```csharp
public abstract int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public abstract IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **SamplingPipeline**

Set a custom sampling pipeline to use.

```csharp
public abstract ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

### **DecodeSpecialTokens**

If true, special characters will be converted to text. If false they will be invisible.

```csharp
public abstract bool DecodeSpecialTokens { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
