# ChatMessageRecord

Namespace: LLama.Types

```csharp
public class ChatMessageRecord : System.IEquatable`1[[LLama.Types.ChatMessageRecord, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatMessageRecord](./llama.types.chatmessagerecord.md)<br>
Implements [IEquatable&lt;ChatMessageRecord&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Message**

```csharp
public ChatCompletionMessage Message { get; set; }
```

#### Property Value

[ChatCompletionMessage](./llama.types.chatcompletionmessage.md)<br>

### **Time**

```csharp
public DateTime Time { get; set; }
```

#### Property Value

[DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime)<br>

## Constructors

### **ChatMessageRecord(ChatCompletionMessage, DateTime)**

```csharp
public ChatMessageRecord(ChatCompletionMessage Message, DateTime Time)
```

#### Parameters

`Message` [ChatCompletionMessage](./llama.types.chatcompletionmessage.md)<br>

`Time` [DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime)<br>

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

### **Equals(ChatMessageRecord)**

```csharp
public bool Equals(ChatMessageRecord other)
```

#### Parameters

`other` [ChatMessageRecord](./llama.types.chatmessagerecord.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatMessageRecord <Clone>$()
```

#### Returns

[ChatMessageRecord](./llama.types.chatmessagerecord.md)<br>

### **Deconstruct(ChatCompletionMessage&, DateTime&)**

```csharp
public void Deconstruct(ChatCompletionMessage& Message, DateTime& Time)
```

#### Parameters

`Message` [ChatCompletionMessage&](./llama.types.chatcompletionmessage&.md)<br>

`Time` [DateTime&](https://docs.microsoft.com/en-us/dotnet/api/system.datetime&)<br>
