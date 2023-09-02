# IChatModel

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public interface IChatModel
```

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Chat(String, String, String)**

```csharp
IEnumerable<string> Chat(string text, string prompt, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InitChatPrompt(String, String)**

Init a prompt for chat and automatically produce the next prompt during the chat.

```csharp
void InitChatPrompt(string prompt, string encoding)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InitChatAntiprompt(String[])**

```csharp
void InitChatAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
