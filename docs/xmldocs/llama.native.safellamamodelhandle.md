[`< Back`](./)

---

# SafeLlamaModelHandle

Namespace: LLama.Native

A reference to a set of llama model weights

```csharp
public sealed class SafeLlamaModelHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **RopeType**

Get the rope (positional embedding) type for this model

```csharp
public LLamaRopeType RopeType { get; }
```

#### Property Value

[LLamaRopeType](./llama.native.llamaropetype.md)<br>

### **ContextSize**

The number of tokens in the context that this model was trained for

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **RopeFrequency**

Get the rope frequency this model was trained with

```csharp
public float RopeFrequency { get; }
```

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SizeInBytes**

Get the size of this model in bytes

```csharp
public ulong SizeInBytes { get; }
```

#### Property Value

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **ParameterCount**

Get the number of parameters in this model

```csharp
public ulong ParameterCount { get; }
```

#### Property Value

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **LayerCount**

Get the number of layers in this model

```csharp
public int LayerCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LayerCountMtp**

Get the number of MTP layers in this model

```csharp
public int LayerCountMtp { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **HeadCount**

Get the number of heads in this model

```csharp
public int HeadCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KVHeadCount**

Get the number of KV heads in this model

```csharp
public int KVHeadCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SWACount**

Get the number of SWA in this model

```csharp
public int SWACount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **HasEncoder**

Returns true if the model contains an encoder that requires llama_encode() call

```csharp
public bool HasEncoder { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **HasDecoder**

Returns true if the model contains a decoder that requires llama_decode() call

```csharp
public bool HasDecoder { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsRecurrent**

Returns true if the model is recurrent (like Mamba, RWKV, etc.)

```csharp
public bool IsRecurrent { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsDiffusion**

Returns true if the model is diffusion based (like LLaDA , Dream etc )

```csharp
public bool IsDiffusion { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Description**

Get a description of this model

```csharp
public string Description { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **FileType**

Get the file quantisation type of this model

```csharp
public LLamaFtype FileType { get; }
```

#### Property Value

[LLamaFtype](./llama.native.llamaftype.md)<br>

### **MetadataCount**

Get the number of metadata key/value pairs

```csharp
public int MetadataCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Vocab**

Get the vocabulary of this model

```csharp
public Vocabulary Vocab { get; }
```

#### Property Value

[Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

### **IsInvalid**

```csharp
public override bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **SafeLlamaModelHandle()**

```csharp
public SafeLlamaModelHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected override bool ReleaseHandle()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LoadFromFile(String, LLamaModelParams)**

Load a model from the given file path into memory

```csharp
public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaModelParams lparams)
```

#### Parameters

`modelPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`lparams` [LLamaModelParams](./llama.native.llamamodelparams.md)<br>

#### Returns

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **SaveToFile(String)**

Save this model to a file

```csharp
public void SaveToFile(string modelPath)
```

#### Parameters

`modelPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoadLoraFromFile(String)**

Load a LoRA adapter from file. The adapter will be associated with this model but will not be applied

```csharp
public LoraAdapter LoadLoraFromFile(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LoraAdapter](./llama.native.loraadapter.md)<br>

#### Exceptions

[InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>

### **TokenToSpan(LLamaToken, Span&lt;Byte&gt;, Int32, Boolean)**

Convert a single llama token into bytes

```csharp
public uint TokenToSpan(LLamaToken token, Span<byte> dest, int lstrip = 0, bool special = false)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

`lstrip` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
User can skip up to 'lstrip' leading spaces before copying (useful when encoding/decoding multiple tokens with 'add_space_prefix')

`special` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, special characters will be converted to text. If false they will be invisible.

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert a string of text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool addBos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`addBos` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`special` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **CreateContext(LLamaContextParams)**

Create a new context for this model

```csharp
public SafeLLamaContextHandle CreateContext(LLamaContextParams params)
```

#### Parameters

`params` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **MetadataValueByKey(String)**

Get the metadata value for the given key

```csharp
public Memory<byte>? MetadataValueByKey(string key)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The key to fetch

#### Returns

[Memory&lt;Byte&gt;?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The value, null if there is no such key

### **MetadataKeyByIndex(Int32)**

Get the metadata key for the given index

```csharp
public Memory<byte>? MetadataKeyByIndex(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index to get

#### Returns

[Memory&lt;Byte&gt;?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The key, null if there is no such key or if the buffer was too small

### **MetadataValueByIndex(Int32)**

Get the metadata value for the given index

```csharp
public Memory<byte>? MetadataValueByIndex(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index to get

#### Returns

[Memory&lt;Byte&gt;?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The value, null if there is no such value or if the buffer was too small

### **GetTemplate(String, Boolean)**

Get the default chat template. Returns nullptr if not available
 If name is NULL, returns the default chat template

```csharp
public string? GetTemplate(string? name = null, bool strict = true)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>
The name of the template, in case there are many or differently named. Set to 'null' for the default behaviour of finding an appropriate match.

`strict` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Setting this to true will cause the call to throw if no valid templates are found.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

---

[`< Back`](./)
