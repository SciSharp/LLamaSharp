# EmbeddingUsage

Namespace: LLama.Types

```csharp
public class EmbeddingUsage : System.IEquatable`1[[LLama.Types.EmbeddingUsage, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [EmbeddingUsage](./llama.types.embeddingusage.md)<br>
Implements [IEquatable&lt;EmbeddingUsage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **PromptTokens**

```csharp
public int PromptTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TotalTokens**

```csharp
public int TotalTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **EmbeddingUsage(Int32, Int32)**

```csharp
public EmbeddingUsage(int PromptTokens, int TotalTokens)
```

#### Parameters

`PromptTokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`TotalTokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **Equals(EmbeddingUsage)**

```csharp
public bool Equals(EmbeddingUsage other)
```

#### Parameters

`other` [EmbeddingUsage](./llama.types.embeddingusage.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public EmbeddingUsage <Clone>$()
```

#### Returns

[EmbeddingUsage](./llama.types.embeddingusage.md)<br>

### **Deconstruct(Int32&, Int32&)**

```csharp
public void Deconstruct(Int32& PromptTokens, Int32& TotalTokens)
```

#### Parameters

`PromptTokens` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`TotalTokens` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
