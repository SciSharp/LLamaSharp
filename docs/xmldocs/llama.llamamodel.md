# LLamaModel

Namespace: LLama

```csharp
public class LLamaModel : IChatModel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaModel](./llama.llamamodel.md)<br>
Implements [IChatModel](./llama.ichatmodel.md)

## Properties

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **NativeHandle**

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

## Constructors

### **LLamaModel(String, String, Boolean, Boolean, Int32, Int32, Int32, Int32, Int32, Int32, Int32, Dictionary&lt;Int32, Single&gt;, Int32, Single, Single, Single, Single, Single, Int32, Single, Single, Int32, Single, Single, String, String, String, String, List&lt;String&gt;, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean)**

```csharp
public LLamaModel(string model_path, string model_name, bool echo_input, bool verbose, int seed, int n_threads, int n_predict, int n_parts, int n_ctx, int n_batch, int n_keep, Dictionary<int, float> logit_bias, int top_k, float top_p, float tfs_z, float typical_p, float temp, float repeat_penalty, int repeat_last_n, float frequency_penalty, float presence_penalty, int mirostat, float mirostat_tau, float mirostat_eta, string prompt, string path_session, string input_prefix, string input_suffix, List<string> antiprompt, string lora_adapter, string lora_base, bool memory_f16, bool random_prompt, bool use_color, bool interactive, bool embedding, bool interactive_first, bool instruct, bool penalize_nl, bool perplexity, bool use_mmap, bool use_mlock, bool mem_test, bool verbose_prompt)
```

#### Parameters

`model_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`model_name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`echo_input` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`verbose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_predict` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_parts` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_ctx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_batch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_keep` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logit_bias` [Dictionary&lt;Int32, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`tfs_z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`typical_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_last_n` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`mirostat_tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat_eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`input_prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`input_suffix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`antiprompt` [List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`lora_adapter` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`lora_base` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`memory_f16` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`random_prompt` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_color` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`interactive` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`embedding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`interactive_first` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`instruct` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`penalize_nl` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`perplexity` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mmap` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mlock` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`mem_test` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`verbose_prompt` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LLamaModel(LLamaParams, String, Boolean, Boolean)**

```csharp
public LLamaModel(LLamaParams params, string name, bool echo_input, bool verbose)
```

#### Parameters

`params` [LLamaParams](./llama.llamaparams.md)<br>

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`echo_input` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`verbose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **WithPrompt(String)**

```csharp
public LLamaModel WithPrompt(string prompt)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaModel](./llama.llamamodel.md)<br>

### **WithPromptFile(String)**

```csharp
public LLamaModel WithPromptFile(string promptFileName)
```

#### Parameters

`promptFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaModel](./llama.llamamodel.md)<br>

### **InitChatPrompt(String)**

```csharp
public void InitChatPrompt(string prompt)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InitChatAntiprompt(String[])**

```csharp
public void InitChatAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Chat(String, String)**

```csharp
public IEnumerable<string> Chat(string text, string prompt)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Call(String)**

```csharp
public IEnumerable<string> Call(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
