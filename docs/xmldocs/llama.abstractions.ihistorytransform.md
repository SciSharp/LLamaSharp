# IHistoryTransform

Namespace: LLama.Abstractions

Transform history to plain text and vice versa.

```csharp
public interface IHistoryTransform
```

## Methods

### **HistoryToText(ChatHistory)**

Convert a ChatHistory instance to plain text.

```csharp
string HistoryToText(ChatHistory history)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>
The ChatHistory instance

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TextToHistory(AuthorRole, String)**

Converts plain text to a ChatHistory instance.

```csharp
ChatHistory TextToHistory(AuthorRole role, string text)
```

#### Parameters

`role` [AuthorRole](./llama.common.authorrole.md)<br>
The role for the author.

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The chat history as plain text.

#### Returns

[ChatHistory](./llama.common.chathistory.md)<br>
The updated history.

### **Clone()**

Copy the transform.

```csharp
IHistoryTransform Clone()
```

#### Returns

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>
