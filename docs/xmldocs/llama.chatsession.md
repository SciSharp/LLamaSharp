# ChatSession

Namespace: LLama

The main chat session class.

```csharp
public class ChatSession
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatSession](./llama.chatsession.md)

## Fields

### **OutputTransform**

The output transform used in this session.

```csharp
public ITextStreamTransform OutputTransform;
```

## Properties

### **Executor**

The executor for this session.

```csharp
public ILLamaExecutor Executor { get; }
```

#### Property Value

[ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>

### **History**

The chat history for this session.

```csharp
public ChatHistory History { get; }
```

#### Property Value

[ChatHistory](./llama.common.chathistory.md)<br>

### **HistoryTransform**

The history transform used in this session.

```csharp
public IHistoryTransform HistoryTransform { get; set; }
```

#### Property Value

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

### **InputTransformPipeline**

The input transform pipeline used in this session.

```csharp
public List<ITextTransform> InputTransformPipeline { get; set; }
```

#### Property Value

[List&lt;ITextTransform&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

## Constructors

### **ChatSession(ILLamaExecutor)**



```csharp
public ChatSession(ILLamaExecutor executor)
```

#### Parameters

`executor` [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
The executor for this session

## Methods

### **WithHistoryTransform(IHistoryTransform)**

Use a custom history transform.

```csharp
public ChatSession WithHistoryTransform(IHistoryTransform transform)
```

#### Parameters

`transform` [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **AddInputTransform(ITextTransform)**

Add a text transform to the input transform pipeline.

```csharp
public ChatSession AddInputTransform(ITextTransform transform)
```

#### Parameters

`transform` [ITextTransform](./llama.abstractions.itexttransform.md)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **WithOutputTransform(ITextStreamTransform)**

Use a custom output transform.

```csharp
public ChatSession WithOutputTransform(ITextStreamTransform transform)
```

#### Parameters

`transform` [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **SaveSession(String)**



```csharp
public void SaveSession(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The directory name to save the session. If the directory does not exist, a new directory will be created.

### **LoadSession(String)**



```csharp
public void LoadSession(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The directory name to load the session.

### **Chat(ChatHistory, IInferenceParams, CancellationToken)**

Get the response from the LLama model with chat histories.

```csharp
public IEnumerable<string> Chat(ChatHistory history, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Chat(String, IInferenceParams, CancellationToken)**

Get the response from the LLama model. Note that prompt could not only be the preset words, 
 but also the question you want to ask.

```csharp
public IEnumerable<string> Chat(string prompt, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **ChatAsync(ChatHistory, IInferenceParams, CancellationToken)**

Get the response from the LLama model with chat histories.

```csharp
public IAsyncEnumerable<string> ChatAsync(ChatHistory history, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

### **ChatAsync(String, IInferenceParams, CancellationToken)**

Get the response from the LLama model with chat histories asynchronously.

```csharp
public IAsyncEnumerable<string> ChatAsync(string prompt, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
