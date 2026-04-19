[`< Back`](./)

---

# NativeApi

Namespace: LLama.Native

Direct translation of the llama.cpp API

```csharp
public static class NativeApi
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeApi](./llama.native.nativeapi.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Methods

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

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **llama_supports_mmap()**

Check if memory mapping is supported

```csharp
public static bool llama_supports_mmap()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_mlock()**

Check if memory locking is supported

```csharp
public static bool llama_supports_mlock()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_gpu_offload()**

Check if GPU offload is supported

```csharp
public static bool llama_supports_gpu_offload()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_supports_rpc()**

Check if RPC offload is supported

```csharp
public static bool llama_supports_rpc()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_load_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64, UInt64&)**

Load session file

```csharp
public static bool llama_state_load_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, UInt64& n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens_out` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_capacity` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`n_token_count_out` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_save_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64)**

Save session file

```csharp
public static bool llama_state_save_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_count` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_state_seq_save_file(SafeLLamaContextHandle, String, LLamaSeqId, LLamaToken*, UIntPtr)**

Saves the specified sequence as a file on specified filepath. Can later be loaded via [NativeApi.llama_state_load_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64, UInt64&)](./llama.native.nativeapi.md#llama_state_load_filesafellamacontexthandle-string-llamatoken-uint64-uint64&)

```csharp
public static UIntPtr llama_state_seq_save_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, UIntPtr n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`seq_id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`tokens` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`n_token_count` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **llama_state_seq_load_file(SafeLLamaContextHandle, String, LLamaSeqId, LLamaToken*, UIntPtr, UIntPtr&)**

Loads a sequence saved as a file via [NativeApi.llama_state_save_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64)](./llama.native.nativeapi.md#llama_state_save_filesafellamacontexthandle-string-llamatoken-uint64) into the specified sequence

```csharp
public static UIntPtr llama_state_seq_load_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, UIntPtr n_token_capacity, UIntPtr& n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`dest_seq_id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`tokens_out` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`n_token_capacity` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`n_token_count_out` [UIntPtr&](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr&)<br>

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **llama_set_causal_attn(SafeLLamaContextHandle, Boolean)**

Set whether to use causal attention or not. If set to true, the model will only attend to the past tokens

```csharp
public static void llama_set_causal_attn(SafeLLamaContextHandle ctx, bool causalAttn)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`causalAttn` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_set_embeddings(SafeLLamaContextHandle, Boolean)**

Set whether the model is in embeddings mode or not.

```csharp
public static void llama_set_embeddings(SafeLLamaContextHandle ctx, bool embeddings)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`embeddings` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, embeddings will be returned but logits will not

### **llama_set_abort_callback(SafeLlamaModelHandle, IntPtr, IntPtr)**

Set abort callback

```csharp
public static void llama_set_abort_callback(SafeLlamaModelHandle ctx, IntPtr abortCallback, IntPtr abortCallbackData)
```

#### Parameters

`ctx` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`abortCallback` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`abortCallbackData` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_n_seq_max(SafeLLamaContextHandle)**

Get the n_seq_max for this context

```csharp
public static uint llama_n_seq_max(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

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

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **llama_chat_apply_template(Byte*, LLamaChatMessage*, UIntPtr, Boolean, Byte*, Int32)**

Apply chat template. Inspired by hf apply_chat_template() on python.

```csharp
public static int llama_chat_apply_template(Byte* tmpl, LLamaChatMessage* chat, UIntPtr n_msg, bool add_ass, Byte* buf, int length)
```

#### Parameters

`tmpl` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
A Jinja template to use for this chat. If this is nullptr, the model’s default chat template will be used instead.

`chat` [LLamaChatMessage*](./llama.native.llamachatmessage*.md)<br>
Pointer to a list of multiple llama_chat_message

`n_msg` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of llama_chat_message in this chat

`add_ass` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to end the prompt with the token(s) that indicate the start of an assistant message.

`buf` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
A buffer to hold the output formatted prompt. The recommended alloc size is 2 * (total number of characters of all messages)

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The size of the allocated buffer

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The total number of bytes of the formatted prompt. If is it larger than the size of buffer, you may need to re-alloc it and then re-apply the template.

### **llama_chat_builtin_templates(Char**, UIntPtr)**

Get list of built-in chat templates

```csharp
public static int llama_chat_builtin_templates(Char** output, UIntPtr len)
```

#### Parameters

`output` [Char**](https://docs.microsoft.com/en-us/dotnet/api/system.char**)<br>

`len` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_token_to_piece(Vocabulary, LLamaToken, Span&lt;Byte&gt;, Int32, Boolean)**

Convert a single token into text

```csharp
public static int llama_token_to_piece(Vocabulary vocab, LLamaToken llamaToken, Span<byte> buffer, int lstrip, bool special)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

`llamaToken` [LLamaToken](./llama.native.llamatoken.md)<br>

`buffer` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
buffer to write string into

`lstrip` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
User can skip up to 'lstrip' leading spaces before copying (useful when encoding/decoding multiple tokens with 'add_space_prefix')

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, special tokens are rendered in the output

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
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

### **llama_kv_self_seq_rm(SafeLLamaContextHandle, LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public static bool llama_kv_self_seq_rm(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Returns false if a partial sequence cannot be removed. Removing a whole sequence never fails

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

`n_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`embd` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_seq_max` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
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

### **llama_apply_adapter_cvec(SafeLLamaContextHandle, Single*, UIntPtr, Int32, Int32, Int32)**

Apply a loaded control vector to a llama_context, or if data is NULL, clear
 the currently loaded vector.
 n_embd should be the size of a single layer's control, and data should point
 to an n_embd x n_layers buffer starting from layer 1.
 il_start and il_end are the layer range the vector should apply to (both inclusive)
 See llama_control_vector_load in common to load a control vector.

```csharp
public static int llama_apply_adapter_cvec(SafeLLamaContextHandle ctx, Single* data, UIntPtr len, int n_embd, int il_start, int il_end)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`data` [Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

`len` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`n_embd` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`il_start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`il_end` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_split_path(String, UIntPtr, String, Int32, Int32)**

Build a split GGUF final path for this chunk.
 llama_split_path(split_path, sizeof(split_path), "/models/ggml-model-q4_0", 2, 4) =&gt; split_path = "/models/ggml-model-q4_0-00002-of-00004.gguf"

```csharp
public static int llama_split_path(string split_path, UIntPtr maxlen, string path_prefix, int split_no, int split_count)
```

#### Parameters

`split_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`maxlen` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`path_prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`split_no` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`split_count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns the split_path length.

### **llama_split_prefix(String, UIntPtr, String, Int32, Int32)**

Extract the path prefix from the split_path if and only if the split_no and split_count match.
 llama_split_prefix(split_prefix, 64, "/models/ggml-model-q4_0-00002-of-00004.gguf", 2, 4) =&gt; split_prefix = "/models/ggml-model-q4_0"

```csharp
public static int llama_split_prefix(string split_prefix, UIntPtr maxlen, string split_path, int split_no, int split_count)
```

#### Parameters

`split_prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`maxlen` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`split_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`split_no` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`split_count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns the split_prefix length.

### **ggml_backend_dev_count()**

Get the number of available backend devices

```csharp
public static UIntPtr ggml_backend_dev_count()
```

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Count of available backend devices

### **ggml_backend_dev_get(UIntPtr)**

Get a backend device by index

```csharp
public static IntPtr ggml_backend_dev_get(UIntPtr i)
```

#### Parameters

`i` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Device index

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the backend device

### **ggml_backend_dev_buffer_type(IntPtr)**

Get the buffer type for a backend device

```csharp
public static IntPtr ggml_backend_dev_buffer_type(IntPtr dev)
```

#### Parameters

`dev` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Backend device pointer

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the buffer type

### **ggml_backend_buft_name(IntPtr)**

Get the name of a buffer type

```csharp
public static IntPtr ggml_backend_buft_name(IntPtr buft)
```

#### Parameters

`buft` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Buffer type pointer

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Name of the buffer type

### **llava_validate_embed_size(SafeLLamaContextHandle, SafeLlavaModelHandle)**

Sanity check for clip &lt;-&gt; llava embed size match

```csharp
public static bool llava_validate_embed_size(SafeLLamaContextHandle ctxLlama, SafeLlavaModelHandle ctxClip)
```

#### Parameters

`ctxLlama` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
LLama Context

`ctxClip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>
Llava Model

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if validate successfully

### **llava_image_embed_make_with_bytes(SafeLlavaModelHandle, Int32, Byte[], Int32)**

Build an image embed from image file bytes

```csharp
public static SafeLlavaImageEmbedHandle llava_image_embed_make_with_bytes(SafeLlavaModelHandle ctx_clip, int n_threads, Byte[] image_bytes, int image_bytes_length)
```

#### Parameters

`ctx_clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>
SafeHandle to the Clip Model

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of threads

`image_bytes` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Binary image in jpeg format

`image_bytes_length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Bytes length of the image

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
SafeHandle to the Embeddings

### **llava_image_embed_make_with_filename(SafeLlavaModelHandle, Int32, String)**

Build an image embed from a path to an image filename

```csharp
public static SafeLlavaImageEmbedHandle llava_image_embed_make_with_filename(SafeLlavaModelHandle ctx_clip, int n_threads, string image_path)
```

#### Parameters

`ctx_clip` [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>
SafeHandle to the Clip Model

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of threads

`image_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Image filename (jpeg) to generate embeddings

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
SafeHandle to the embeddings

### **llava_image_embed_free(IntPtr)**

Free an embedding made with llava_image_embed_make_*

```csharp
public static void llava_image_embed_free(IntPtr embed)
```

#### Parameters

`embed` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Embeddings to release

### **llava_eval_image_embed(SafeLLamaContextHandle, SafeLlavaImageEmbedHandle, Int32, Int32&)**

Write the image represented by embed into the llama context with batch size n_batch, starting at context
 pos n_past. on completion, n_past points to the next position in the context after the image embed.

```csharp
public static bool llava_eval_image_embed(SafeLLamaContextHandle ctx_llama, SafeLlavaImageEmbedHandle embed, int n_batch, Int32& n_past)
```

#### Parameters

`ctx_llama` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Llama Context

`embed` [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
Embedding handle

`n_batch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_past` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success

### **GetLoadedNativeLibrary(NativeLibraryName)**

Get the loaded native library. If you are using netstandard2.0, it will always return null.

```csharp
public static INativeLibrary GetLoadedNativeLibrary(NativeLibraryName name)
```

#### Parameters

`name` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

#### Returns

[INativeLibrary](./llama.abstractions.inativelibrary.md)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **llama_model_quantize(String, String, LLamaModelQuantizeParams&)**

Returns 0 on success

```csharp
public static uint llama_model_quantize(string fname_inp, string fname_out, LLamaModelQuantizeParams& param)
```

#### Parameters

`fname_inp` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`fname_out` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`param` [LLamaModelQuantizeParams&](./llama.native.llamamodelquantizeparams&.md)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
Returns 0 on success

---

[`< Back`](./)
