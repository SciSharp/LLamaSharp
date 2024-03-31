# ChatSession&lt;T&gt;

Namespace: LLama

```csharp
public class ChatSession<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatSession&lt;T&gt;](./llama.chatsession-1.md)

## Constructors

### **ChatSession(T)**

```csharp
public ChatSession(T model)
```

#### Parameters

`model` T<br>

## Methods

### **Chat(String, String)**

```csharp
public IEnumerable<string> Chat(string text, string prompt)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **WithPrompt(String)**

```csharp
public ChatSession<T> WithPrompt(string prompt)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.chatsession-1.md)<br>

### **WithPromptFile(String)**

```csharp
public ChatSession<T> WithPromptFile(string promptFilename)
```

#### Parameters

`promptFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.chatsession-1.md)<br>

### **WithAntiprompt(String[])**

```csharp
public ChatSession<T> WithAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession&lt;T&gt;](./llama.chatsession-1.md)<br>
