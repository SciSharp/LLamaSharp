# SessionState

Namespace: LLama

The state of a chat session in-memory.

```csharp
public class SessionState : System.IEquatable`1[[LLama.SessionState, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SessionState](./llama.sessionstate.md)<br>
Implements [IEquatable&lt;SessionState&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **ExecutorState**

Saved executor state for the session in JSON format.

```csharp
public ExecutorBaseState ExecutorState { get; set; }
```

#### Property Value

[ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **ContextState**

Saved context state (KV cache) for the session.

```csharp
public State ContextState { get; set; }
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

The the chat history messages for this session.

```csharp
public Message[] History { get; set; }
```

#### Property Value

[Message[]](./llama.common.chathistory.message.md)<br>

## Constructors

### **SessionState(State, ExecutorBaseState, ChatHistory, List&lt;ITextTransform&gt;, ITextStreamTransform, IHistoryTransform)**

Create a new session state.

```csharp
public SessionState(State contextState, ExecutorBaseState executorState, ChatHistory history, List<ITextTransform> inputTransformPipeline, ITextStreamTransform outputTransform, IHistoryTransform historyTransform)
```

#### Parameters

`contextState` [State](./llama.llamacontext.state.md)<br>

`executorState` [ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

`history` [ChatHistory](./llama.common.chathistory.md)<br>

`inputTransformPipeline` [List&lt;ITextTransform&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`outputTransform` [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

`historyTransform` [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>

## Methods

### **Save(String)**

Save the session state to folder.

```csharp
public void Save(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Load(String)**

Load the session state from folder.

```csharp
public static SessionState Load(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[SessionState](./llama.sessionstate.md)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Throws when session state is incorrect

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

### **Equals(SessionState)**

```csharp
public bool Equals(SessionState other)
```

#### Parameters

`other` [SessionState](./llama.sessionstate.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public SessionState <Clone>$()
```

#### Returns

[SessionState](./llama.sessionstate.md)<br>
