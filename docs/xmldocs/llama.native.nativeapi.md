# NativeApi

Namespace: LLama.Native

Direct translation of the llama.cpp API

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

### **llama_sample_token_mirostat(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, Single, Int32, Single&)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float tau, float eta, int m, Single& mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`m` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.

`mu` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token_mirostat_v2(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, Single, Single&)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float tau, float eta, Single& mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`mu` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token_greedy(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Selects the token with the highest probability.

```csharp
public static int llama_sample_token_greedy(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Randomly selects a token from the candidates based on their probabilities.

```csharp
public static int llama_sample_token(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **llama_token_bos(SafeLLamaContextHandle)**

Get the "Beginning of sentence" token

```csharp
public static int llama_token_bos(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_eos(SafeLLamaContextHandle)**

Get the "End of sentence" token

```csharp
public static int llama_token_eos(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_nl(SafeLLamaContextHandle)**

Get the "new line" token

```csharp
public static int llama_token_nl(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_print_timings(SafeLLamaContextHandle)**

Print out timing information for this context

```csharp
public static void llama_print_timings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **llama_reset_timings(SafeLLamaContextHandle)**

Reset all collected timing information for this context

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

### **llama_model_n_vocab(SafeLlamaModelHandle)**

Get the number of tokens in the model vocabulary

```csharp
public static int llama_model_n_vocab(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_model_n_ctx(SafeLlamaModelHandle)**

Get the size of the context window for the model

```csharp
public static int llama_model_n_ctx(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_model_n_embd(SafeLlamaModelHandle)**

Get the dimension of embedding vectors from this model

```csharp
public static int llama_model_n_embd(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_to_piece_with_model(SafeLlamaModelHandle, Int32, Byte*, Int32)**

Convert a single token into text

```csharp
public static int llama_token_to_piece_with_model(SafeLlamaModelHandle model, int llamaToken, Byte* buffer, int length)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`llamaToken` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`buffer` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
buffer to write string into

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
size of the buffer

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length writte, or if the buffer is too small a negative that indicates the length required

### **llama_tokenize_with_model(SafeLlamaModelHandle, Byte*, Int32*, Int32, Boolean)**

Convert text into tokens

```csharp
public static int llama_tokenize_with_model(SafeLlamaModelHandle model, Byte* text, Int32* tokens, int n_max_tokens, bool add_bos)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`text` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`tokens` [Int32*](https://docs.microsoft.com/en-us/dotnet/api/system.int32*)<br>

`n_max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns the number of tokens on success, no more than n_max_tokens.
 Returns a negative number on failure - the number of tokens that would have been returned

### **llama_log_set(LLamaLogCallback)**

Register a callback to receive llama log messages

```csharp
public static void llama_log_set(LLamaLogCallback logCallback)
```

#### Parameters

`logCallback` [LLamaLogCallback](./llama.native.llamalogcallback.md)<br>

### **llama_grammar_init(LLamaGrammarElement**, UInt64, UInt64)**

Create a new grammar from the given set of grammar rules

```csharp
public static IntPtr llama_grammar_init(LLamaGrammarElement** rules, ulong n_rules, ulong start_rule_index)
```

#### Parameters

`rules` [LLamaGrammarElement**](./llama.native.llamagrammarelement**.md)<br>

`n_rules` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`start_rule_index` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_grammar_free(IntPtr)**

Free all memory from the given SafeLLamaGrammarHandle

```csharp
public static void llama_grammar_free(IntPtr grammar)
```

#### Parameters

`grammar` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_sample_grammar(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, SafeLLamaGrammarHandle)**

Apply constraints from grammar

```csharp
public static void llama_sample_grammar(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, SafeLLamaGrammarHandle grammar)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **llama_grammar_accept_token(SafeLLamaContextHandle, SafeLLamaGrammarHandle, Int32)**

Accepts the sampled token into the grammar

```csharp
public static void llama_grammar_accept_token(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, int token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_model_quantize(String, String, LLamaModelQuantizeParams*)**

Returns 0 on success

```csharp
public static int llama_model_quantize(string fname_inp, string fname_out, LLamaModelQuantizeParams* param)
```

#### Parameters

`fname_inp` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`fname_out` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`param` [LLamaModelQuantizeParams*](./llama.native.llamamodelquantizeparams*.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns 0 on success

**Remarks:**

not great API - very likely to change

### **llama_sample_classifier_free_guidance(SafeLLamaContextHandle, LLamaTokenDataArrayNative, SafeLLamaContextHandle, Single)**

Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806

```csharp
public static void llama_sample_classifier_free_guidance(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative candidates, SafeLLamaContextHandle guidanceCtx, float scale)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)<br>
A vector of `llama_token_data` containing the candidate tokens, the logits must be directly extracted from the original generation context without being sorted.

`guidanceCtx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
A separate context from the same model. Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.

### **llama_sample_repetition_penalty(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Int32*, UInt64, Single)**

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.

```csharp
public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, Int32* last_tokens, ulong last_tokens_size, float penalty)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Int32*](https://docs.microsoft.com/en-us/dotnet/api/system.int32*)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Int32*, UInt64, Single, Single)**

Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, Int32* last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Int32*](https://docs.microsoft.com/en-us/dotnet/api/system.int32*)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`alpha_frequency` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alpha_presence` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_classifier_free_guidance(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, SafeLLamaContextHandle, Single)**

Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806

```csharp
public static void llama_sample_classifier_free_guidance(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, SafeLLamaContextHandle guidance_ctx, float scale)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
A vector of `llama_token_data` containing the candidate tokens, the logits must be directly extracted from the original generation context without being sorted.

`guidance_ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
A separate context from the same model. Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.

### **llama_sample_softmax(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.

```csharp
public static void llama_sample_softmax(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

### **llama_sample_top_k(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Int32, UInt64)**

Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_k(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, int k, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_top_p(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, UInt64)**

Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_p(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_tail_free(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, UInt64)**

Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.

```csharp
public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float z, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_typical(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, UInt64)**

Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.

```csharp
public static void llama_sample_typical(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_temperature(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single)**

Modify logits by temperature

```csharp
public static void llama_sample_temperature(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float temp)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_empty_call()**

A method that does nothing. This is a native method, calling it will force the llama native dependencies to be loaded.

```csharp
public static bool llama_empty_call()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_context_default_params()**

Create a LLamaContextParams with default values

```csharp
public static LLamaContextParams llama_context_default_params()
```

#### Returns

[LLamaContextParams](./llama.native.llamacontextparams.md)<br>

### **llama_model_quantize_default_params()**

Create a LLamaModelQuantizeParams with default values

```csharp
public static LLamaModelQuantizeParams llama_model_quantize_default_params()
```

#### Returns

[LLamaModelQuantizeParams](./llama.native.llamamodelquantizeparams.md)<br>

### **llama_mmap_supported()**

Check if memory mapping is supported

```csharp
public static bool llama_mmap_supported()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_mlock_supported()**

Check if memory lockingis supported

```csharp
public static bool llama_mlock_supported()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_eval_export(SafeLLamaContextHandle, String)**

Export a static computation graph for context of 511 and batch size of 1
 NOTE: since this functionality is mostly for debugging and demonstration purposes, we hardcode these
 parameters here to keep things simple
 IMPORTANT: do not use for anything else other than debugging and testing!

```csharp
public static int llama_eval_export(SafeLLamaContextHandle ctx, string fname)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`fname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_load_model_from_file(String, LLamaContextParams)**

Various functions for loading a ggml llama model.
 Allocate (almost) all memory needed for the model.
 Return NULL on failure

```csharp
public static IntPtr llama_load_model_from_file(string path_model, LLamaContextParams params)
```

#### Parameters

`path_model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`params` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_new_context_with_model(SafeLlamaModelHandle, LLamaContextParams)**

Create a new llama_context with the given model.
 Return value should always be wrapped in SafeLLamaContextHandle!

```csharp
public static IntPtr llama_new_context_with_model(SafeLlamaModelHandle model, LLamaContextParams params)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`params` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_backend_init(Boolean)**

not great API - very likely to change. 
 Initialize the llama + ggml backend
 Call once at the start of the program

```csharp
public static void llama_backend_init(bool numa)
```

#### Parameters

`numa` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_free(IntPtr)**

Frees all allocated memory in the given llama_context

```csharp
public static void llama_free(IntPtr ctx)
```

#### Parameters

`ctx` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_free_model(IntPtr)**

Frees all allocated memory associated with a model

```csharp
public static void llama_free_model(IntPtr model)
```

#### Parameters

`model` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_model_apply_lora_from_file(SafeLlamaModelHandle, String, String, Int32)**

Apply a LoRA adapter to a loaded model
 path_base_model is the path to a higher quality model to use as a base for
 the layers modified by the adapter. Can be NULL to use the current loaded model.
 The model needs to be reloaded before applying a new adapter, otherwise the adapter
 will be applied on top of the previous one

```csharp
public static int llama_model_apply_lora_from_file(SafeLlamaModelHandle model_ptr, string path_lora, string path_base_model, int n_threads)
```

#### Parameters

`model_ptr` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

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

### **llama_copy_state_data(SafeLLamaContextHandle, Byte*)**

Copies the state to the specified destination address.
 Destination needs to have allocated enough memory.

```csharp
public static ulong llama_copy_state_data(SafeLLamaContextHandle ctx, Byte* dest)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`dest` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
the number of bytes copied

### **llama_copy_state_data(SafeLLamaContextHandle, Byte[])**

Copies the state to the specified destination address.
 Destination needs to have allocated enough memory (see llama_get_state_size)

```csharp
public static ulong llama_copy_state_data(SafeLLamaContextHandle ctx, Byte[] dest)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`dest` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
the number of bytes copied

### **llama_set_state_data(SafeLLamaContextHandle, Byte*)**

Set the state reading from the specified address

```csharp
public static ulong llama_set_state_data(SafeLLamaContextHandle ctx, Byte* src)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`src` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
the number of bytes read

### **llama_set_state_data(SafeLLamaContextHandle, Byte[])**

Set the state reading from the specified address

```csharp
public static ulong llama_set_state_data(SafeLLamaContextHandle ctx, Byte[] src)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`src` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
the number of bytes read

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

Run the llama inference to obtain the logits and probabilities for the next token.
 tokens + n_tokens is the provided batch of new tokens to process
 n_past is the number of tokens to use from previous eval calls

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
Returns 0 on success

### **llama_tokenize(SafeLLamaContextHandle, String, Encoding, Int32[], Int32, Boolean)**

Convert the provided text into tokens.

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
Returns the number of tokens on success, no more than n_max_tokens.
 Returns a negative number on failure - the number of tokens that would have been returned

### **llama_tokenize_native(SafeLLamaContextHandle, Byte*, Int32*, Int32, Boolean)**

Convert the provided text into tokens.

```csharp
public static int llama_tokenize_native(SafeLLamaContextHandle ctx, Byte* text, Int32* tokens, int n_max_tokens, bool add_bos)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`text` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`tokens` [Int32*](https://docs.microsoft.com/en-us/dotnet/api/system.int32*)<br>

`n_max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns the number of tokens on success, no more than n_max_tokens.
 Returns a negative number on failure - the number of tokens that would have been returned

### **llama_n_vocab(SafeLLamaContextHandle)**

Get the number of tokens in the model vocabulary for this context

```csharp
public static int llama_n_vocab(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_n_ctx(SafeLLamaContextHandle)**

Get the size of the context window for the model for this context

```csharp
public static int llama_n_ctx(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_n_embd(SafeLLamaContextHandle)**

Get the dimension of embedding vectors from the model for this context

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
 Can be mutated in order to change the probabilities of the next token.<br>
 Rows: n_tokens<br>
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
