# SafeLlavaImageEmbedHandle

Namespace: LLama.Native

A Reference to a llava Image Embed handle

```csharp
public sealed class SafeLlavaImageEmbedHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

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

### **CreateFromFileName(SafeLlavaModelHandle, LLamaContext, String)**

Create an image embed from an image file

```csharp
public static SafeLlavaImageEmbedHandle CreateFromFileName(SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, string image)
```

#### Parameters

`ctxLlava` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>

`image` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the image file. Supported formats:
 JPGPNGBMPTGA

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>

### **CreateFromMemory(SafeLlavaModelHandle, LLamaContext, Byte[])**

Create an image embed from the bytes of an image.

```csharp
public static SafeLlavaImageEmbedHandle CreateFromMemory(SafeLlavaModelHandle ctxLlava, LLamaContext ctxLlama, Byte[] image)
```

#### Parameters

`ctxLlava` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image bytes. Supported formats:
 JPGPNGBMPTGA

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
