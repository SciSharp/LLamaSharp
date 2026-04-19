[`< Back`](./)

---

# LLamaTemplate

Namespace: LLama

Converts a sequence of messages into text according to a model template

```csharp
public sealed class LLamaTemplate
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaTemplate](./llama.llamatemplate.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [DefaultMemberAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.defaultmemberattribute)

## Fields

### **Encoding**

The encoding algorithm to use

```csharp
public static Encoding Encoding;
```

## Properties

### **Count**

Number of messages added to this template

```csharp
public int Count { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**

```csharp
public TextMessage Item { get; }
```

#### Property Value

[TextMessage](./llama.llamatemplate.textmessage.md)<br>

### **AddAssistant**

Whether to end the prompt with the token(s) that indicate the start of an assistant message.

```csharp
public bool AddAssistant { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **LLamaTemplate(SafeLlamaModelHandle, String, Boolean)**

Construct a new template, using the default model template

```csharp
public LLamaTemplate(SafeLlamaModelHandle model, string name, bool strict)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
The native handle of the loaded model.

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The name of the template, in case there are many or differently named. Set to 'null' for the default behaviour of finding an appropriate match.

`strict` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Setting this to true will cause the call to throw if no valid templates are found.

### **LLamaTemplate(LLamaWeights, Boolean)**

Construct a new template, using the default model template

```csharp
public LLamaTemplate(LLamaWeights weights, bool strict)
```

#### Parameters

`weights` [LLamaWeights](./llama.llamaweights.md)<br>
The handle of the loaded model's weights.

`strict` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Setting this to true will cause the call to throw if no valid templates are found.

### **LLamaTemplate(String)**

Construct a new template, using a custom template.

```csharp
public LLamaTemplate(string customTemplate)
```

#### Parameters

`customTemplate` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

**Remarks:**

Only support a pre-defined list of templates. See more: https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template

## Methods

### **Add(String, String)**

Add a new message to the end of this template

```csharp
public LLamaTemplate Add(string role, string content)
```

#### Parameters

`role` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaTemplate](./llama.llamatemplate.md)<br>
This template, for chaining calls.

### **Add(TextMessage)**

Add a new message to the end of this template

```csharp
public LLamaTemplate Add(TextMessage message)
```

#### Parameters

`message` [TextMessage](./llama.llamatemplate.textmessage.md)<br>

#### Returns

[LLamaTemplate](./llama.llamatemplate.md)<br>
This template, for chaining calls.

### **RemoveAt(Int32)**

Remove a message at the given index

```csharp
public LLamaTemplate RemoveAt(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaTemplate](./llama.llamatemplate.md)<br>
This template, for chaining calls.

### **Clear()**

Remove all messages from the template and resets internal state to accept/generate new messages

```csharp
public void Clear()
```

### **Apply()**

Apply the template to the messages and return a span containing the results

```csharp
public ReadOnlySpan<byte> Apply()
```

#### Returns

[ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A span over the buffer that holds the applied template

---

[`< Back`](./)
