# NativeApi

Namespace: LLama.Native

```csharp
public class NativeApi
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeApi](./llama.native.nativeapi.md)

## Constructors

### **NativeApi()**

```csharp
public NativeApi()
```

## Methods

### **llama_print_timings(SafeLLamaContextHandle)**

```csharp
public static void llama_print_timings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **llama_reset_timings(SafeLLamaContextHandle)**

```csharp
public static void llama_reset_timings(SafeLLamaContextHandle ctx)
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

### **llama_model_quantize(String, String, LLamaFtype, Int32)**

```csharp
public static int llama_model_quantize(string fname_inp, string fname_out, LLamaFtype ftype, int nthread)
```

#### Parameters

`fname_inp` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`fname_out` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ftype` [LLamaFtype](./llama.native.llamaftype.md)<br>

`nthread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_repetition_penalty(SafeLLamaContextHandle, IntPtr, Int32[], UInt64, Single)**

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.

```csharp
public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, IntPtr candidates, Int32[] last_tokens, ulong last_tokens_size, float penalty)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle, IntPtr, Int32[], UInt64, Single, Single)**

Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, IntPtr candidates, Int32[] last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`alpha_frequency` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alpha_presence` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_softmax(SafeLLamaContextHandle, IntPtr)**

Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.

```csharp
public static void llama_sample_softmax(SafeLLamaContextHandle ctx, IntPtr candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

### **llama_sample_top_k(SafeLLamaContextHandle, IntPtr, Int32, UInt64)**

Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_k(SafeLLamaContextHandle ctx, IntPtr candidates, int k, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_top_p(SafeLLamaContextHandle, IntPtr, Single, UInt64)**

Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_p(SafeLLamaContextHandle ctx, IntPtr candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_tail_free(SafeLLamaContextHandle, IntPtr, Single, UInt64)**

Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.

```csharp
public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, IntPtr candidates, float z, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_typical(SafeLLamaContextHandle, IntPtr, Single, UInt64)**

Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.

```csharp
public static void llama_sample_typical(SafeLLamaContextHandle ctx, IntPtr candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_temperature(SafeLLamaContextHandle, IntPtr, Single)**

```csharp
public static void llama_sample_temperature(SafeLLamaContextHandle ctx, IntPtr candidates, float temp)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_token_mirostat(SafeLLamaContextHandle, IntPtr, Single, Single, Int32, Single*)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat(SafeLLamaContextHandle ctx, IntPtr candidates, float tau, float eta, int m, Single* mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`m` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.

`mu` [Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token_mirostat_v2(SafeLLamaContextHandle, IntPtr, Single, Single, Single*)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, IntPtr candidates, float tau, float eta, Single* mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`mu` [Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token_greedy(SafeLLamaContextHandle, IntPtr)**

Selects the token with the highest probability.

```csharp
public static int llama_sample_token_greedy(SafeLLamaContextHandle ctx, IntPtr candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token(SafeLLamaContextHandle, IntPtr)**

Randomly selects a token from the candidates based on their probabilities.

```csharp
public static int llama_sample_token(SafeLLamaContextHandle ctx, IntPtr candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_empty_call()**

```csharp
public static bool llama_empty_call()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_context_default_params()**

```csharp
public static LLamaContextParams llama_context_default_params()
```

#### Returns

[LLamaContextParams](./llama.native.llamacontextparams.md)<br>

### **llama_mmap_supported()**

```csharp
public static bool llama_mmap_supported()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_mlock_supported()**

```csharp
public static bool llama_mlock_supported()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_init_from_file(String, LLamaContextParams)**

Various functions for loading a ggml llama model.
 Allocate (almost) all memory needed for the model.
 Return NULL on failure

```csharp
public static IntPtr llama_init_from_file(string path_model, LLamaContextParams params_)
```

#### Parameters

`path_model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`params_` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_init_backend()**

not great API - very likely to change. 
 Initialize the llama + ggml backend
 Call once at the start of the program

```csharp
public static void llama_init_backend()
```

### **llama_free(IntPtr)**

Frees all allocated memory

```csharp
public static void llama_free(IntPtr ctx)
```

#### Parameters

`ctx` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_apply_lora_from_file(SafeLLamaContextHandle, String, String, Int32)**

Apply a LoRA adapter to a loaded model
 path_base_model is the path to a higher quality model to use as a base for
 the layers modified by the adapter. Can be NULL to use the current loaded model.
 The model needs to be reloaded before applying a new adapter, otherwise the adapter
 will be applied on top of the previous one

```csharp
public static int llama_apply_lora_from_file(SafeLLamaContextHandle ctx, string path_lora, string path_base_model, int n_threads)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_lora` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`path_base_model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns 0 on success

### **llama_get_kv_cache_token_count(SafeLLamaContextHandle)**

Returns the number of tokens in the KV cache

```csharp
public static int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_set_rng_seed(SafeLLamaContextHandle, Int32)**

Sets the current rng seed.

```csharp
public static void llama_set_rng_seed(SafeLLamaContextHandle ctx, int seed)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_get_state_size(SafeLLamaContextHandle)**

Returns the maximum size in bytes of the state (rng, logits, embedding
 and kv_cache) - will often be smaller after compacting tokens

```csharp
public static ulong llama_get_state_size(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_copy_state_data(SafeLLamaContextHandle, Byte[])**

Copies the state to the specified destination address.
 Destination needs to have allocated enough memory.
 Returns the number of bytes copied

```csharp
public static ulong llama_copy_state_data(SafeLLamaContextHandle ctx, Byte[] dest)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`dest` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_set_state_data(SafeLLamaContextHandle, Byte[])**

Set the state reading from the specified address
 Returns the number of bytes read

```csharp
public static ulong llama_set_state_data(SafeLLamaContextHandle ctx, Byte[] src)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`src` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_load_session_file(SafeLLamaContextHandle, String, Int32[], UInt64, UInt64*)**

Load session file

```csharp
public static bool llama_load_session_file(SafeLLamaContextHandle ctx, string path_session, Int32[] tokens_out, ulong n_token_capacity, UInt64* n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens_out` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_token_capacity` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`n_token_count_out` [UInt64*](https://docs.microsoft.com/en-us/dotnet/api/system.uint64*)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_save_session_file(SafeLLamaContextHandle, String, Int32[], UInt64)**

Save session file

```csharp
public static bool llama_save_session_file(SafeLLamaContextHandle ctx, string path_session, Int32[] tokens, ulong n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_token_count` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_eval(SafeLLamaContextHandle, Int32[], Int32, Int32, Int32)**

Run the llama inference to obtain the logits and probabilities for the next token.
 tokens + n_tokens is the provided batch of new tokens to process
 n_past is the number of tokens to use from previous eval calls

```csharp
public static int llama_eval(SafeLLamaContextHandle ctx, Int32[] tokens, int n_tokens, int n_past, int n_threads)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns 0 on success

### **llama_eval_with_pointer(SafeLLamaContextHandle, Int32*, Int32, Int32, Int32)**

```csharp
public static int llama_eval_with_pointer(SafeLLamaContextHandle ctx, Int32* tokens, int n_tokens, int n_past, int n_threads)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`tokens` [Int32*](https://docs.microsoft.com/en-us/dotnet/api/system.int32*)<br>

`n_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_tokenize(SafeLLamaContextHandle, String, Encoding, Int32[], Int32, Boolean)**

Convert the provided text into tokens.
 The tokens pointer must be large enough to hold the resulting tokens.
 Returns the number of tokens on success, no more than n_max_tokens
 Returns a negative number on failure - the number of tokens that would have been returned

```csharp
public static int llama_tokenize(SafeLLamaContextHandle ctx, string text, Encoding encoding, Int32[] tokens, int n_max_tokens, bool add_bos)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_tokenize_native(SafeLLamaContextHandle, SByte[], Int32[], Int32, Boolean)**

```csharp
public static int llama_tokenize_native(SafeLLamaContextHandle ctx, SByte[] text, Int32[] tokens, int n_max_tokens, bool add_bos)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`text` [SByte[]](https://docs.microsoft.com/en-us/dotnet/api/system.sbyte)<br>

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_n_vocab(SafeLLamaContextHandle)**

```csharp
public static int llama_n_vocab(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_n_ctx(SafeLLamaContextHandle)**

```csharp
public static int llama_n_ctx(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_n_embd(SafeLLamaContextHandle)**

```csharp
public static int llama_n_embd(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_get_logits(SafeLLamaContextHandle)**

Token logits obtained from the last call to llama_eval()
 The logits for the last token are stored in the last row
 Can be mutated in order to change the probabilities of the next token
 Rows: n_tokens
 Cols: n_vocab

```csharp
public static Single* llama_get_logits(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **llama_get_embeddings(SafeLLamaContextHandle)**

Get the embeddings for the input
 shape: [n_embd] (1-dimensional)

```csharp
public static Single* llama_get_embeddings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **llama_token_to_str(SafeLLamaContextHandle, Int32)**

Token Id -&gt; String. Uses the vocabulary in the provided context

```csharp
public static IntPtr llama_token_to_str(SafeLLamaContextHandle ctx, int token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to a string.

### **llama_token_bos()**

```csharp
public static int llama_token_bos()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_eos()**

```csharp
public static int llama_token_eos()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_nl()**

```csharp
public static int llama_token_nl()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
