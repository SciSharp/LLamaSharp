# ChatCompletion

Namespace: LLama.Types

```csharp
public class ChatCompletion : System.IEquatable`1[[LLama.Types.ChatCompletion, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatCompletion](./llama.types.chatcompletion.md)<br>
Implements [IEquatable&lt;ChatCompletion&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Id**

```csharp
public string Id { get; set; }
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

### **Model**

```csharp
public string Model { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Choices**

```csharp
public ChatCompletionChoice[] Choices { get; set; }
```

#### Property Value

[ChatCompletionChoice[]](./llama.types.chatcompletionchoice.md)<br>

### **Usage**

```csharp
public CompletionUsage Usage { get; set; }
```

#### Property Value

[CompletionUsage](./llama.types.completionusage.md)<br>

## Constructors

### **ChatCompletion(String, String, Int32, String, ChatCompletionChoice[], CompletionUsage)**

```csharp
public ChatCompletion(string Id, string Object, int Created, string Model, ChatCompletionChoice[] Choices, CompletionUsage Usage)
```

#### Parameters

`Id` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Created` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Choices` [ChatCompletionChoice[]](./llama.types.chatcompletionchoice.md)<br>

`Usage` [CompletionUsage](./llama.types.completionusage.md)<br>

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

### **Equals(ChatCompletion)**

```csharp
public bool Equals(ChatCompletion other)
```

#### Parameters

`other` [ChatCompletion](./llama.types.chatcompletion.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ChatCompletion <Clone>$()
```

#### Returns

[ChatCompletion](./llama.types.chatcompletion.md)<br>

### **Deconstruct(String&, String&, Int32&, String&, ChatCompletionChoice[]&, CompletionUsage&)**

```csharp
public void Deconstruct(String& Id, String& Object, Int32& Created, String& Model, ChatCompletionChoice[]& Choices, CompletionUsage& Usage)
```

#### Parameters

`Id` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Created` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Model` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Choices` [ChatCompletionChoice[]&](./llama.types.chatcompletionchoice&.md)<br>

`Usage` [CompletionUsage&](./llama.types.completionusage&.md)<br>
