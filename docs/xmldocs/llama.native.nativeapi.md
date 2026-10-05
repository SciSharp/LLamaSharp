[`< Back`](./)

---

# NativeApi

Namespace: LLama.Native

Direct translation of the llama.cpp API

```csharp
public static class NativeApi
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NativeApi](./llama.native.nativeapi.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Methods

### **llama_load_mode_name(LLamaLoadMode)**

Get the canonical name of a particular load mode

```csharp
public static string llama_load_mode_name(LLamaLoadMode load_mode)
```

#### Parameters

`load_mode` [LLamaLoadMode](./llama.native.llamaloadmode.md)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **llama_load_mode_from_str(String)**

Parse a load mode from a string

```csharp
public static LLamaLoadMode llama_load_mode_from_str(string str)
```

#### Parameters

`str` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaLoadMode](./llama.native.llamaloadmode.md)<br>

### **llama_empty_call()**

A method that does nothing. This is a native method, calling it will force the llama native dependencies to be loaded.

```csharp
public static void llama_empty_call()
```

### **llama_backend_free()**

Call once at the end of the program - currently only used for MPI

```csharp
public static void llama_backend_free()
```

### **llama_max_devices()**

Get the maximum number of devices supported by llama.cpp

```csharp
public static long llama_max_devices()
```

#### Returns

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **llama_max_tensor_buft_overrides()**

```csharp
public static IntPtr llama_max_tensor_buft_overrides()
```

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_max_parallel_sequences()**

Maximum number of parallel sequences

```csharp
public static long llama_max_parallel_sequences()
```

#### Returns

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **llama_supports_mmap()**

Check if memory mapping is supported

```csharp
public static bool llama_supports_mmap()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_mlock()**

Check if memory locking is supported

```csharp
public static bool llama_supports_mlock()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_gpu_offload()**

Check if GPU offload is supported

```csharp
public static bool llama_supports_gpu_offload()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_rpc()**

Check if RPC offload is supported

```csharp
public static bool llama_supports_rpc()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_load_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64, out UInt64)**

Load session file

```csharp
public static bool llama_state_load_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens_out` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_capacity` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`out` `n_token_count_out` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_save_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64)**

Save session file

```csharp
public static bool llama_state_save_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_count` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_seq_save_file(SafeLLamaContextHandle, String, LLamaSeqId, LLamaToken*, UIntPtr)**

Saves the specified sequence as a file on specified filepath. Can later be loaded via [NativeApi.llama_state_load_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64, out UInt64)](./llama.native.nativeapi.md#llama_state_load_filesafellamacontexthandle-string-llamatoken-uint64-out-uint64)

```csharp
public static UIntPtr llama_state_seq_save_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, UIntPtr n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`filepath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`seq_id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`tokens` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`n_token_count` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **llama_state_seq_load_file(SafeLLamaContextHandle, String, LLamaSeqId, LLamaToken*, UIntPtr, out UIntPtr)**

Loads a sequence saved as a file via [NativeApi.llama_state_save_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64)](./llama.native.nativeapi.md#llama_state_save_filesafellamacontexthandle-string-llamatoken-uint64) into the specified sequence

```csharp
public static UIntPtr llama_state_seq_load_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, UIntPtr n_token_capacity, out UIntPtr n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`filepath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`dest_seq_id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`tokens_out` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`n_token_capacity` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`out` `n_token_count_out` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **llama_set_abort_callback(SafeLLamaContextHandle, IntPtr, IntPtr)**

Set abort callback

```csharp
public static void llama_set_abort_callback(SafeLLamaContextHandle ctx, IntPtr abortCallback, IntPtr abortCallbackData)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`abortCallback` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`abortCallbackData` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_get_embeddings(SafeLLamaContextHandle)**

Get all output token embeddings.
 When pooling_type == LLAMA_POOLING_TYPE_NONE or when using a generative model, the embeddings for which
 llama_batch.logits[i] != 0 are stored contiguously in the order they have appeared in the batch.
 shape: [n_outputs*n_embd]
 Otherwise, returns an empty span.

```csharp
public static Single* llama_get_embeddings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Single*](https://learn.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **llama_chat_apply_template(Byte*, LLamaChatMessage*, UIntPtr, Boolean, Byte*, Int32)**

Apply chat template. Inspired by hf apply_chat_template() on python.
 <br>
 NOTE: This function does not use a jinja parser. It only support a pre-defined list of template.
 See more: https://github.com/ggml-org/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template

```csharp
public static int llama_chat_apply_template(Byte* tmpl, LLamaChatMessage* chat, UIntPtr n_msg, bool add_ass, Byte* buf, int length)
```

#### Parameters

`tmpl` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
A Jinja template to use for this chat.

`chat` [LLamaChatMessage*](./llama.native.llamachatmessage*.md)<br>
Pointer to a list of multiple llama_chat_message

`n_msg` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of llama_chat_message in this chat

`add_ass` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to end the prompt with the token(s) that indicate the start of an assistant message.

`buf` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
A buffer to hold the output formatted prompt. The recommended alloc size is 2 * (total number of characters of all messages)

`length` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The size of the allocated buffer

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The total number of bytes of the formatted prompt. If is it larger than the size of buffer, you may need to re-alloc it and then re-apply the template.

### **llama_chat_builtin_templates(Char**, UIntPtr)**

Get list of built-in chat templates

```csharp
public static int llama_chat_builtin_templates(Char** output, UIntPtr len)
```

#### Parameters

`output` [Char**](https://learn.microsoft.com/en-us/dotnet/api/system.char**)<br>

`len` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_print_timings(SafeLLamaContextHandle)**

Print out timing information for this context

```csharp
public static void llama_print_timings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **llama_print_system_info()**

Print system information

```csharp
public static IntPtr llama_print_system_info()
```

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_token_to_piece(Vocabulary, LLamaToken, Span&lt;Byte&gt;, Int32, Boolean)**

Convert a single token into text

```csharp
public static int llama_token_to_piece(Vocabulary vocab, LLamaToken llamaToken, Span<byte> buffer, int lstrip, bool special)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

`llamaToken` [LLamaToken](./llama.native.llamatoken.md)<br>

`buffer` [Span&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
buffer to write string into

`lstrip` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
User can skip up to 'lstrip' leading spaces before copying (useful when encoding/decoding multiple tokens with 'add_space_prefix')

`special` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, special tokens are rendered in the output

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length written, or if the buffer is too small a negative that indicates the length required

### **llama_log_set(LLamaLogCallback)**

#### Caution

Use `NativeLogConfig.llama_log_set` instead

---

Register a callback to receive llama log messages

```csharp
public static void llama_log_set(LLamaLogCallback logCallback)
```

#### Parameters

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

### **llama_batch_init(Int32, Int32, Int32)**

Allocates a batch of tokens on the heap
 Each token can be assigned up to n_seq_max sequence ids
 The batch has to be freed with llama_batch_free()
 If embd != 0, llama_batch.embd will be allocated with size of n_tokens * embd * sizeof(float)
 Otherwise, llama_batch.token will be allocated to store n_tokens llama_token
 The rest of the llama_batch members are allocated with size n_tokens
 All members are left uninitialized

```csharp
public static LLamaNativeBatch llama_batch_init(int n_tokens, int embd, int n_seq_max)
```

#### Parameters

`n_tokens` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`embd` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_seq_max` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Each token can be assigned up to n_seq_max sequence ids

#### Returns

[LLamaNativeBatch](./llama.native.llamanativebatch.md)<br>

### **llama_batch_free(LLamaNativeBatch)**

Frees a batch of tokens allocated with llama_batch_init()

```csharp
public static void llama_batch_free(LLamaNativeBatch batch)
```

#### Parameters

`batch` [LLamaNativeBatch](./llama.native.llamanativebatch.md)<br>

### **llama_set_adapter_cvec(SafeLLamaContextHandle, Single*, UIntPtr, Int32, Int32, Int32)**

Apply a loaded control vector to a llama_context, or if data is NULL, clear
 the currently loaded vector.
 n_embd should be the size of a single layer's control, and data should point
 to an n_embd x n_layers buffer starting from layer 1.
 il_start and il_end are the layer range the vector should apply to (both inclusive)
 See llama_control_vector_load in common to load a control vector.

```csharp
public static int llama_set_adapter_cvec(SafeLLamaContextHandle ctx, Single* data, UIntPtr len, int n_embd, int il_start, int il_end)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [Single*](https://learn.microsoft.com/en-us/dotnet/api/system.single*)<br>

`len` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`n_embd` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`il_start` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`il_end` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_split_path(Span&lt;Byte&gt;, String, Int32, Int32)**

Build the fully-qualified path for a specific split file in a GGUF shard set.

```csharp
public static int llama_split_path(Span<byte> splitPathBuffer, string pathPrefix, int splitNo, int splitCount)
```

#### Parameters

`splitPathBuffer` [Span&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Writable buffer that receives the UTF-8 encoded path.

`pathPrefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Base path (e.g. "/models/ggml-model-q4_0").

`splitNo` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero-based split index.

`splitCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Total number of splits.

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of bytes written to `splitPathBuffer`.

### **llama_split_path(String, Int32, Int32, Int32)**

Build the fully-qualified path for a specific split file in a GGUF shard set.

```csharp
public static string llama_split_path(string pathPrefix, int splitNo, int splitCount, int maxLength = 1024)
```

#### Parameters

`pathPrefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Base path (e.g. "/models/ggml-model-q4_0").

`splitNo` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero-based split index.

`splitCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Total number of splits.

`maxLength` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of bytes to allocate for the resulting UTF-8 string.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
UTF-8 decoded split path.

### **llama_split_prefix(Span&lt;Byte&gt;, String, Int32, Int32)**

Extract the shard prefix from a GGUF split path when the split metadata matches.

```csharp
public static int llama_split_prefix(Span<byte> splitPrefixBuffer, string splitPath, int splitNo, int splitCount)
```

#### Parameters

`splitPrefixBuffer` [Span&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Writable buffer that receives the UTF-8 encoded prefix.

`splitPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path to a shard file.

`splitNo` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero-based split index.

`splitCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Total number of splits.

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of bytes written to `splitPrefixBuffer`.

### **llama_split_prefix(String, Int32, Int32, Int32)**

Extract the shard prefix from a GGUF split path when the split metadata matches.

```csharp
public static string llama_split_prefix(string splitPath, int splitNo, int splitCount, int maxLength = 1024)
```

#### Parameters

`splitPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path to a shard file.

`splitNo` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero-based split index.

`splitCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Total number of splits.

`maxLength` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of bytes to allocate for the resulting UTF-8 string.

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
UTF-8 decoded split prefix.

### **ggml_backend_dev_count()**

Get the number of available backend devices

```csharp
public static UIntPtr ggml_backend_dev_count()
```

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Count of available backend devices

### **ggml_backend_dev_get(UIntPtr)**

Get a backend device by index

```csharp
public static IntPtr ggml_backend_dev_get(UIntPtr i)
```

#### Parameters

`i` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Device index

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the backend device

### **ggml_backend_dev_name(IntPtr)**

Get the name of a backend device (e.g. "CPU", "Vulkan0", "CUDA1")

```csharp
public static IntPtr ggml_backend_dev_name(IntPtr dev)
```

#### Parameters

`dev` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Backend device pointer

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to a null terminated UTF-8 string, owned by the device

### **ggml_backend_dev_buffer_type(IntPtr)**

Get the buffer type for a backend device

```csharp
public static IntPtr ggml_backend_dev_buffer_type(IntPtr dev)
```

#### Parameters

`dev` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Backend device pointer

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the buffer type

### **ggml_backend_buft_name(IntPtr)**

Get the name of a buffer type

```csharp
public static IntPtr ggml_backend_buft_name(IntPtr buft)
```

#### Parameters

`buft` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Buffer type pointer

#### Returns

[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Name of the buffer type

### **llama_time_us()**

```csharp
public static long llama_time_us()
```

#### Returns

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **GetLoadedNativeLibrary(NativeLibraryName)**

Get the loaded native library. If you are using netstandard2.0, it will always return null.

```csharp
public static INativeLibrary? GetLoadedNativeLibrary(NativeLibraryName name)
```

#### Parameters

`name` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

#### Returns

[INativeLibrary](./llama.abstractions.inativelibrary.md)?<br>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **llama_memory_clear(IntPtr, Boolean)**

Clear the memory contents. If data == true, the data buffers will also be cleared together with the metadata

```csharp
public static void llama_memory_clear(IntPtr mem, bool data)
```

#### Parameters

`mem` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`data` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_memory_seq_rm(IntPtr, LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public static bool llama_memory_seq_rm(IntPtr mem, LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`mem` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Returns false if a partial sequence cannot be removed. Removing a whole sequence never fails

### **MtmdDefaultMarker()**

Retrieve the default multimodal marker text.

```csharp
public static string? MtmdDefaultMarker()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

### **mtmd_get_marker(SafeMtmdModelHandle)**

get the current marker string

```csharp
public static string mtmd_get_marker(SafeMtmdModelHandle ctx)
```

#### Parameters

`ctx` [SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **llama_model_quantize(String, String, ref LLamaModelQuantizeParams)**

Returns 0 on success

```csharp
public static uint llama_model_quantize(string fname_inp, string fname_out, ref LLamaModelQuantizeParams param)
```

#### Parameters

`fname_inp` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`fname_out` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`ref` `param` [LLamaModelQuantizeParams](./llama.native.llamamodelquantizeparams.md)<br>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>
Returns 0 on success

### **llama_ftype_name(LLamaFtype)**

Get the model file type (quantization) as a string, e.g. "Q8_0" or "Q4_K - Medium"

```csharp
public static string llama_ftype_name(LLamaFtype ftype)
```

#### Parameters

`ftype` [LLamaFtype](./llama.native.llamaftype.md)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

---

[`< Back`](./)
