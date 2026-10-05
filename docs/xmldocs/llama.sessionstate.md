[`< Back`](./)

---

# SessionState

Namespace: LLama

The state of a chat session in-memory.

```csharp
public record class SessionState : System.IEquatable`1[[LLama.SessionState, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SessionState](./llama.sessionstate.md)<br>
Implements [IEquatable&lt;SessionState&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **ExecutorState**

Saved executor state for the session in JSON format.

```csharp
public ExecutorBaseState? ExecutorState { get; set; }
```

#### Property Value

[ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **ContextState**

Saved context state (KV cache) for the session.

```csharp
public State? ContextState { get; set; }
```

#### Property Value

[State](./llama.llamacontext.state.md)<br>

### **InputTransformPipeline**

The input transform pipeline used in this session.

```csharp
public ITextTransform[] InputTransformPipeline { get; set; }
```

#### Property Value

[ITextTransform[]](./llama.abstractions.itexttransform.md)<br>

### **OutputTransform**

The output transform used in this session.

```csharp
public ITextStreamTransform OutputTransform { get; set; }
```

#### Property Value

[ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

### **HistoryTransform**

The history transform used in this session.

```csharp
public IHistoryTransform HistoryTransform { get; set; }
```

#### Property Value

[IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

### **History**

The chat history messages for this session.

```csharp
public Message[] History { get; set; }
```

#### Property Value

[Message[]](./llama.common.chathistory.message.md)<br>

## Constructors

### **SessionState(State, ExecutorBaseState, ChatHistory, List&lt;ITextTransform&gt;, ITextStreamTransform, IHistoryTransform)**

Create a new session state.

```csharp
public SessionState(State? contextState, ExecutorBaseState executorState, ChatHistory history, List<ITextTransform> inputTransformPipeline, ITextStreamTransform outputTransform, IHistoryTransform historyTransform)
```

#### Parameters

`contextState` [State](./llama.llamacontext.state.md)?<br>

`executorState` [ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`inputTransformPipeline` [List&lt;ITextTransform&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`outputTransform` [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

`historyTransform` [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

### **SessionState(SessionState)**

```csharp
protected SessionState(SessionState original)
```

#### Parameters

`original` [SessionState](./llama.sessionstate.md)<br>

## Methods

### **Save(String)**

Save the session state to folder.

```csharp
public void Save(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Load(String)**

Load the session state from folder.

```csharp
public static SessionState Load(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[SessionState](./llama.sessionstate.md)<br>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Throws when session state is incorrect

---

[`< Back`](./)
