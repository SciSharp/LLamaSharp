# LLamaTokenDataArray

Namespace: LLama.Native

Contains an array of LLamaTokenData, potentially sorted.

```csharp
public struct LLamaTokenDataArray
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)

## Fields

### **data**

The LLamaTokenData

```csharp
public Memory<LLamaTokenData> data;
```

### **sorted**

Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.

```csharp
public bool sorted;
```

## Constructors

### **LLamaTokenDataArray(Memory&lt;LLamaTokenData&gt;, Boolean)**

Create a new LLamaTokenDataArray

```csharp
LLamaTokenDataArray(Memory<LLamaTokenData> tokens, bool isSorted)
```

#### Parameters

`tokens` [Memory&lt;LLamaTokenData&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1)<br>

`isSorted` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Create(ReadOnlySpan&lt;Single&gt;)**

Create a new LLamaTokenDataArray, copying the data from the given logits

```csharp
LLamaTokenDataArray Create(ReadOnlySpan<float> logits)
```

#### Parameters

`logits` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

### **OverwriteLogits(ReadOnlySpan&lt;ValueTuple&lt;LLamaToken, Single&gt;&gt;)**

Overwrite the logit values for all given tokens

```csharp
void OverwriteLogits(ReadOnlySpan<ValueTuple<LLamaToken, float>> values)
```

#### Parameters

`values` [ReadOnlySpan&lt;ValueTuple&lt;LLamaToken, Single&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
tuples of token and logit value to overwrite

### **ApplyGrammar(SafeLLamaContextHandle, SafeLLamaGrammarHandle)**

Apply grammar rules to candidate tokens

```csharp
void ApplyGrammar(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **TopK(SafeLLamaContextHandle, Int32, UInt64)**

Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
void TopK(SafeLLamaContextHandle context, int k, ulong minKeep)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of tokens to keep

`minKeep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Minimum number to keep

### **TopP(SafeLLamaContextHandle, Single, UInt64)**

Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
void TopP(SafeLLamaContextHandle context, float p, ulong minKeep)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`minKeep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **MinP(SafeLLamaContextHandle, Single, UInt64)**

Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841

```csharp
void MinP(SafeLLamaContextHandle context, float p, ulong minKeep)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
All tokens with probability greater than this will be kept

`minKeep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **TailFree(SafeLLamaContextHandle, Single, UInt64)**

Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.

```csharp
void TailFree(SafeLLamaContextHandle context, float z, ulong min_keep)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **LocallyTypical(SafeLLamaContextHandle, Single, UInt64)**

Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.

```csharp
void LocallyTypical(SafeLLamaContextHandle context, float p, ulong min_keep)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`min_keep` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **RepetitionPenalty(SafeLLamaContextHandle, ReadOnlySpan&lt;LLamaToken&gt;, Single, Single, Single)**

Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
 Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.

```csharp
void RepetitionPenalty(SafeLLamaContextHandle context, ReadOnlySpan<LLamaToken> last_tokens, float penalty_repeat, float penalty_freq, float penalty_present)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`last_tokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`penalty_repeat` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`penalty_freq` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`penalty_present` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Guidance(SafeLLamaContextHandle, ReadOnlySpan&lt;Single&gt;, Single)**

Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806

```csharp
void Guidance(SafeLLamaContextHandle context, ReadOnlySpan<float> guidanceLogits, float guidance)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`guidanceLogits` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Logits extracted from a separate context from the same model.
 Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.

`guidance` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Guidance strength. 0 means no guidance, higher values applies stronger guidance

### **Temperature(SafeLLamaContextHandle, Single)**

Sample with temperature.
 As temperature increases, the prediction becomes more diverse but also vulnerable to hallucinations -- generating tokens that are sensible but not factual

```csharp
void Temperature(SafeLLamaContextHandle context, float temp)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Softmax(SafeLLamaContextHandle)**

Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.

```csharp
void Softmax(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **SampleToken(SafeLLamaContextHandle)**

Randomly selects a token from the candidates based on their probabilities.

```csharp
LLamaToken SampleToken(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **SampleTokenGreedy(SafeLLamaContextHandle)**

Selects the token with the highest probability.

```csharp
LLamaToken SampleTokenGreedy(SafeLLamaContextHandle context)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **SampleTokenMirostat(SafeLLamaContextHandle, Single, Single, Int32, Single&)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
LLamaToken SampleTokenMirostat(SafeLLamaContextHandle context, float tau, float eta, int m, Single& mu)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

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

### **SampleTokenMirostat2(SafeLLamaContextHandle, Single, Single, Single&)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
LLamaToken SampleTokenMirostat2(SafeLLamaContextHandle context, float tau, float eta, Single& mu)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`mu` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>
