[`< Back`](./)

---

# Message

Namespace: LLama.Common

Chat message representation

```csharp
internal class Message
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Message](./llama.common.chathistory.message.md)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **AuthorRole**

Role of the message author, e.g. user/assistant/system

```csharp
public AuthorRole AuthorRole { get; set; }
```

#### Property Value

[AuthorRole](./llama.common.authorrole.md)<br>

### **Content**

Message content

```csharp
public string Content { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **Message(AuthorRole, String)**

Create a new instance

```csharp
public Message(AuthorRole authorRole, string content)
```

#### Parameters

`authorRole` [AuthorRole](./llama.common.authorrole.md)<br>
Role of message author

`content` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Message content

---

[`< Back`](./)
