# ChatCompletionChunkChoice

Namespace: LLama.Types

```csharp
public class ChatCompletionChunkChoice : System.IEquatable`1[[LLama.Types.ChatCompletionChunkChoice, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletionChunkChoice](./llama.types.chatcompletionchunkchoice.md)<br>
Implements [IEquatable&lt;ChatCompletionChunkChoice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Index**

```csharp
public int Index { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Delta**

```csharp
public ChatCompletionChunkDelta Delta { get; set; }
```

#### Property Value

[ChatCompletionChunkDelta](./llama.types.chatcompletionchunkdelta.md)<br>

### **FinishReason**

```csharp
public string FinishReason { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ChatCompletionChunkChoice(Int32, ChatCompletionChunkDelta, String)**

```csharp
public ChatCompletionChunkChoice(int Index, ChatCompletionChunkDelta Delta, string FinishReason)
```

#### Parameters

`Index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Delta` [ChatCompletionChunkDelta](./llama.types.chatcompletionchunkdelta.md)<br>

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

### **Equals(ChatCompletionChunkChoice)**

```csharp
public bool Equals(ChatCompletionChunkChoice other)
```

#### Parameters

`other` [ChatCompletionChunkChoice](./llama.types.chatcompletionchunkchoice.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletionChunkChoice <Clone>$()
```

#### Returns

[ChatCompletionChunkChoice](./llama.types.chatcompletionchunkchoice.md)<br>

### **Deconstruct(Int32&, ChatCompletionChunkDelta&, String&)**

```csharp
public void Deconstruct(Int32& Index, ChatCompletionChunkDelta& Delta, String& FinishReason)
```

#### Parameters

`Index` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Delta` [ChatCompletionChunkDelta&](./llama.types.chatcompletionchunkdelta&.md)<br>

`FinishReason` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
