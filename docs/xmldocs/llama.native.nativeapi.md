# NativeApi

Namespace: LLama.Native

Direct translation of the llama.cpp API

```csharp
public static class NativeApi
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeApi](./llama.native.nativeapi.md)

## Methods

### **llama_sample_token_mirostat(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, Single, Int32, Single&)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static LLamaToken llama_sample_token_mirostat(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float tau, float eta, int m, Single& mu)
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

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_sample_token_mirostat_v2(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, Single, Single&)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static LLamaToken llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float tau, float eta, Single& mu)
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

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_sample_token_greedy(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Selects the token with the highest probability.

```csharp
public static LLamaToken llama_sample_token_greedy(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_sample_token(SafeLLamaContextHandle, LLamaTokenDataArrayNative&)**

Randomly selects a token from the candidates based on their probabilities.

```csharp
public static LLamaToken llama_sample_token(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **&lt;llama_get_embeddings&gt;g__llama_get_embeddings_native|30_0(SafeLLamaContextHandle)**

```csharp
internal static Single* <llama_get_embeddings>g__llama_get_embeddings_native|30_0(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **&lt;llama_token_to_piece&gt;g__llama_token_to_piece_native|44_0(SafeLlamaModelHandle, LLamaToken, Byte*, Int32)**

```csharp
internal static int <llama_token_to_piece>g__llama_token_to_piece_native|44_0(SafeLlamaModelHandle model, LLamaToken llamaToken, Byte* buffer, int length)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`llamaToken` [LLamaToken](./llama.native.llamatoken.md)<br>

`buffer` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **&lt;TryLoadLibraries&gt;g__TryLoad|84_0(String)**

```csharp
internal static IntPtr <TryLoadLibraries>g__TryLoad|84_0(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **&lt;TryLoadLibraries&gt;g__TryFindPath|84_1(String, &lt;&gt;c__DisplayClass84_0&)**

```csharp
internal static string <TryLoadLibraries>g__TryFindPath|84_1(string filename, <>c__DisplayClass84_0& )
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`` [&lt;&gt;c__DisplayClass84_0&](./llama.native.nativeapi.<>c__displayclass84_0&.md)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **llama_set_n_threads(SafeLLamaContextHandle, UInt32, UInt32)**

Set the number of threads used for decoding

```csharp
public static void llama_set_n_threads(SafeLLamaContextHandle ctx, uint n_threads, uint n_threads_batch)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`n_threads` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
n_threads is the number of threads used for generation (single token)

`n_threads_batch` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
n_threads_batch is the number of threads used for prompt and batch processing (multiple tokens)

### **llama_vocab_type(SafeLlamaModelHandle)**

```csharp
public static LLamaVocabType llama_vocab_type(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaVocabType](./llama.native.llamavocabtype.md)<br>

### **llama_rope_type(SafeLlamaModelHandle)**

```csharp
public static LLamaRopeType llama_rope_type(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaRopeType](./llama.native.llamaropetype.md)<br>

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

### **llama_grammar_copy(SafeLLamaGrammarHandle)**

Create a copy of an existing grammar instance

```csharp
public static IntPtr llama_grammar_copy(SafeLLamaGrammarHandle grammar)
```

#### Parameters

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

#### Returns

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **llama_sample_grammar(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, SafeLLamaGrammarHandle)**

Apply constraints from grammar

```csharp
public static void llama_sample_grammar(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, SafeLLamaGrammarHandle grammar)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **llama_grammar_accept_token(SafeLLamaContextHandle, SafeLLamaGrammarHandle, LLamaToken)**

Accepts the sampled token into the grammar

```csharp
public static void llama_grammar_accept_token(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, LLamaToken token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

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
Bytes lenght of the image

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
SafeHandel to the embeddings

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

### **llama_model_quantize(String, String, LLamaModelQuantizeParams*)**

Returns 0 on success

```csharp
public static uint llama_model_quantize(string fname_inp, string fname_out, LLamaModelQuantizeParams* param)
```

#### Parameters

`fname_inp` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`fname_out` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`param` [LLamaModelQuantizeParams*](./llama.native.llamamodelquantizeparams*.md)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
Returns 0 on success

### **llama_sample_repetition_penalties(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, LLamaToken*, UInt64, Single, Single, Single)**

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
 Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
public static void llama_sample_repetition_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, LLamaToken* last_tokens, ulong last_tokens_size, float penalty_repeat, float penalty_freq, float penalty_present)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`penalty_repeat` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.

`penalty_freq` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

`penalty_present` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

### **llama_sample_apply_guidance(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;Single&gt;, Single)**

Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806

```csharp
public static void llama_sample_apply_guidance(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<float> logits_guidance, float scale)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
Logits extracted from the original generation context.

`logits_guidance` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Logits extracted from a separate context from the same model.
 Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.

### **llama_sample_apply_guidance(SafeLLamaContextHandle, Single*, Single*, Single)**

Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806

```csharp
public static void llama_sample_apply_guidance(SafeLLamaContextHandle ctx, Single* logits, Single* logits_guidance, float scale)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`logits` [Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>
Logits extracted from the original generation context.

`logits_guidance` [Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>
Logits extracted from a separate context from the same model.
 Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.

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

### **llama_sample_min_p(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, UInt64)**

Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841

```csharp
public static void llama_sample_min_p(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float p, ulong min_keep)
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

### **llama_sample_typical(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single, Single, Single)**

Dynamic temperature implementation described in the paper https://arxiv.org/abs/2309.02772.

```csharp
public static void llama_sample_typical(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float min_temp, float max_temp, float exponent_val)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>
Pointer to LLamaTokenDataArray

`min_temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`max_temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`exponent_val` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_temp(SafeLLamaContextHandle, LLamaTokenDataArrayNative&, Single)**

Modify logits by temperature

```csharp
public static void llama_sample_temp(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative& candidates, float temp)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArrayNative&](./llama.native.llamatokendataarraynative&.md)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_get_embeddings(SafeLLamaContextHandle)**

Get the embeddings for the input

```csharp
public static Span<float> llama_get_embeddings(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **llama_chat_apply_template(SafeLlamaModelHandle, Char*, LLamaChatMessage*, IntPtr, Boolean, Char*, Int32)**

Apply chat template. Inspired by hf apply_chat_template() on python.
 Both "model" and "custom_template" are optional, but at least one is required. "custom_template" has higher precedence than "model"
 NOTE: This function does not use a jinja parser. It only support a pre-defined list of template. See more: https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template

```csharp
public static int llama_chat_apply_template(SafeLlamaModelHandle model, Char* tmpl, LLamaChatMessage* chat, IntPtr n_msg, bool add_ass, Char* buf, int length)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`tmpl` [Char*](https://docs.microsoft.com/en-us/dotnet/api/system.char*)<br>
A Jinja template to use for this chat. If this is nullptr, the model’s default chat template will be used instead.

`chat` [LLamaChatMessage*](./llama.native.llamachatmessage*.md)<br>
Pointer to a list of multiple llama_chat_message

`n_msg` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Number of llama_chat_message in this chat

`add_ass` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to end the prompt with the token(s) that indicate the start of an assistant message.

`buf` [Char*](https://docs.microsoft.com/en-us/dotnet/api/system.char*)<br>
A buffer to hold the output formatted prompt. The recommended alloc size is 2 * (total number of characters of all messages)

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The size of the allocated buffer

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The total number of bytes of the formatted prompt. If is it larger than the size of buffer, you may need to re-alloc it and then re-apply the template.

### **llama_token_bos(SafeLlamaModelHandle)**

Get the "Beginning of sentence" token

```csharp
public static LLamaToken llama_token_bos(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_token_eos(SafeLlamaModelHandle)**

Get the "End of sentence" token

```csharp
public static LLamaToken llama_token_eos(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_token_nl(SafeLlamaModelHandle)**

Get the "new line" token

```csharp
public static LLamaToken llama_token_nl(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **llama_add_bos_token(SafeLlamaModelHandle)**

Returns -1 if unknown, 1 for true or 0 for false.

```csharp
public static int llama_add_bos_token(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_add_eos_token(SafeLlamaModelHandle)**

Returns -1 if unknown, 1 for true or 0 for false.

```csharp
public static int llama_add_eos_token(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_prefix(SafeLlamaModelHandle)**

codellama infill tokens, Beginning of infill prefix

```csharp
public static int llama_token_prefix(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_middle(SafeLlamaModelHandle)**

codellama infill tokens, Beginning of infill middle

```csharp
public static int llama_token_middle(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_suffix(SafeLlamaModelHandle)**

codellama infill tokens, Beginning of infill suffix

```csharp
public static int llama_token_suffix(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_token_eot(SafeLlamaModelHandle)**

codellama infill tokens, End of infill middle

```csharp
public static int llama_token_eot(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

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

### **llama_token_to_piece(SafeLlamaModelHandle, LLamaToken, Span&lt;Byte&gt;)**

Convert a single token into text

```csharp
public static int llama_token_to_piece(SafeLlamaModelHandle model, LLamaToken llamaToken, Span<byte> buffer)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`llamaToken` [LLamaToken](./llama.native.llamatoken.md)<br>

`buffer` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
buffer to write string into

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The length written, or if the buffer is too small a negative that indicates the length required

### **llama_tokenize(SafeLlamaModelHandle, Byte*, Int32, LLamaToken*, Int32, Boolean, Boolean)**

Convert text into tokens

```csharp
public static int llama_tokenize(SafeLlamaModelHandle model, Byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_bos, bool special)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`text` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

`text_len` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`tokens` [LLamaToken*](./llama.native.llamatoken*.md)<br>

`n_max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext. Does not insert a leading space.

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

### **llama_kv_cache_clear(SafeLLamaContextHandle)**

Clear the KV cache

```csharp
public static void llama_kv_cache_clear(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **llama_kv_cache_seq_rm(SafeLLamaContextHandle, LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public static void llama_kv_cache_seq_rm(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **llama_kv_cache_seq_cp(SafeLLamaContextHandle, LLamaSeqId, LLamaSeqId, LLamaPos, LLamaPos)**

Copy all tokens that belong to the specified sequence to another sequence
 Note that this does not allocate extra KV cache memory - it simply assigns the tokens to the new sequence

```csharp
public static void llama_kv_cache_seq_cp(SafeLLamaContextHandle ctx, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`src` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`dest` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **llama_kv_cache_seq_keep(SafeLLamaContextHandle, LLamaSeqId)**

Removes all tokens that do not belong to the specified sequence

```csharp
public static void llama_kv_cache_seq_keep(SafeLLamaContextHandle ctx, LLamaSeqId seq)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **llama_kv_cache_seq_add(SafeLLamaContextHandle, LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Adds relative position "delta" to all tokens that belong to the specified sequence and have positions in [p0, p1)
 If the KV cache is RoPEd, the KV data is updated accordingly:
 - lazily on next llama_decode()
 - explicitly with llama_kv_cache_update()

```csharp
public static void llama_kv_cache_seq_add(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`delta` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_kv_cache_seq_div(SafeLLamaContextHandle, LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Integer division of the positions by factor of `d &gt; 1`
 If the KV cache is RoPEd, the KV data is updated accordingly:
 - lazily on next llama_decode()
 - explicitly with llama_kv_cache_update()
 <br>
 p0 &lt; 0 : [0, p1]
 <br>
 p1 &lt; 0 : [p0, inf)

```csharp
public static void llama_kv_cache_seq_div(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`d` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_kv_cache_seq_pos_max(SafeLLamaContextHandle, LLamaSeqId)**

Returns the largest position present in the KV cache for the specified sequence

```csharp
public static LLamaPos llama_kv_cache_seq_pos_max(SafeLLamaContextHandle ctx, LLamaSeqId seq)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

### **llama_kv_cache_defrag(SafeLLamaContextHandle)**

Defragment the KV cache. This will be applied:
 - lazily on next llama_decode()
 - explicitly with llama_kv_cache_update()

```csharp
public static LLamaPos llama_kv_cache_defrag(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

### **llama_kv_cache_update(SafeLLamaContextHandle)**

Apply the KV cache updates (such as K-shifts, defragmentation, etc.)

```csharp
public static void llama_kv_cache_update(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

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

### **llama_decode(SafeLLamaContextHandle, LLamaNativeBatch)**



```csharp
public static int llama_decode(SafeLLamaContextHandle ctx, LLamaNativeBatch batch)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`batch` [LLamaNativeBatch](./llama.native.llamanativebatch.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Positive return values does not mean a fatal error, but rather a warning:<br>
 - 0: success<br>
 - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br>
 - &lt; 0: error<br>

### **llama_kv_cache_view_init(SafeLLamaContextHandle, Int32)**

Create an empty KV cache view. (use only for debugging purposes)

```csharp
public static LLamaKvCacheView llama_kv_cache_view_init(SafeLLamaContextHandle ctx, int n_max_seq)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`n_max_seq` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaKvCacheView](./llama.native.llamakvcacheview.md)<br>

### **llama_kv_cache_view_free(LLamaKvCacheView&)**

Free a KV cache view. (use only for debugging purposes)

```csharp
public static void llama_kv_cache_view_free(LLamaKvCacheView& view)
```

#### Parameters

`view` [LLamaKvCacheView&](./llama.native.llamakvcacheview&.md)<br>

### **llama_kv_cache_view_update(SafeLLamaContextHandle, LLamaKvCacheView&)**

Update the KV cache view structure with the current state of the KV cache. (use only for debugging purposes)

```csharp
public static void llama_kv_cache_view_update(SafeLLamaContextHandle ctx, LLamaKvCacheView& view)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`view` [LLamaKvCacheView&](./llama.native.llamakvcacheview&.md)<br>

### **llama_get_kv_cache_token_count(SafeLLamaContextHandle)**

Returns the number of tokens in the KV cache (slow, use only for debug)
 If a KV cell has multiple sequences assigned to it, it will be counted multiple times

```csharp
public static int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_get_kv_cache_used_cells(SafeLLamaContextHandle)**

Returns the number of used KV cells (i.e. have at least one sequence assigned to them)

```csharp
public static int llama_get_kv_cache_used_cells(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_beam_search(SafeLLamaContextHandle, LLamaBeamSearchCallback, IntPtr, UInt64, Int32, Int32, Int32)**

Deterministically returns entire sentence constructed by a beam search.

```csharp
public static void llama_beam_search(SafeLLamaContextHandle ctx, LLamaBeamSearchCallback callback, IntPtr callback_data, ulong n_beams, int n_past, int n_predict, int n_threads)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Pointer to the llama_context.

`callback` [LLamaBeamSearchCallback](./llama.native.nativeapi.llamabeamsearchcallback.md)<br>
Invoked for each iteration of the beam_search loop, passing in beams_state.

`callback_data` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
A pointer that is simply passed back to callback.

`n_beams` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of beams to use.

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of tokens already evaluated.

`n_predict` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of tokens to predict. EOS may occur earlier.

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of threads.

### **llama_empty_call()**

A method that does nothing. This is a native method, calling it will force the llama native dependencies to be loaded.

```csharp
public static void llama_empty_call()
```

### **llama_max_devices()**

Get the maximum number of devices supported by llama.cpp

```csharp
public static long llama_max_devices()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **llama_model_default_params()**

Create a LLamaModelParams with default values

```csharp
public static LLamaModelParams llama_model_default_params()
```

#### Returns

[LLamaModelParams](./llama.native.llamamodelparams.md)<br>

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

### **llama_set_rng_seed(SafeLLamaContextHandle, UInt32)**

Sets the current rng seed.

```csharp
public static void llama_set_rng_seed(SafeLLamaContextHandle ctx, uint seed)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`seed` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

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

### **llama_load_session_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64, UInt64&)**

Load session file

```csharp
public static bool llama_load_session_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, UInt64& n_token_count_out)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens_out` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_capacity` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`n_token_count_out` [UInt64&](https://docs.microsoft.com/en-us/dotnet/api/system.uint64&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_save_session_file(SafeLLamaContextHandle, String, LLamaToken[], UInt64)**

Save session file

```csharp
public static bool llama_save_session_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`tokens` [LLamaToken[]](./llama.native.llamatoken.md)<br>

`n_token_count` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **llama_token_get_text(SafeLlamaModelHandle, LLamaToken)**

```csharp
public static Byte* llama_token_get_text(SafeLlamaModelHandle model, LLamaToken token)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Returns

[Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>

### **llama_token_get_score(SafeLlamaModelHandle, LLamaToken)**

```csharp
public static float llama_token_get_score(SafeLlamaModelHandle model, LLamaToken token)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Returns

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_token_get_type(SafeLlamaModelHandle, LLamaToken)**

```csharp
public static LLamaTokenType llama_token_get_type(SafeLlamaModelHandle model, LLamaToken token)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Returns

[LLamaTokenType](./llama.native.llamatokentype.md)<br>

### **llama_n_ctx(SafeLLamaContextHandle)**

Get the size of the context window for the model for this context

```csharp
public static uint llama_n_ctx(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **llama_n_batch(SafeLLamaContextHandle)**

Get the batch size for this context

```csharp
public static uint llama_n_batch(SafeLLamaContextHandle ctx)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **llama_get_logits(SafeLLamaContextHandle)**

Token logits obtained from the last call to llama_decode
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

### **llama_get_logits_ith(SafeLLamaContextHandle, Int32)**

Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab

```csharp
public static Single* llama_get_logits_ith(SafeLLamaContextHandle ctx, int i)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`i` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>

### **llama_get_embeddings_ith(SafeLLamaContextHandle, Int32)**

Get the embeddings for the ith sequence. Equivalent to: llama_get_embeddings(ctx) + i*n_embd

```csharp
public static Single* llama_get_embeddings_ith(SafeLLamaContextHandle ctx, int i)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`i` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Single*](https://docs.microsoft.com/en-us/dotnet/api/system.single*)<br>
