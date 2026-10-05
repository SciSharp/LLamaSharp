[`< Back`](./)

---

# LLamaToken

Namespace: LLama.Native

A single token

```csharp
public readonly record struct LLamaToken
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaToken](./llama.native.llamatoken.md)<br>
Implements [IEquatable&lt;LLamaToken&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [IsReadOnlyAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isreadonlyattribute), [DebuggerDisplayAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute)

## Fields

### **InvalidToken**

Token Value used when token is inherently null

```csharp
public static LLamaToken InvalidToken;
```

## Methods

### **GetAttributes(SafeLlamaModelHandle)**

Get attributes for this token

```csharp
public LLamaTokenAttr GetAttributes(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaTokenAttr](./llama.native.llamatokenattr.md)<br>

### **GetAttributes(Vocabulary)**

Get attributes for this token

```csharp
public LLamaTokenAttr GetAttributes(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[LLamaTokenAttr](./llama.native.llamatokenattr.md)<br>

### **GetScore(Vocabulary)**

Get score for this token

```csharp
public float GetScore(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **IsControl(SafeLlamaModelHandle)**

Check if this is a control token

```csharp
public bool IsControl(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsControl(Vocabulary)**

Check if this is a control token

```csharp
public bool IsControl(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsEndOfGeneration(SafeLlamaModelHandle)**

Check if this token should end generation

```csharp
public bool IsEndOfGeneration(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsEndOfGeneration(Vocabulary)**

Check if this token should end generation

```csharp
public bool IsEndOfGeneration(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Operators

### **explicit operator int(LLamaToken)**

Convert a LLamaToken into an integer (extract the raw value)

```csharp
public static explicit operator int(LLamaToken pos)
```

#### Parameters

`pos` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **implicit operator LLamaToken(Int32)**

Convert an integer into a LLamaToken

```csharp
public static implicit operator LLamaToken(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
