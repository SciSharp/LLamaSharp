[`< Back`](./)

---

# StatelessExecutor

Namespace: LLama

This executor infer the input as one-time job. Previous inputs won't impact on the 
 response to current input.

```csharp
public class StatelessExecutor : LLama.Abstractions.ILLamaExecutor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StatelessExecutor](./llama.statelessexecutor.md)<br>
Implements [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **IsMultiModal**

```csharp
public bool IsMultiModal { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ClipModel**

```csharp
public LLavaWeights ClipModel { get; }
```

#### Property Value

[LLavaWeights](./llama.llavaweights.md)<br>

### **Images**

```csharp
public List<Byte[]> Images { get; }
```

#### Property Value

[List&lt;Byte[]&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Context**

The context used by the executor when running the inference.

```csharp
public LLamaContext Context { get; private set; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

### **ApplyTemplate**

If true, applies the default template to the prompt as defined in the rules for llama_chat_apply_template template.

```csharp
public bool ApplyTemplate { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SystemMessage**

The system message to use with the prompt. Only used when [StatelessExecutor.ApplyTemplate](./llama.statelessexecutor.md#applytemplate) is true.

```csharp
public string SystemMessage { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **StatelessExecutor(LLamaWeights, IContextParams, ILogger)**

Create a new stateless executor which will use the given model

```csharp
public StatelessExecutor(LLamaWeights weights, IContextParams params, ILogger logger)
```

#### Parameters

`weights` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IContextParams](./llama.abstractions.icontextparams.md)<br>

`logger` ILogger<br>

## Methods

### **InferAsync(String, IInferenceParams, CancellationToken)**

```csharp
public IAsyncEnumerable<string> InferAsync(string prompt, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

---

[`< Back`](./)
