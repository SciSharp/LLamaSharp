[`< Back`](./)

---

# SafeMtmdInputChunk

Namespace: LLama.Native

Managed wrapper around a single `mtmd_input_chunk`. Instances can either own the
 underlying native pointer (when created via [SafeMtmdInputChunk.Copy()](./llama.native.safemtmdinputchunk.md#copy)) or act as non-owning views
 produced by the tokenizer.

```csharp
public sealed class SafeMtmdInputChunk : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeMtmdInputChunk](./llama.native.safemtmdinputchunk.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **NativePtr**

Raw pointer to the native chunk structure.

```csharp
public IntPtr NativePtr { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **Type**

Chunk modality reported by the native helper.

```csharp
public SafeMtmdInputChunkType Type { get; }
```

#### Property Value

[SafeMtmdInputChunkType](./llama.native.safemtmdinputchunk.safemtmdinputchunktype.md)<br>

### **NTokens**

Number of tokens contained in this chunk.

```csharp
public ulong NTokens { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Id**

Identifier assigned by the tokenizer (if any).

```csharp
public string Id { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **NPos**

Number of positional slots consumed by this chunk.

```csharp
public long NPos { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

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

### **Wrap(IntPtr)**

Wrap an existing chunk pointer without taking ownership.

```csharp
public static SafeMtmdInputChunk Wrap(IntPtr ptr)
```

#### Parameters

`ptr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer returned by the native tokenizer.

#### Returns

[SafeMtmdInputChunk](./llama.native.safemtmdinputchunk.md)<br>
Managed wrapper, or `null` when the pointer is null.

### **Copy()**

Create an owning copy of the current chunk. The caller becomes responsible for disposal.

```csharp
public SafeMtmdInputChunk Copy()
```

#### Returns

[SafeMtmdInputChunk](./llama.native.safemtmdinputchunk.md)<br>
Owning managed wrapper, or `null` if the native copy failed.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
Thrown when the current wrapper has been disposed.

### **GetTextTokensSpan()**

Zero-copy view over the chunk's token buffer. The span remains valid only while the native chunk is alive.

```csharp
public ReadOnlySpan<int> GetTextTokensSpan()
```

#### Returns

[ReadOnlySpan&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Read-only span exposing the chunk's tokens.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
Thrown when the wrapper has been disposed.

### **ReleaseHandle()**

Releases the native chunk when ownership is held by this instance.

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
