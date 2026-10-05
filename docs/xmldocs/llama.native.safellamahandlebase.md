[`< Back`](./)

---

# SafeLLamaHandleBase

Namespace: LLama.Native

Base class for all llama handles to native resources

```csharp
public abstract class SafeLLamaHandleBase : System.Runtime.InteropServices.SafeHandle, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

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

### **ToString()**

```csharp
public override string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

---

[`< Back`](./)
