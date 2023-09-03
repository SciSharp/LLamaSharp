# ChatCompletionMessage

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class ChatCompletionMessage : System.IEquatable`1[[LLama.OldVersion.ChatCompletionMessage, LLamaSharp, Version=0.5.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletionMessage](./llama.oldversion.chatcompletionmessage.md)<br>
Implements [IEquatable&lt;ChatCompletionMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Role**

```csharp
public ChatRole Role { get; set; }
```

#### Property Value

[ChatRole](./llama.oldversion.chatrole.md)<br>

### **Content**

```csharp
public string Content { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ChatCompletionMessage(ChatRole, String, String)**

```csharp
public ChatCompletionMessage(ChatRole Role, string Content, string Name)
```

#### Parameters

`Role` [ChatRole](./llama.oldversion.chatrole.md)<br>

`Content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **Equals(ChatCompletionMessage)**

```csharp
public bool Equals(ChatCompletionMessage other)
```

#### Parameters

`other` [ChatCompletionMessage](./llama.oldversion.chatcompletionmessage.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletionMessage <Clone>$()
```

#### Returns

[ChatCompletionMessage](./llama.oldversion.chatcompletionmessage.md)<br>

### **Deconstruct(ChatRole&, String&, String&)**

```csharp
public void Deconstruct(ChatRole& Role, String& Content, String& Name)
```

#### Parameters

`Role` [ChatRole&](./llama.oldversion.chatrole&.md)<br>

`Content` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Name` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
