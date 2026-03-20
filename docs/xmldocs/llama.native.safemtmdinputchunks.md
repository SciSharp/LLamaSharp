[`< Back`](./)

---

# SafeMtmdInputChunks

Namespace: LLama.Native

Managed lifetime wrapper around a native `mtmd_input_chunks` collection returned by the tokenizer.

```csharp
public sealed class SafeMtmdInputChunks : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **Size**

Number of chunks currently held by the native collection.

```csharp
public ulong Size { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **IsInvalid**

```csharp
public bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **ReleaseHandle()**

Releases the native chunk collection.

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **CountTokens()**

Get the number of tokens contained in this chunk collection.

```csharp
public ulong CountTokens()
```

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Total token count.

### **CountPositions()**

Get the number of positions contained in this chunk collection.

```csharp
public long CountPositions()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>
Total number of positional slots consumed.

### **GetChunkPtr(UInt64)**

Get a raw pointer to a chunk. The returned [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr) is the `mtmd_input_chunk*`.
 Use [SafeMtmdInputChunk.Wrap(IntPtr)](./llama.native.safemtmdinputchunk.md#wrapintptr) to create a managed wrapper if desired.

```csharp
public IntPtr GetChunkPtr(ulong index)
```

#### Parameters

`index` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Zero-based index of the chunk to retrieve.

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the requested chunk.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The collection has already been disposed.

[IndexOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.indexoutofrangeexception)<br>
The requested index is outside of the valid range.

### **Enumerate()**

Enumerate the contained chunks as non-owning wrappers. Callers should dispose the returned chunk
 if they create a copy.

```csharp
public IEnumerable<SafeMtmdInputChunk> Enumerate()
```

#### Returns

[IEnumerable&lt;SafeMtmdInputChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
Enumeration of chunk wrappers backed by the native collection.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The collection has already been disposed.

---

[`< Back`](./)
