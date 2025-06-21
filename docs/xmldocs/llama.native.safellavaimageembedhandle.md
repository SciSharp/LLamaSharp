[`< Back`](./)

---

# SafeLlavaImageEmbedHandle

Namespace: LLama.Native

A Reference to a llava Image Embed handle

```csharp
public sealed class SafeLlavaImageEmbedHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **Model**

Get the model used to create this image embedding

```csharp
public SafeLlavaModelHandle Model { get; private set; }
```

#### Property Value

[SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

### **EmbeddingDimensions**

Get the number of dimensions in an embedding

```csharp
public int EmbeddingDimensions { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **PatchCount**

Get the number of "patches" in an image embedding

```csharp
public int PatchCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

## Constructors

### **SafeLlavaImageEmbedHandle()**

```csharp
public SafeLlavaImageEmbedHandle()
```

## Methods

### **CreateFromFileName(SafeLlavaModelHandle, LLamaContext, String)**

Create an image embed from an image file

```csharp
public static SafeLlavaImageEmbedHandle CreateFromFileName(SafeLlavaModelHandle clip, LLamaContext ctx, string image)
```

#### Parameters

`clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

`ctx` [LLamaContext](./llama.llamacontext.md)<br>

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

### **CreateFromFileName(SafeLlavaModelHandle, String, Int32)**

Create an image embed from an image file

```csharp
public static SafeLlavaImageEmbedHandle CreateFromFileName(SafeLlavaModelHandle clip, string image, int threads)
```

#### Parameters

`clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

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

### **CreateFromMemory(SafeLlavaModelHandle, LLamaContext, Byte[])**

Create an image embed from the bytes of an image.

```csharp
public static SafeLlavaImageEmbedHandle CreateFromMemory(SafeLlavaModelHandle clip, LLamaContext ctx, Byte[] image)
```

#### Parameters

`clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

`ctx` [LLamaContext](./llama.llamacontext.md)<br>

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image bytes. Supported formats:

- 
- 
- 
-

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

### **CreateFromMemory(SafeLlavaModelHandle, Byte[], Int32)**

Create an image embed from the bytes of an image.

```csharp
public static SafeLlavaImageEmbedHandle CreateFromMemory(SafeLlavaModelHandle clip, Byte[] image, int threads)
```

#### Parameters

`clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image bytes. Supported formats:

- 
- 
- 
-

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetEmbedding(Span&lt;Single&gt;, Int32)**

Copy the embeddings data to the destination span

```csharp
public void GetEmbedding(Span<float> dest, int index)
```

#### Parameters

`dest` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

---

[`< Back`](./)
