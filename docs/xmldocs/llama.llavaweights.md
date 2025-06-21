[`< Back`](./)

---

# LLavaWeights

Namespace: LLama

A set of llava model weights (mmproj), loaded into memory.

```csharp
public sealed class LLavaWeights : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLavaWeights](./llama.llavaweights.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **NativeHandle**

The native handle, which is used in the native APIs

```csharp
public SafeLlavaModelHandle NativeHandle { get; }
```

#### Property Value

[SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

**Remarks:**

Be careful how you use this!

## Methods

### **LoadFromFile(String)**

Load weights into memory

```csharp
public static LLavaWeights LoadFromFile(string mmProject)
```

#### Parameters

`mmProject` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
path to the "mmproj" model file

#### Returns

[LLavaWeights](./llama.llavaweights.md)<br>

### **LoadFromFileAsync(String, CancellationToken)**

Load weights into memory

```csharp
public static Task<LLavaWeights> LoadFromFileAsync(string mmProject, CancellationToken token)
```

#### Parameters

`mmProject` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
path to the "mmproj" model file

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;LLavaWeights&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **CreateImageEmbeddings(LLamaContext, Byte[])**

Create the Image Embeddings from the bytes of an image.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, Byte[] image)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image bytes. Supported formats:

- 
- 
- 
-

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

### **CreateImageEmbeddings(Byte[], Int32)**

Create the Image Embeddings.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(Byte[] image, int threads)
```

#### Parameters

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image in binary format (it supports jpeg format only)

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of threads to use

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
return the SafeHandle of these embeddings

### **CreateImageEmbeddings(LLamaContext, String)**

Create the Image Embeddings from the bytes of an image.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, string image)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>

`image` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the image file. Supported formats:

- 
- 
- 
-

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>

### **CreateImageEmbeddings(String, Int32)**

Create the Image Embeddings from the bytes of an image.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(string image, int threads)
```

#### Parameters

`image` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the image file. Supported formats:

- 
- 
- 
-

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>

### **EvalImageEmbed(LLamaContext, SafeLlavaImageEmbedHandle, Int32&)**

Eval the image embeddings

```csharp
public bool EvalImageEmbed(LLamaContext ctxLlama, SafeLlavaImageEmbedHandle imageEmbed, Int32& n_past)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>

`imageEmbed` [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

`n_past` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Dispose()**

```csharp
public void Dispose()
```

---

[`< Back`](./)
