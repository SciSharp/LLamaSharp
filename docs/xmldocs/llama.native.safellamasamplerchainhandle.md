[`< Back`](./)

---

# SafeLLamaSamplerChainHandle

Namespace: LLama.Native

A chain of sampler stages that can be used to select tokens from logits.

```csharp
public sealed class SafeLLamaSamplerChainHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

**Remarks:**

Wraps a handle returned from `llama_sampler_chain_init`. Other samplers are owned by this chain and are never directly exposed.

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **Count**

Get the number of samplers in this chain

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **SafeLLamaSamplerChainHandle()**

```csharp
public SafeLLamaSamplerChainHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected override bool ReleaseHandle()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Apply(ref LLamaTokenDataArrayNative)**

Apply this sampler to a set of candidates

```csharp
public void Apply(ref LLamaTokenDataArrayNative candidates)
```

#### Parameters

`ref` `candidates` [LLamaTokenDataArrayNative](./llama.native.llamatokendataarraynative.md)<br>

### **Sample(SafeLLamaContextHandle, Int32)**

Sample and accept a token from the idx-th output of the last evaluation. Shorthand for:

```csharp
var logits = ctx.GetLogitsIth(idx);
var token_data_array = LLamaTokenDataArray.Create(logits);
using LLamaTokenDataArrayNative.Create(token_data_array, out var native_token_data);
sampler_chain.Apply(native_token_data);
var token = native_token_data.Data.Span[native_token_data.Selected];
sampler_chain.Accept(token);
return token;
```

```csharp
public LLamaToken Sample(SafeLLamaContextHandle context, int index)
```

#### Parameters

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Reset()**

Reset the state of this sampler

```csharp
public void Reset()
```

### **Accept(LLamaToken)**

Accept a token and update the internal state of this sampler

```csharp
public void Accept(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **GetName(Int32)**

Get the name of the sampler at the given index

```csharp
public string GetName(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetSeed(Int32)**

Get the seed of the sampler at the given index if applicable. returns LLAMA_DEFAULT_SEED otherwise

```csharp
public uint GetSeed(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Create(LLamaSamplerChainParams)**

Create a new sampler chain

```csharp
public static SafeLLamaSamplerChainHandle Create(LLamaSamplerChainParams params)
```

#### Parameters

`params` [LLamaSamplerChainParams](./llama.native.llamasamplerchainparams.md)<br>

#### Returns

[SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

### **AddClone(SafeLLamaSamplerChainHandle, Int32)**

Clone a sampler stage from another chain and add it to this chain

```csharp
public void AddClone(SafeLLamaSamplerChainHandle src, int index)
```

#### Parameters

`src` [SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>
The chain to clone a stage from

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index of the stage to clone

### **Remove(Int32)**

Remove a sampler stage from this chain

```csharp
public void Remove(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Exceptions

[ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>

### **AddCustom&lt;TSampler&gt;(TSampler)**

Add a custom sampler stage

```csharp
public void AddCustom<TSampler>(TSampler sampler) where TSampler : class, ICustomSampler
```

#### Type Parameters

`TSampler`<br>

#### Parameters

`sampler` TSampler<br>

### **AddGreedySampler()**

Add a sampler which picks the most likely token.

```csharp
public void AddGreedySampler()
```

### **AddDistributionSampler(UInt32)**

Add a sampler which picks from the probability distribution of all tokens

```csharp
public void AddDistributionSampler(uint seed)
```

#### Parameters

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **AddMirostat1Sampler(Int32, UInt32, Single, Single, Int32)**

Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public void AddMirostat1Sampler(int vocabCount, uint seed, float tau, float eta, int m)
```

#### Parameters

`vocabCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

`tau` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

`m` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.

### **AddMirostat2Sampler(UInt32, Single, Single)**

Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.

```csharp
public void AddMirostat2Sampler(uint seed, float tau, float eta)
```

#### Parameters

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

`tau` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.

`eta` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.

### **AddTopK(Int32)**

Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public void AddTopK(int k)
```

#### Parameters

`k` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

**Remarks:**

Setting k &lt;= 0 makes this a noop

### **AddTopNSigma(Single)**

Top n sigma sampling as described in academic paper "Top-nσ: Not All Logits Are You Need" https://arxiv.org/pdf/2411.07641

```csharp
public void AddTopNSigma(float n)
```

#### Parameters

`n` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **AddTopP(Single, IntPtr)**

Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751

```csharp
public void AddTopP(float p, IntPtr minKeep)
```

#### Parameters

`p` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`minKeep` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **AddMinP(Single, IntPtr)**

Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841

```csharp
public void AddMinP(float p, IntPtr minKeep)
```

#### Parameters

`p` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`minKeep` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **AddTypical(Single, IntPtr)**

Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.

```csharp
public void AddTypical(float p, IntPtr minKeep)
```

#### Parameters

`p` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`minKeep` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

### **AddTemperature(Single)**

Apply temperature to the logits.
 If temperature is less than zero the maximum logit is left unchanged and the rest are set to -infinity

```csharp
public void AddTemperature(float t)
```

#### Parameters

`t` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **AddDynamicTemperature(Single, Single, Single)**

Dynamic temperature implementation (a.k.a. entropy) described in the paper https://arxiv.org/abs/2309.02772.

```csharp
public void AddDynamicTemperature(float t, float delta, float exponent)
```

#### Parameters

`t` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`delta` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`exponent` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **AddXTC(Single, Single, Int32, UInt32)**

XTC sampler as described in https://github.com/oobabooga/text-generation-webui/pull/6335

```csharp
public void AddXTC(float p, float t, int minKeep, uint seed)
```

#### Parameters

`p` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`t` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

`minKeep` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **AddFillInMiddleInfill(SafeLlamaModelHandle)**

This sampler is meant to be used for fill-in-the-middle infilling, after top_k + top_p sampling
 <br>
 1. if the sum of the EOG probs times the number of candidates is higher than the sum of the other probs -&gt; pick EOG<br>
 2. combine probs of tokens that have the same prefix<br><br>
 example:<br><br>
 - before:<br>
 "abc": 0.5<br>
 "abcd": 0.2<br>
 "abcde": 0.1<br>
 "dummy": 0.1<br><br>
 - after:<br>
 "abc": 0.8<br>
 "dummy": 0.1<br><br>
 3. discard non-EOG tokens with low prob<br>
 4. if no tokens are left -&gt; pick EOT

```csharp
public void AddFillInMiddleInfill(SafeLlamaModelHandle model)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

### **AddGrammar(SafeLlamaModelHandle, String, String)**

Create a sampler which makes tokens impossible unless they match the grammar.

```csharp
public void AddGrammar(SafeLlamaModelHandle model, string grammar, string root)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
The model that this grammar will be used with

`grammar` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`root` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Root rule of the grammar

### **AddGrammar(Vocabulary, String, String)**

Create a sampler which makes tokens impossible unless they match the grammar.

```csharp
public void AddGrammar(Vocabulary vocab, string grammar, string root)
```

#### Parameters

`vocab` [Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>
The vocabulary that this grammar will be used with

`grammar` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`root` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Root rule of the grammar

### **AddLazyGrammar(SafeLlamaModelHandle, String, String, ReadOnlySpan&lt;String&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

Create a sampler using lazy grammar sampling: https://github.com/ggerganov/llama.cpp/pull/9639

```csharp
public void AddLazyGrammar(SafeLlamaModelHandle model, string grammar, string root, ReadOnlySpan<string> patterns, ReadOnlySpan<LLamaToken> triggerTokens)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`grammar` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Grammar in GBNF form

`root` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Root rule of the grammar

`patterns` [ReadOnlySpan&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A list of patterns that will trigger the grammar sampler. Pattern will be matched from the start of the generation output, and grammar sampler will be fed content starting from its first match group.

`triggerTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
A list of tokens that will trigger the grammar sampler. Grammar sampler will be fed content starting from the trigger token included..

### **AddPenalties(Int32, Single, Single, Single)**

Create a sampler that applies various repetition penalties.
 
 Avoid using on the full vocabulary as searching for repeated tokens can become slow. For example, apply top-k or top-p sampling first.

```csharp
public void AddPenalties(int penaltyCount, float repeat, float freq, float presence)
```

#### Parameters

`penaltyCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
How many tokens of history to consider when calculating penalties

`repeat` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
Repetition penalty

`freq` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
Frequency penalty

`presence` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
Presence penalty

### **AddDry(SafeLlamaModelHandle, ReadOnlySpan&lt;String&gt;, Single, Single, Int32, Int32)**

DRY sampler, designed by p-e-w, as described in: https://github.com/oobabooga/text-generation-webui/pull/5677.
 Porting Koboldcpp implementation authored by pi6am: https://github.com/LostRuins/koboldcpp/pull/982

```csharp
public void AddDry(SafeLlamaModelHandle model, ReadOnlySpan<string> sequenceBreakers, float multiplier = 0.8f, float base = 1.75f, int allowedLength = 2, int penaltyLastN = 0)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
The model this sampler will be used with

`sequenceBreakers` [ReadOnlySpan&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`multiplier` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
penalty multiplier, 0.0 = disabled

`base` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
exponential base

`allowedLength` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
repeated sequences longer than this are penalized

`penaltyLastN` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
how many tokens to scan for repetitions (0 = entire context)

### **AddAdaptiveP(Single, Single, UInt32)**

Adaptive-p: select tokens near a configurable target probability over time.
 <br><br>
 the adaptive-p sampler transforms the token probability distribution to favor tokens
 that fall near a user-configurable probability target.
 <br><br>
 internally, the sampler maintains an exponential moving average of the *ORIGINAL*
 probabilities of selected tokens at each sampling step. it uses this EMA to compute an
 adapted target probability at each sampling step, thus maintaining the desired target
 probability over time.
 <br><br>
 adaptive-p selects a token ID rather than just mutating candidates, so it must be last
 in the sampler chain (like mirostat, dist, greedy).
 <br><br>
 only mild truncation before this sampler is recommended. we suggest applying min-p
 before adaptive-p as the only other active sampler in the chain.
 <br><br>
 ref: https://github.com/ggml-org/llama.cpp/pull/17927

```csharp
public void AddAdaptiveP(float target, float decay, uint seed)
```

#### Parameters

`target` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
select tokens near this probability (valid range 0.0 to 1.0; negative = disabled)

`decay` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>
EMA decay for adaptation; history ≈ 1/(1-decay) tokens (valid range 0.0 - 0.99)

`seed` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>
RNG seed

### **AddLogitBias(Int32, Span&lt;LLamaLogitBias&gt;)**

Create a sampler that applies a bias directly to the logits

```csharp
public void AddLogitBias(int vocabSize, Span<LLamaLogitBias> biases)
```

#### Parameters

`vocabSize` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`biases` [Span&lt;LLamaLogitBias&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>

---

[`< Back`](./)
