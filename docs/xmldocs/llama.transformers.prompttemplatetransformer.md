[`< Back`](./)

---

# PromptTemplateTransformer

Namespace: LLama.Transformers

A prompt formatter that will use llama.cpp's template formatter
 If your model is not supported, you will need to define your own formatter according the cchat prompt specification for your model

```csharp
public class PromptTemplateTransformer : LLama.Abstractions.IHistoryTransform
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PromptTemplateTransformer](./llama.transformers.prompttemplatetransformer.md)<br>
Implements [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Constructors

### **PromptTemplateTransformer(LLamaWeights, Boolean)**

A prompt formatter that will use llama.cpp's template formatter
 If your model is not supported, you will need to define your own formatter according the cchat prompt specification for your model

```csharp
public PromptTemplateTransformer(LLamaWeights model, bool withAssistant = true)
```

#### Parameters

`model` [LLamaWeights](./llama.llamaweights.md)<br>

`withAssistant` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **HistoryToText(ChatHistory)**

Convert a ChatHistory instance to plain text.

```csharp
public string HistoryToText(ChatHistory history)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>
The ChatHistory instance

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TextToHistory(AuthorRole, String)**

Converts plain text to a ChatHistory instance.

```csharp
public ChatHistory TextToHistory(AuthorRole role, string text)
```

#### Parameters

`role` [AuthorRole](./llama.common.authorrole.md)<br>
The role for the author.

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The chat history as plain text.

#### Returns

[ChatHistory](./llama.common.chathistory.md)<br>
The updated history.

### **Clone()**

Copy the transform.

```csharp
public IHistoryTransform Clone()
```

#### Returns

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

### **ToModelPrompt(LLamaTemplate)**

Apply the template to the messages and return the resulting prompt as a string

```csharp
public static string ToModelPrompt(LLamaTemplate template)
```

#### Parameters

`template` [LLamaTemplate](./llama.llamatemplate.md)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The formatted template string as defined by the model

---

[`< Back`](./)
