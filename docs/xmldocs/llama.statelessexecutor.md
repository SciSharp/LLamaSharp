[`< Back`](./)

---

# StatelessExecutor

Namespace: LLama

This executor infer the input as one-time job. Previous inputs won't impact on the 
 response to current input.

```csharp
public class StatelessExecutor : LLama.Abstractions.ILLamaExecutor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StatelessExecutor](./llama.statelessexecutor.md)<br>
Implements [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **IsMultiModal**

Identify if it's a multi-modal model and there is a image to process.

```csharp
public bool IsMultiModal { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ClipModel**

Multi-Modal Projections / Clip Model weights

```csharp
public MtmdWeights? ClipModel { get; }
```

#### Property Value

[MtmdWeights](./llama.mtmdweights.md)<br>

### **Embeds**

List of media: List of media for Multi-Modal models.

```csharp
public List<SafeMtmdEmbed> Embeds { get; }
```

#### Property Value

[List&lt;SafeMtmdEmbed&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

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
public bool ApplyTemplate { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SystemMessage**

The system message to use with the prompt. Only used when [StatelessExecutor.ApplyTemplate](./llama.statelessexecutor.md#applytemplate) is true.

```csharp
public string? SystemMessage { get; init; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **.ctor**

## Methods

### **InferAsync(String, IInferenceParams, CancellationToken)**

Asynchronously infers a response from the model.

```csharp
public IAsyncEnumerable<string> InferAsync(string prompt, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = null)
```

#### Parameters

`prompt` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)?<br>
Any additional parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

---

[`< Back`](./)
