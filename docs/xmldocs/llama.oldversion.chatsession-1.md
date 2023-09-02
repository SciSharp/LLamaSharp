# ChatSession&lt;T&gt;

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class ChatSession<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatSession&lt;T&gt;](./llama.oldversion.chatsession-1.md)

## Constructors

### **ChatSession(T)**

```csharp
public ChatSession(T model)
```

#### Parameters

`model` T<br>

## Methods

### **Chat(String, String, String)**

```csharp
public IEnumerable<string> Chat(string text, string prompt, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **WithPrompt(String, String)**

```csharp
public ChatSession<T> WithPrompt(string prompt, string encoding)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.oldversion.chatsession-1.md)<br>

### **WithPromptFile(String, String)**

```csharp
public ChatSession<T> WithPromptFile(string promptFilename, string encoding)
```

#### Parameters

`promptFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.oldversion.chatsession-1.md)<br>

### **WithAntiprompt(String[])**

Set the keywords to split the return value of chat AI.

```csharp
public ChatSession<T> WithAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.oldversion.chatsession-1.md)<br>
