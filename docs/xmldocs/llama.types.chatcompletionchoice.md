# ChatCompletionChoice

Namespace: LLama.Types

```csharp
public class ChatCompletionChoice : System.IEquatable`1[[LLama.Types.ChatCompletionChoice, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletionChoice](./llama.types.chatcompletionchoice.md)<br>
Implements [IEquatable&lt;ChatCompletionChoice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Index**

```csharp
public int Index { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Message**

```csharp
public ChatCompletionMessage Message { get; set; }
```

#### Property Value

[ChatCompletionMessage](./llama.types.chatcompletionmessage.md)<br>

### **FinishReason**

```csharp
public string FinishReason { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ChatCompletionChoice(Int32, ChatCompletionMessage, String)**

```csharp
public ChatCompletionChoice(int Index, ChatCompletionMessage Message, string FinishReason)
```

#### Parameters

`Index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Message` [ChatCompletionMessage](./llama.types.chatcompletionmessage.md)<br>

`FinishReason` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **Equals(ChatCompletionChoice)**

```csharp
public bool Equals(ChatCompletionChoice other)
```

#### Parameters

`other` [ChatCompletionChoice](./llama.types.chatcompletionchoice.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletionChoice <Clone>$()
```

#### Returns

[ChatCompletionChoice](./llama.types.chatcompletionchoice.md)<br>

### **Deconstruct(Int32&, ChatCompletionMessage&, String&)**

```csharp
public void Deconstruct(Int32& Index, ChatCompletionMessage& Message, String& FinishReason)
```

#### Parameters

`Index` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Message` [ChatCompletionMessage&](./llama.types.chatcompletionmessage&.md)<br>

`FinishReason` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
