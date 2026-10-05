[`< Back`](./)

---

# State

Namespace: LLama

The state of this context, which can be reloaded later

```csharp
internal class State : LLama.Native.SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [State](./llama.llamacontext.state.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **Size**

Get the size in bytes of this state object

```csharp
public UIntPtr Size { get; }
```

#### Property Value

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **IsInvalid**

```csharp
public override bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **ReleaseHandle()**

```csharp
protected override bool ReleaseHandle()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SaveAsync(Stream)**

Write all the bytes of this state to the given stream

```csharp
public async Task SaveAsync(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>

### **Save(Stream)**

Write all the bytes of this state to the given stream

```csharp
public void Save(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

### **LoadAsync(Stream)**

Load a state from a stream

```csharp
public static async Task<State> LoadAsync(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

#### Returns

[Task&lt;State&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **Load(Stream)**

Load a state from a stream

```csharp
public static State Load(Stream stream)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream)<br>

#### Returns

[State](./llama.llamacontext.state.md)<br>

---

[`< Back`](./)
