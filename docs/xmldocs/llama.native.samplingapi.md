# SamplingApi

Namespace: LLama.Native

Direct translation of the llama.cpp sampling API

```csharp
public class SamplingApi
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SamplingApi](./llama.native.samplingapi.md)

## Constructors

### **SamplingApi()**

```csharp
public SamplingApi()
```

## Methods

### **llama_sample_grammar(SafeLLamaContextHandle, LLamaTokenDataArray, SafeLLamaGrammarHandle)**

Apply grammar rules to candidate tokens

```csharp
public static void llama_sample_grammar(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, SafeLLamaGrammarHandle grammar)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **llama_sample_repetition_penalty(SafeLLamaContextHandle, LLamaTokenDataArray, Memory&lt;Int32&gt;, UInt64, Single)**

#### Caution

last_tokens_size parameter is no longer needed

---

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.

```csharp
public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<int> last_tokens, ulong last_tokens_size, float penalty)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Memory&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_repetition_penalty(SafeLLamaContextHandle, LLamaTokenDataArray, Memory&lt;Int32&gt;, Single)**

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.

```csharp
public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<int> last_tokens, float penalty)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Memory&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle, LLamaTokenDataArray, Memory&lt;Int32&gt;, UInt64, Single, Single)**

#### Caution

last_tokens_size parameter is no longer needed

---

Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<int> last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Memory&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`last_tokens_size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

`alpha_frequency` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alpha_presence` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle, LLamaTokenDataArray, Memory&lt;Int32&gt;, Single, Single)**

Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<int> last_tokens, float alpha_frequency, float alpha_presence)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`last_tokens` [Memory&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`alpha_frequency` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alpha_presence` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_softmax(SafeLLamaContextHandle, LLamaTokenDataArray)**

Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.

```csharp
public static void llama_sample_softmax(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

### **llama_sample_top_k(SafeLLamaContextHandle, LLamaTokenDataArray, Int32, UInt64)**

Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_k(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, int k, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_top_p(SafeLLamaContextHandle, LLamaTokenDataArray, Single, UInt64)**

Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public static void llama_sample_top_p(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_tail_free(SafeLLamaContextHandle, LLamaTokenDataArray, Single, UInt64)**

Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.

```csharp
public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float z, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_typical(SafeLLamaContextHandle, LLamaTokenDataArray, Single, UInt64)**

Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.

```csharp
public static void llama_sample_typical(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **llama_sample_temperature(SafeLLamaContextHandle, LLamaTokenDataArray, Single)**

Sample with temperature.
 As temperature increases, the prediction becomes diverse but also vulnerable to hallucinations -- generating tokens that are sensible but not factual

```csharp
public static void llama_sample_temperature(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float temp)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **llama_sample_token_mirostat(SafeLLamaContextHandle, LLamaTokenDataArray, Single, Single, Int32, Single&)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, int m, Single& mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
A vector of `LLamaTokenData` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

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

### **llama_sample_token_mirostat_v2(SafeLLamaContextHandle, LLamaTokenDataArray, Single, Single, Single&)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public static int llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, Single& mu)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
A vector of `LLamaTokenData` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`mu` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token_greedy(SafeLLamaContextHandle, LLamaTokenDataArray)**

Selects the token with the highest probability.

```csharp
public static int llama_sample_token_greedy(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **llama_sample_token(SafeLLamaContextHandle, LLamaTokenDataArray)**

Randomly selects a token from the candidates based on their probabilities.

```csharp
public static int llama_sample_token(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>
Pointer to LLamaTokenDataArray

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
