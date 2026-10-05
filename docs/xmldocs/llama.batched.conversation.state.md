[`< Back`](./)

---

# State

Namespace: LLama.Batched

In memory saved state of a [Conversation](./llama.batched.conversation.md)

```csharp
internal abstract class State : System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [State](./llama.batched.conversation.state.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **IsDisposed**

Indicates if this state has been disposed

```csharp
public bool IsDisposed { get; protected set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Size**

Get the size in bytes of this state object

```csharp
public abstract ulong Size { get; }
```

#### Property Value

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

## Methods

### **Dispose()**

```csharp
public abstract void Dispose()
```

---

[`< Back`](./)
