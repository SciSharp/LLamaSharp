[`< Back`](./)

---

# IEmbedData

Namespace: LLama.Native

Accessor for the raw data of a [SafeMtmdEmbed](./llama.native.safemtmdembed.md)

```csharp
internal interface IEmbedData : System.IDisposable
```

Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Data**

Get the raw data. Access to this span is only guaranteed to be valid until this accessor is disposed.

```csharp
ReadOnlySpan<byte> Data { get; }
```

#### Property Value

[ReadOnlySpan&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Exceptions

[ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
Thrown if this accessor has been disposed

### **IsValid**

Indicates if this accessor is still valid (i.e. not disposed)

```csharp
bool IsValid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
