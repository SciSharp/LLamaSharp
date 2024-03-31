# LLamaModelV1

Namespace: LLama

#### Caution

This type is obsolete.

---

```csharp
public class LLamaModelV1
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaModelV1](./llama.llamamodelv1.md)

## Constructors

### **LLamaModelV1(String, Int32, Int32, Int32, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Int32, Int32, Int32, String, String, Boolean)**

```csharp
public LLamaModelV1(string model_path, int n_ctx, int n_parts, int seed, bool f16_kv, bool logits_all, bool vocab_only, bool use_mmap, bool use_mlock, bool embedding, int n_threads, int n_batch, int last_n_tokens_size, string lora_base, string lora_path, bool verbose)
```

#### Parameters

`model_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`n_ctx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_parts` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`f16_kv` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`logits_all` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`vocab_only` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mmap` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mlock` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`embedding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_batch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`last_n_tokens_size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`lora_base` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`lora_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`verbose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LLamaModelV1(LLamaModelV1)**

```csharp
public LLamaModelV1(LLamaModelV1 other)
```

#### Parameters

`other` [LLamaModelV1](./llama.llamamodelv1.md)<br>

## Methods

### **Tokenize(String)**

```csharp
public List<int> Tokenize(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[List&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **DeTokenize(IEnumerable&lt;Int32&gt;)**

```csharp
public string DeTokenize(IEnumerable<int> tokens)
```

#### Parameters

`tokens` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **DeTokenize(Int32)**

```csharp
public string DeTokenize(int token)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SetCache(LLamaCache)**

```csharp
public void SetCache(LLamaCache cache)
```

#### Parameters

`cache` [LLamaCache](./llama.llamacache.md)<br>

### **Reset()**

```csharp
public void Reset()
```

### **Eval(List&lt;Int32&gt;)**

```csharp
public void Eval(List<int> tokens)
```

#### Parameters

`tokens` [List&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Sample(Int32, Single, Single, Single, Single, Single)**

```csharp
public int Sample(int top_k, float top_p, float temp, float repeat_penalty, float frequency_penalty, float presence_penalty)
```

#### Parameters

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Generate(IEnumerable&lt;Int32&gt;, Int32, Single, Single, Single, Single, Single, Boolean)**

```csharp
public IEnumerable<int> Generate(IEnumerable<int> tokens, int top_k, float top_p, float temp, float repeat_penalty, float frequency_penalty, float presence_penalty, bool reset)
```

#### Parameters

`tokens` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`reset` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **CreateEmbedding(String)**

```csharp
public Embedding CreateEmbedding(string input)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Embedding](./llama.types.embedding.md)<br>

### **Embed(String)**

```csharp
public Single[] Embed(string input)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **CreateCompletion(String, String, Int32, Single, Single, Int32, Boolean, String[], Single, Single, Single, Int32)**

```csharp
public IEnumerable<CompletionChunk> CreateCompletion(string prompt, string suffix, int max_tokens, float temperature, float top_p, int logprobs, bool echo, String[] stop, float frequency_penalty, float presence_penalty, float repeat_penalty, int top_k)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`suffix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`temperature` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`logprobs` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`echo` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`stop` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[IEnumerable&lt;CompletionChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Call(String, String, Int32, Single, Single, Int32, Boolean, String[], Single, Single, Single, Int32)**

```csharp
public IEnumerable<CompletionChunk> Call(string prompt, string suffix, int max_tokens, float temperature, float top_p, int logprobs, bool echo, String[] stop, float frequency_penalty, float presence_penalty, float repeat_penalty, int top_k)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`suffix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`temperature` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`logprobs` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`echo` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`stop` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[IEnumerable&lt;CompletionChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **CreateChatCompletion(IEnumerable&lt;ChatCompletionMessage&gt;, Single, Single, Int32, String[], Int32, Single, Single, Single)**

```csharp
public IEnumerable<ChatCompletionChunk> CreateChatCompletion(IEnumerable<ChatCompletionMessage> messages, float temperature, float top_p, int top_k, String[] stop, int max_tokens, float presence_penalty, float frequency_penalty, float repeat_penalty)
```

#### Parameters

`messages` [IEnumerable&lt;ChatCompletionMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`temperature` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`stop` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`max_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[IEnumerable&lt;ChatCompletionChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **SaveState()**

```csharp
public LLamaState SaveState()
```

#### Returns

[LLamaState](./llama.llamastate.md)<br>

### **LoadState(LLamaState)**

```csharp
public void LoadState(LLamaState state)
```

#### Parameters

`state` [LLamaState](./llama.llamastate.md)<br>

### **LongestTokenPrefix(IEnumerable&lt;Int32&gt;, IEnumerable&lt;Int32&gt;)**

```csharp
internal static int LongestTokenPrefix(IEnumerable<int> a, IEnumerable<int> b)
```

#### Parameters

`a` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`b` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **&lt;CreateChatCompletion&gt;g__GetRole|31_0(ChatCompletionMessage)**

```csharp
internal static string <CreateChatCompletion>g__GetRole|31_0(ChatCompletionMessage message)
```

#### Parameters

`message` [ChatCompletionMessage](./llama.types.chatcompletionmessage.md)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
