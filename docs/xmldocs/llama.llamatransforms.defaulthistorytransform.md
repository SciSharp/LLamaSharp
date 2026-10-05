[`< Back`](./)

---

# DefaultHistoryTransform

Namespace: LLama

The default history transform.
 Uses plain text with the following format:
 [Author]: [Message]

```csharp
internal class DefaultHistoryTransform : LLama.Abstractions.IHistoryTransform
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultHistoryTransform](./llama.llamatransforms.defaulthistorytransform.md)<br>
Implements [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **UserName**

```csharp
public string UserName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **AssistantName**

```csharp
public string AssistantName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SystemName**

```csharp
public string SystemName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **UnknownName**

```csharp
public string UnknownName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **IsInstructMode**

```csharp
public bool IsInstructMode { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **DefaultHistoryTransform(String, String, String, String, Boolean)**



```csharp
public DefaultHistoryTransform(string? userName = null, string? assistantName = null, string? systemName = null, string? unknownName = null, bool isInstructMode = false)
```

#### Parameters

`userName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

`assistantName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

`systemName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

`unknownName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

`isInstructMode` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Clone()**

Copy the transform.

```csharp
public IHistoryTransform Clone()
```

#### Returns

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

### **HistoryToText(ChatHistory)**

Convert a ChatHistory instance to plain text.

```csharp
public virtual string HistoryToText(ChatHistory history)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>
The ChatHistory instance

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TextToHistory(AuthorRole, String)**

Converts plain text to a ChatHistory instance.

```csharp
public virtual ChatHistory TextToHistory(AuthorRole role, string text)
```

#### Parameters

`role` [AuthorRole](./llama.common.authorrole.md)<br>
The role for the author.

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The chat history as plain text.

#### Returns

[ChatHistory](./llama.common.chathistory.md)<br>
The updated history.

### **TrimNamesFromText(String, AuthorRole)**

Drop the name at the beginning and the end of the text.

```csharp
public virtual string TrimNamesFromText(string text, AuthorRole role)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`role` [AuthorRole](./llama.common.authorrole.md)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

---

[`< Back`](./)
