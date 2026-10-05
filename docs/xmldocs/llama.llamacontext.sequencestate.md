[`< Back`](./)

---

# SequenceState

Namespace: LLama

The state of a single sequence, which can be reloaded later

```csharp
internal class SequenceState : LLama.Native.SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SequenceState](./llama.llamacontext.sequencestate.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

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

### **CopyTo(Byte*, UInt64, UInt64)**

Copy bytes to a destination pointer.

```csharp
public ulong CopyTo(Byte* dst, ulong length, ulong offset = 0)
```

#### Parameters

`dst` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`length` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Length of the destination buffer

`offset` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Offset from start of src to start copying from

#### Returns

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of bytes written to destination

---

[`< Back`](./)
