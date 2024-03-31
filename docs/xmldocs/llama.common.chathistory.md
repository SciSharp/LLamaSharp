# ChatHistory

Namespace: LLama.Common

The chat history class

```csharp
public class ChatHistory
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatHistory](./llama.common.chathistory.md)

## Properties

### **Messages**

List of messages in the chat

```csharp
public List<Message> Messages { get; set; }
```

#### Property Value

[List&lt;Message&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

## Constructors

### **ChatHistory()**

Create a new instance of the chat content class

```csharp
public ChatHistory()
```

### **ChatHistory(Message[])**

Create a new instance of the chat history from array of messages

```csharp
public ChatHistory(Message[] messageHistory)
```

#### Parameters

`messageHistory` [Message[]](./llama.common.chathistory.message.md)<br>

## Methods

### **AddMessage(AuthorRole, String)**

Add a message to the chat history

```csharp
public void AddMessage(AuthorRole authorRole, string content)
```

#### Parameters

`authorRole` [AuthorRole](./llama.common.authorrole.md)<br>
Role of the message author

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Message content

### **ToJson()**

Serialize the chat history to JSON

```csharp
public string ToJson()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **FromJson(String)**

Deserialize a chat history from JSON

```csharp
public static ChatHistory FromJson(string json)
```

#### Parameters

`json` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatHistory](./llama.common.chathistory.md)<br>
