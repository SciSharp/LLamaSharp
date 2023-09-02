# LLamaModel

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class LLamaModel : IChatModel, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaModel](./llama.oldversion.llamamodel.md)<br>
Implements [IChatModel](./llama.oldversion.ichatmodel.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Verbose**

```csharp
public bool Verbose { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **NativeHandle**

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

## Constructors

### **LLamaModel(String, String, Boolean, Int32, Int32, Int32, Int32, Int32, Int32, Int32, Dictionary&lt;Int32, Single&gt;, Int32, Single, Single, Single, Single, Single, Int32, Single, Single, Int32, Single, Single, String, String, String, String, List&lt;String&gt;, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String)**

Please refer `LLamaParams` to find the meanings of each arg. Be sure to have set the `n_gpu_layers`, otherwise it will 
 load 20 layers to gpu by default.

```csharp
public LLamaModel(string model_path, string model_name, bool verbose, int seed, int n_threads, int n_predict, int n_ctx, int n_batch, int n_keep, int n_gpu_layers, Dictionary<int, float> logit_bias, int top_k, float top_p, float tfs_z, float typical_p, float temp, float repeat_penalty, int repeat_last_n, float frequency_penalty, float presence_penalty, int mirostat, float mirostat_tau, float mirostat_eta, string prompt, string path_session, string input_prefix, string input_suffix, List<string> antiprompt, string lora_adapter, string lora_base, bool memory_f16, bool random_prompt, bool use_color, bool interactive, bool embedding, bool interactive_first, bool prompt_cache_all, bool instruct, bool penalize_nl, bool perplexity, bool use_mmap, bool use_mlock, bool mem_test, bool verbose_prompt, string encoding)
```

#### Parameters

`model_path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model file path.

`model_name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model name.

`verbose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to print details when running the model.

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_predict` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_ctx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_batch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_keep` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_gpu_layers` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

`prompt_cache_all` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`instruct` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`penalize_nl` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`perplexity` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mmap` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mlock` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`mem_test` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`verbose_prompt` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LLamaModel(LLamaParams, String, Boolean, String)**

Please refer `LLamaParams` to find the meanings of each arg. Be sure to have set the `n_gpu_layers`, otherwise it will 
 load 20 layers to gpu by default.

```csharp
public LLamaModel(LLamaParams params, string name, bool verbose, string encoding)
```

#### Parameters

`params` [LLamaParams](./llama.oldversion.llamaparams.md)<br>
The LLamaModel params

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Model name

`verbose` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to output the detailed info.

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

## Methods

### **WithPrompt(String, String)**

Apply a prompt to the model.

```csharp
public LLamaModel WithPrompt(string prompt, string encoding)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaModel](./llama.oldversion.llamamodel.md)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **WithPromptFile(String)**

Apply the prompt file to the model.

```csharp
public LLamaModel WithPromptFile(string promptFileName)
```

#### Parameters

`promptFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaModel](./llama.oldversion.llamamodel.md)<br>

### **InitChatPrompt(String, String)**

```csharp
public void InitChatPrompt(string prompt, string encoding)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **InitChatAntiprompt(String[])**

```csharp
public void InitChatAntiprompt(String[] antiprompt)
```

#### Parameters

`antiprompt` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Chat(String, String, String)**

Chat with the LLaMa model under interactive mode.

```csharp
public IEnumerable<string> Chat(string text, string prompt, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **SaveState(String)**

Save the state to specified path.

```csharp
public void SaveState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoadState(String, Boolean)**

Load the state from specified path.

```csharp
public void LoadState(string filename, bool clearPreviousEmbed)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`clearPreviousEmbed` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to clear previous footprints of this model.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Tokenize(String, String)**

Tokenize a string.

```csharp
public List<int> Tokenize(string text, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The utf-8 encoded string to tokenize.

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[List&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
A list of tokens.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
If the tokenization failed.

### **DeTokenize(IEnumerable&lt;Int32&gt;)**

Detokenize a list of tokens.

```csharp
public string DeTokenize(IEnumerable<int> tokens)
```

#### Parameters

`tokens` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
The list of tokens to detokenize.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The detokenized string.

### **Call(String, String)**

Call the model to run inference.

```csharp
public IEnumerable<string> Call(string text, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Dispose()**

```csharp
public void Dispose()
```
