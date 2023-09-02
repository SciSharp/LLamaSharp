# CompletionUsage

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class CompletionUsage : System.IEquatable`1[[LLama.OldVersion.CompletionUsage, LLamaSharp, Version=0.5.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CompletionUsage](./llama.oldversion.completionusage.md)<br>
Implements [IEquatable&lt;CompletionUsage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **PromptTokens**

```csharp
public int PromptTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **CompletionTokens**

```csharp
public int CompletionTokens { get; set; }
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

### **CompletionUsage(Int32, Int32, Int32)**

```csharp
public CompletionUsage(int PromptTokens, int CompletionTokens, int TotalTokens)
```

#### Parameters

`PromptTokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`CompletionTokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **Equals(CompletionUsage)**

```csharp
public bool Equals(CompletionUsage other)
```

#### Parameters

`other` [CompletionUsage](./llama.oldversion.completionusage.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public CompletionUsage <Clone>$()
```

#### Returns

[CompletionUsage](./llama.oldversion.completionusage.md)<br>

### **Deconstruct(Int32&, Int32&, Int32&)**

```csharp
public void Deconstruct(Int32& PromptTokens, Int32& CompletionTokens, Int32& TotalTokens)
```

#### Parameters

`PromptTokens` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`CompletionTokens` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`TotalTokens` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
