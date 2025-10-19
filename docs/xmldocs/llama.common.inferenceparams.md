[`< Back`](./)

---

# InferenceParams

Namespace: LLama.Common

The parameters used for inference.

```csharp
public class InferenceParams : LLama.Abstractions.IInferenceParams, System.IEquatable`1[[LLama.Common.InferenceParams, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [InferenceParams](./llama.common.inferenceparams.md)<br>
Implements [IInferenceParams](./llama.abstractions.iinferenceparams.md), [IEquatable&lt;InferenceParams&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EqualityContract**

```csharp
protected Type EqualityContract { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **TokensKeep**

number of tokens to keep from initial prompt when applying context shifting

```csharp
public int TokensKeep { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxTokens**

how many new tokens to predict (n_predict), set to -1 to infinitely generate response
 until it complete.

```csharp
public int MaxTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AntiPrompts**

Sequences where the model will stop generating further tokens.

```csharp
public IReadOnlyList<string> AntiPrompts { get; set; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **SamplingPipeline**

```csharp
public ISamplingPipeline SamplingPipeline { get; set; }
```

#### Property Value

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

### **DecodeSpecialTokens**

```csharp
public bool DecodeSpecialTokens { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

---

[`< Back`](./)
