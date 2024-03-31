# SafeLlamaModelHandle

Namespace: LLama.Native

A reference to a set of llama model weights

```csharp
public sealed class SafeLlamaModelHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **VocabCount**

Total number of tokens in vocabulary of this model

```csharp
public int VocabCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ContextSize**

Total number of tokens in the context

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **RopeFrequency**

Get the rope frequency this model was trained with

```csharp
public float RopeFrequency { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SizeInBytes**

Get the size of this model in bytes

```csharp
public ulong SizeInBytes { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **ParameterCount**

Get the number of parameters in this model

```csharp
public ulong ParameterCount { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Description**

Get a description of this model

```csharp
public string Description { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **MetadataCount**

Get the number of metadata key/value pairs

```csharp
public int MetadataCount { get; }
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

### **SafeLlamaModelHandle()**

```csharp
public SafeLlamaModelHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LoadFromFile(String, LLamaModelParams)**

Load a model from the given file path into memory

```csharp
public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaModelParams lparams)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`lparams` [LLamaModelParams](./llama.native.llamamodelparams.md)<br>

#### Returns

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **llama_model_apply_lora_from_file(SafeLlamaModelHandle, String, Single, String, Int32)**

Apply a LoRA adapter to a loaded model
 path_base_model is the path to a higher quality model to use as a base for
 the layers modified by the adapter. Can be NULL to use the current loaded model.
 The model needs to be reloaded before applying a new adapter, otherwise the adapter
 will be applied on top of the previous one

```csharp
public static int llama_model_apply_lora_from_file(SafeLlamaModelHandle model_ptr, string path_lora, float scale, string path_base_model, int n_threads)
```

#### Parameters

`model_ptr` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`path_lora` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`path_base_model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns 0 on success

### **llama_model_meta_val_str(SafeLlamaModelHandle, Byte*, Byte*, Int64)**

Get metadata value as a string by key name

```csharp
public static int llama_model_meta_val_str(SafeLlamaModelHandle model, Byte* key, Byte* buf, long buf_size)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`key` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`buf` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`buf_size` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length of the string on success, or -1 on failure

### **ApplyLoraFromFile(String, Single, String, Nullable&lt;Int32&gt;)**

Apply a LoRA adapter to a loaded model

```csharp
public void ApplyLoraFromFile(string lora, float scale, string modelBase, Nullable<int> threads)
```

#### Parameters

`lora` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`modelBase` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A path to a higher quality model to use as a base for the layers modified by the
 adapter. Can be NULL to use the current loaded model.

`threads` [Nullable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **TokenToSpan(LLamaToken, Span&lt;Byte&gt;)**

Convert a single llama token into bytes

```csharp
public uint TokenToSpan(LLamaToken token, Span<byte> dest)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **TokensToSpan(IReadOnlyList&lt;LLamaToken&gt;, Span&lt;Char&gt;, Encoding)**

#### Caution

Use a StreamingTokenDecoder instead

---

Convert a sequence of tokens into characters.

```csharp
internal Span<char> TokensToSpan(IReadOnlyList<LLamaToken> tokens, Span<char> dest, Encoding encoding)
```

#### Parameters

`tokens` [IReadOnlyList&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

`dest` [Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[Span&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The section of the span which has valid data in it.
 If there was insufficient space in the output span this will be
 filled with as many characters as possible, starting from the _last_ token.

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert a string of text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

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

### **MetadataKeyByIndex(Int32)**

Get the metadata key for the given index

```csharp
public Nullable<Memory<byte>> MetadataKeyByIndex(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index to get

#### Returns

[Nullable&lt;Memory&lt;Byte&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The key, null if there is no such key or if the buffer was too small

### **MetadataValueByIndex(Int32)**

Get the metadata value for the given index

```csharp
public Nullable<Memory<byte>> MetadataValueByIndex(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index to get

#### Returns

[Nullable&lt;Memory&lt;Byte&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>
The value, null if there is no such value or if the buffer was too small

### **ReadMetadata()**

```csharp
internal IReadOnlyDictionary<string, string> ReadMetadata()
```

#### Returns

[IReadOnlyDictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>

### **&lt;llama_model_meta_key_by_index&gt;g__llama_model_meta_key_by_index_native|23_0(SafeLlamaModelHandle, Int32, Byte*, Int64)**

```csharp
internal static int <llama_model_meta_key_by_index>g__llama_model_meta_key_by_index_native|23_0(SafeLlamaModelHandle model, int index, Byte* buf, long buf_size)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`buf` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`buf_size` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **&lt;llama_model_meta_val_str_by_index&gt;g__llama_model_meta_val_str_by_index_native|24_0(SafeLlamaModelHandle, Int32, Byte*, Int64)**

```csharp
internal static int <llama_model_meta_val_str_by_index>g__llama_model_meta_val_str_by_index_native|24_0(SafeLlamaModelHandle model, int index, Byte* buf, long buf_size)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`buf` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`buf_size` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
