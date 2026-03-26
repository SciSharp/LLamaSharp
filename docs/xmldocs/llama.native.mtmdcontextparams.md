[`< Back`](./)

---

# MtmdContextParams

Namespace: LLama.Native

Managed representation of the native `mtmd_context_params` structure used to configure multimodal helpers.

```csharp
public class MtmdContextParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **UseGpu**

Whether GPU acceleration should be requested when available.

```csharp
public bool UseGpu { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **PrintTimings**

Whether timing information should be emitted by the native helper.

```csharp
public bool PrintTimings { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NThreads**

Number of worker threads to dedicate to preprocessing and tokenization.

```csharp
public int NThreads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ImageMarker**

Marker token inserted into the text stream to reference an image embedding (deprecated by mtmd).

```csharp
public string ImageMarker { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **MediaMarker**

Marker token inserted into the text stream to reference a generic media embedding.

```csharp
public string MediaMarker { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **FlashAttentionType**

Flash attention policy forwarded to mtmd encoders.

```csharp
public LLamaFlashAttentionType FlashAttentionType { get; set; }
```

#### Property Value

[LLamaFlashAttentionType](./llama.native.llamaflashattentiontype.md)<br>

### **Warmup**

Whether to run a warmup encode pass after initialization.

```csharp
public bool Warmup { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ImageMinTokens**

Minimum number of image tokens for dynamic resolution (use -1 to read metadata).

```csharp
public int ImageMinTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ImageMaxTokens**

Maximum number of image tokens for dynamic resolution (use -1 to read metadata).

```csharp
public int ImageMaxTokens { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **MtmdContextParams()**

```csharp
public MtmdContextParams()
```

## Methods

### **Default()**

Create a managed copy of the native defaults returned by .

```csharp
public static MtmdContextParams Default()
```

#### Returns

[MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>

---

[`< Back`](./)
