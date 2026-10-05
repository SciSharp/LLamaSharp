[`< Back`](./)

---

# UnknownDeviceException

Namespace: LLama.Exceptions

Thrown when a device name in [IModelParams.Devices](./llama.abstractions.imodelparams.md#devices) does not match any available ggml backend device

```csharp
public class UnknownDeviceException : System.Exception, System.Runtime.Serialization.ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [UnknownDeviceException](./llama.exceptions.unknowndeviceexception.md)<br>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **RequestedDevice**

The device name which was requested but could not be found

```csharp
public string RequestedDevice { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **AvailableDevices**

Names of all devices available on this machine, as returned by [NativeApi.ggml_backend_dev_name(IntPtr)](./llama.native.nativeapi.md#ggml_backend_dev_nameintptr)

```csharp
public IReadOnlyList<string> AvailableDevices { get; }
```

#### Property Value

[IReadOnlyList&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

### **TargetSite**

```csharp
public MethodBase? TargetSite { get; }
```

#### Property Value

[MethodBase](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase)<br>

### **Message**

```csharp
public virtual string Message { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Data**

```csharp
public virtual IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://learn.microsoft.com/en-us/dotnet/api/system.collections.idictionary)<br>

### **InnerException**

```csharp
public Exception? InnerException { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **HelpLink**

```csharp
public virtual string? HelpLink { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Source**

```csharp
public virtual string? Source { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **StackTrace**

```csharp
public virtual string? StackTrace { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **UnknownDeviceException(String, IReadOnlyList&lt;String&gt;)**

Create a new UnknownDeviceException

```csharp
public UnknownDeviceException(string requestedDevice, IReadOnlyList<string> availableDevices)
```

#### Parameters

`requestedDevice` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The device name which was requested but could not be found

`availableDevices` [IReadOnlyList&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>
Names of all devices available on this machine

## Events

### **SerializeObjectState**

#### Caution

BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.

---

```csharp
protected event EventHandler<SafeSerializationEventArgs>? SerializeObjectState;
```

---

[`< Back`](./)
