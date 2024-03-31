# ChatCompletionChunkDelta

Namespace: LLama.Types

```csharp
public class ChatCompletionChunkDelta : System.IEquatable`1[[LLama.Types.ChatCompletionChunkDelta, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletionChunkDelta](./llama.types.chatcompletionchunkdelta.md)<br>
Implements [IEquatable&lt;ChatCompletionChunkDelta&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Role**

```csharp
public string Role { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Content**

```csharp
public string Content { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ChatCompletionChunkDelta(String, String)**

```csharp
public ChatCompletionChunkDelta(string Role, string Content)
```

#### Parameters

`Role` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **Equals(ChatCompletionChunkDelta)**

```csharp
public bool Equals(ChatCompletionChunkDelta other)
```

#### Parameters

`other` [ChatCompletionChunkDelta](./llama.types.chatcompletionchunkdelta.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletionChunkDelta <Clone>$()
```

#### Returns

[ChatCompletionChunkDelta](./llama.types.chatcompletionchunkdelta.md)<br>

### **Deconstruct(String&, String&)**

```csharp
public void Deconstruct(String& Role, String& Content)
```

#### Parameters

`Role` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Content` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
