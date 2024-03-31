# ChatCompletionChunk

Namespace: LLama.Types

```csharp
public class ChatCompletionChunk : System.IEquatable`1[[LLama.Types.ChatCompletionChunk, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletionChunk](./llama.types.chatcompletionchunk.md)<br>
Implements [IEquatable&lt;ChatCompletionChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Id**

```csharp
public string Id { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Model**

```csharp
public string Model { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Object**

```csharp
public string Object { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Created**

```csharp
public int Created { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Choices**

```csharp
public ChatCompletionChunkChoice[] Choices { get; set; }
```

#### Property Value

[ChatCompletionChunkChoice[]](./llama.types.chatcompletionchunkchoice.md)<br>

## Constructors

### **ChatCompletionChunk(String, String, String, Int32, ChatCompletionChunkChoice[])**

```csharp
public ChatCompletionChunk(string Id, string Model, string Object, int Created, ChatCompletionChunkChoice[] Choices)
```

#### Parameters

`Id` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Created` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Choices` [ChatCompletionChunkChoice[]](./llama.types.chatcompletionchunkchoice.md)<br>

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

### **Equals(ChatCompletionChunk)**

```csharp
public bool Equals(ChatCompletionChunk other)
```

#### Parameters

`other` [ChatCompletionChunk](./llama.types.chatcompletionchunk.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletionChunk <Clone>$()
```

#### Returns

[ChatCompletionChunk](./llama.types.chatcompletionchunk.md)<br>

### **Deconstruct(String&, String&, String&, Int32&, ChatCompletionChunkChoice[]&)**

```csharp
public void Deconstruct(String& Id, String& Model, String& Object, Int32& Created, ChatCompletionChunkChoice[]& Choices)
```

#### Parameters

`Id` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Model` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Created` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Choices` [ChatCompletionChunkChoice[]&](./llama.types.chatcompletionchunkchoice&.md)<br>
