# IChatModel

Namespace: LLama

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

### **Chat(String, String)**

```csharp
IEnumerable<string> Chat(string text, string prompt)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InitChatPrompt(String)**

```csharp
void InitChatPrompt(string prompt)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InitChatAntiprompt(String[])**

```csharp
void InitChatAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
