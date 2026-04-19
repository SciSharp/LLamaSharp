[`< Back`](./)

---

# LLamaToken

Namespace: LLama.Native

A single token

```csharp
public struct LLamaToken
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaToken](./llama.native.llamatoken.md)<br>
Implements [IEquatable&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [IsReadOnlyAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isreadonlyattribute), [DebuggerDisplayAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute)

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
LLamaTokenAttr GetAttributes(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaTokenAttr](./llama.native.llamatokenattr.md)<br>

### **GetAttributes(Vocabulary)**

Get attributes for this token

```csharp
LLamaTokenAttr GetAttributes(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[LLamaTokenAttr](./llama.native.llamatokenattr.md)<br>

### **GetScore(Vocabulary)**

Get score for this token

```csharp
float GetScore(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **IsControl(SafeLlamaModelHandle)**

Check if this is a control token

```csharp
bool IsControl(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsControl(Vocabulary)**

Check if this is a control token

```csharp
bool IsControl(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsEndOfGeneration(SafeLlamaModelHandle)**

Check if this token should end generation

```csharp
bool IsEndOfGeneration(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsEndOfGeneration(Vocabulary)**

Check if this token should end generation

```csharp
bool IsEndOfGeneration(Vocabulary vocab)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(LLamaToken)**

```csharp
bool Equals(LLamaToken other)
```

#### Parameters

`other` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
