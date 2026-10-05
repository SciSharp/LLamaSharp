[`< Back`](./)

---

# Vocabulary

Namespace: LLama.Native

Get tokens for a model

```csharp
internal sealed class Vocabulary
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **Count**

Total number of tokens in this vocabulary

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Type**

Get the type of this vocabulary

```csharp
public LLamaVocabType Type { get; }
```

#### Property Value

[LLamaVocabType](./llama.native.llamavocabtype.md)<br>

### **BOS**

Get the Beginning of Sentence token for this model

```csharp
public LLamaToken? BOS { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **EOS**

Get the End of Sentence token for this model

```csharp
public LLamaToken? EOS { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Newline**

Get the newline token for this model

```csharp
public LLamaToken? Newline { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Pad**

Get the padding token for this model

```csharp
public LLamaToken? Pad { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Mask**

Get the masking token for this model

```csharp
public LLamaToken? Mask { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **SEP**

Get the sentence separator token for this model

```csharp
public LLamaToken? SEP { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillPrefix**

Codellama beginning of infill prefix

```csharp
public LLamaToken? InfillPrefix { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillMiddle**

Codellama beginning of infill middle

```csharp
public LLamaToken? InfillMiddle { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillSuffix**

Codellama beginning of infill suffix

```csharp
public LLamaToken? InfillSuffix { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillPad**

Codellama pad

```csharp
public LLamaToken? InfillPad { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillRep**

Codellama rep

```csharp
public LLamaToken? InfillRep { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **InfillSep**

Codellama rep

```csharp
public LLamaToken? InfillSep { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **EOT**

end-of-turn token

```csharp
public LLamaToken? EOT { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **DecoderStartToken**

For encoder-decoder models, this function returns id of the token that must be provided
 to the decoder to start generating output sequence.

```csharp
public LLamaToken? DecoderStartToken { get; }
```

#### Property Value

[LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **ShouldAddBOS**

Check if the current model requires a BOS token added

```csharp
public bool ShouldAddBOS { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ShouldAddEOS**

Check if the current model requires a EOS token added

```csharp
public bool ShouldAddEOS { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **LLamaTokenToString(LLamaToken?, Boolean)**

Translate LLamaToken to String

```csharp
public string? LLamaTokenToString(LLamaToken? token, bool isSpecialToken)
```

#### Parameters

`token` [LLamaToken?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

`isSpecialToken` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

---

[`< Back`](./)
