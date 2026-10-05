[`< Back`](./)

---

# ContextOverflowException

Namespace: LLama.Exceptions

Thrown when the KV cache context is full and the model architecture 
 cannot mathematically support native memory shifting, or when the 
 ContextOverflowStrategy.ThrowException is used.

```csharp
public class ContextOverflowException : System.Exception, System.Runtime.Serialization.ISerializable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception) → [ContextOverflowException](./llama.exceptions.contextoverflowexception.md)<br>
Implements [ISerializable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iserializable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

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

### **ContextOverflowException()**

Initializes a new instance of the ContextOverflowException class with a default error message.

```csharp
public ContextOverflowException()
```

### **ContextOverflowException(String)**

Initializes a new instance of the ContextOverflowException class with a specified error message.

```csharp
public ContextOverflowException(string message)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The message that describes the error.

### **ContextOverflowException(String, Exception)**

Initializes a new instance of the ContextOverflowException class with a specified error message 
 and a reference to the inner exception that is the cause of this exception.

```csharp
public ContextOverflowException(string message, Exception innerException)
```

#### Parameters

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The message that describes the error.

`innerException` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br>
The exception that is the cause of the current exception.

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
