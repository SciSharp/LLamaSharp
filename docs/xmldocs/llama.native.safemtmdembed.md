[`< Back`](./)

---

# SafeMtmdEmbed

Namespace: LLama.Native

Managed wrapper around `mtmd_bitmap*` resources. Instances own the native pointer
 and ensure proper cleanup when disposed.

```csharp
public sealed class SafeMtmdEmbed : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **Nx**

Width of the bitmap in pixels (or number of samples for audio embeddings).

```csharp
public uint Nx { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Ny**

Height of the bitmap in pixels. For audio embeddings this is typically `1`.

```csharp
public uint Ny { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **IsAudio**

Indicates whether the embedding stores audio data instead of image pixels.

```csharp
public bool IsAudio { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ByteCount**

Get the byte count of the raw bitmap/audio data in this embed

```csharp
public ulong ByteCount { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Id**

Optional identifier assigned to this embedding.

```csharp
public string Id { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **FromRgbBytes(UInt32, UInt32, ReadOnlySpan&lt;Byte&gt;)**

Create an embedding from raw RGB bytes.

```csharp
public static SafeMtmdEmbed FromRgbBytes(uint nx, uint ny, ReadOnlySpan<byte> rgbData)
```

#### Parameters

`nx` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
Width of the bitmap in pixels.

`ny` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
Height of the bitmap in pixels.

`rgbData` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Packed RGB data (3 bytes per pixel).

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Managed wrapper when initialization succeeds; otherwise `null`.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
The RGB buffer is null.

### **FromAudioSamples(ReadOnlySpan&lt;Single&gt;)**

Create an embedding from PCM audio samples.

```csharp
public static SafeMtmdEmbed FromAudioSamples(ReadOnlySpan<float> samples)
```

#### Parameters

`samples` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Array of mono PCM samples in float format.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Managed wrapper when initialization succeeds; otherwise `null`.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
The audio buffer is null.

### **FromMediaFile(SafeMtmdModelHandle, String)**

Create an embedding by decoding a media file using libmtmd helpers.

```csharp
public static SafeMtmdEmbed FromMediaFile(SafeMtmdModelHandle mtmdContext, string path)
```

#### Parameters

`mtmdContext` [SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>
Model context that provides the decoder configuration.

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the media file on disk.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Managed wrapper when decoding succeeds; otherwise `null`.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
The context is null.

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
The path is null or whitespace.

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>
The supplied file does not exist.

### **FromMediaBuffer(SafeMtmdModelHandle, ReadOnlySpan&lt;Byte&gt;)**

Create an embedding from an in-memory media buffer (image/audio/video).

```csharp
public static SafeMtmdEmbed FromMediaBuffer(SafeMtmdModelHandle mtmdContext, ReadOnlySpan<byte> data)
```

#### Parameters

`mtmdContext` [SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>
Model context that provides the decoder configuration.

`data` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Binary buffer containing the encoded media.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Managed wrapper when decoding succeeds; otherwise `null`.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
The context is null.

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
The buffer is empty.

### **GetData()**

Provides safe zero-copy access to the underlying bitmap bytes.

```csharp
public IEmbedData GetData()
```

#### Returns

[IEmbedData](./llama.native.safemtmdembed.iembeddata.md)<br>
The data access is guaranteed to remain valid until this object is disposed.

### **ReleaseHandle()**

Release the underlying native bitmap.

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
