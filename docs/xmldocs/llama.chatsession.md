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

### **MODEL_STATE_FILENAME**

The filename for the serialized model state (KV cache, etc).

```csharp
public static string MODEL_STATE_FILENAME;
```

### **EXECUTOR_STATE_FILENAME**

The filename for the serialized executor state.

```csharp
public static string EXECUTOR_STATE_FILENAME;
```

### **HISTORY_STATE_FILENAME**

The filename for the serialized chat history.

```csharp
public static string HISTORY_STATE_FILENAME;
```

### **INPUT_TRANSFORM_FILENAME**

The filename for the serialized input transform pipeline.

```csharp
public static string INPUT_TRANSFORM_FILENAME;
```

### **OUTPUT_TRANSFORM_FILENAME**

The filename for the serialized output transform.

```csharp
public static string OUTPUT_TRANSFORM_FILENAME;
```

### **HISTORY_TRANSFORM_FILENAME**

The filename for the serialized history transform.

```csharp
public static string HISTORY_TRANSFORM_FILENAME;
```

## Properties

### **Executor**

The executor for this session.

```csharp
public ILLamaExecutor Executor { get; private set; }
```

#### Property Value

[ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>

### **History**

The chat history for this session.

```csharp
public ChatHistory History { get; private set; }
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

Create a new chat session.

```csharp
public ChatSession(ILLamaExecutor executor)
```

#### Parameters

`executor` [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
The executor for this session

### **ChatSession(ILLamaExecutor, ChatHistory)**

Create a new chat session with a custom history.

```csharp
public ChatSession(ILLamaExecutor executor, ChatHistory history)
```

#### Parameters

`executor` [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>

`history` [ChatHistory](./llama.common.chathistory.md)<br>

## Methods

### **InitializeSessionFromHistoryAsync(ILLamaExecutor, ChatHistory)**

Create a new chat session and preprocess history.

```csharp
public static Task<ChatSession> InitializeSessionFromHistoryAsync(ILLamaExecutor executor, ChatHistory history)
```

#### Parameters

`executor` [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
The executor for this session

`history` [ChatHistory](./llama.common.chathistory.md)<br>
History for this session

#### Returns

[Task&lt;ChatSession&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

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

Save a session from a directory.

```csharp
public void SaveSession(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **GetSessionState()**

Get the session state.

```csharp
public SessionState GetSessionState()
```

#### Returns

[SessionState](./llama.sessionstate.md)<br>
SessionState object representing session state in-memory

### **LoadSession(SessionState, Boolean)**

Load a session from a session state.

```csharp
public void LoadSession(SessionState state, bool loadTransforms)
```

#### Parameters

`state` [SessionState](./llama.sessionstate.md)<br>

`loadTransforms` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true loads transforms saved in the session state.

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **LoadSession(String, Boolean)**

Load a session from a directory.

```csharp
public void LoadSession(string path, bool loadTransforms)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`loadTransforms` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true loads transforms saved in the session state.

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **AddMessage(Message)**

Add a message to the chat history.

```csharp
public ChatSession AddMessage(Message message)
```

#### Parameters

`message` [Message](./llama.common.chathistory.message.md)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **AddSystemMessage(String)**

Add a system message to the chat history.

```csharp
public ChatSession AddSystemMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **AddAssistantMessage(String)**

Add an assistant message to the chat history.

```csharp
public ChatSession AddAssistantMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **AddUserMessage(String)**

Add a user message to the chat history.

```csharp
public ChatSession AddUserMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **RemoveLastMessage()**

Remove the last message from the chat history.

```csharp
public ChatSession RemoveLastMessage()
```

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **AddAndProcessMessage(Message)**

Compute KV cache for the message and add it to the chat history.

```csharp
public Task<ChatSession> AddAndProcessMessage(Message message)
```

#### Parameters

`message` [Message](./llama.common.chathistory.message.md)<br>

#### Returns

[Task&lt;ChatSession&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **AddAndProcessSystemMessage(String)**

Compute KV cache for the system message and add it to the chat history.

```csharp
public Task<ChatSession> AddAndProcessSystemMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Task&lt;ChatSession&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **AddAndProcessUserMessage(String)**

Compute KV cache for the user message and add it to the chat history.

```csharp
public Task<ChatSession> AddAndProcessUserMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Task&lt;ChatSession&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **AddAndProcessAssistantMessage(String)**

Compute KV cache for the assistant message and add it to the chat history.

```csharp
public Task<ChatSession> AddAndProcessAssistantMessage(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Task&lt;ChatSession&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **ReplaceUserMessage(Message, Message)**

Replace a user message with a new message and remove all messages after the new message.
 This is useful when the user wants to edit a message. And regenerate the response.

```csharp
public ChatSession ReplaceUserMessage(Message oldMessage, Message newMessage)
```

#### Parameters

`oldMessage` [Message](./llama.common.chathistory.message.md)<br>

`newMessage` [Message](./llama.common.chathistory.message.md)<br>

#### Returns

[ChatSession](./llama.chatsession.md)<br>

### **ChatAsync(Message, Boolean, IInferenceParams, CancellationToken)**

Chat with the model.

```csharp
public IAsyncEnumerable<string> ChatAsync(Message message, bool applyInputTransformPipeline, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`message` [Message](./llama.common.chathistory.message.md)<br>

`applyInputTransformPipeline` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **ChatAsync(Message, IInferenceParams, CancellationToken)**

Chat with the model.

```csharp
public IAsyncEnumerable<string> ChatAsync(Message message, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`message` [Message](./llama.common.chathistory.message.md)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

### **ChatAsync(ChatHistory, Boolean, IInferenceParams, CancellationToken)**

Chat with the model.

```csharp
public IAsyncEnumerable<string> ChatAsync(ChatHistory history, bool applyInputTransformPipeline, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`applyInputTransformPipeline` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **ChatAsync(ChatHistory, IInferenceParams, CancellationToken)**

Chat with the model.

```csharp
public IAsyncEnumerable<string> ChatAsync(ChatHistory history, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

### **RegenerateAssistantMessageAsync(InferenceParams, CancellationToken)**

Regenerate the last assistant message.

```csharp
public IAsyncEnumerable<string> RegenerateAssistantMessageAsync(InferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
