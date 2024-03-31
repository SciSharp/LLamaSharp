# ILLamaExecutor

Namespace: LLama.Abstractions

A high level interface for LLama models.

```csharp
public interface ILLamaExecutor
```

## Properties

### **Context**

The loaded context for this executor.

```csharp
public abstract LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

### **IsMultiModal**

Identify if it's a multi-modal model and there is a image to process.

```csharp
public abstract bool IsMultiModal { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ClipModel**

Muti-Modal Projections / Clip Model weights

```csharp
public abstract LLavaWeights ClipModel { get; }
```

#### Property Value

[LLavaWeights](./llama.llavaweights.md)<br>

### **ImagePaths**

List of images: Image filename and path (jpeg images).

```csharp
public abstract List<string> ImagePaths { get; set; }
```

#### Property Value

[List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

## Methods

### **InferAsync(String, IInferenceParams, CancellationToken)**

Asynchronously infers a response from the model.

```csharp
IAsyncEnumerable<string> InferAsync(string text, IInferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Your prompt

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>
Any additional parameters

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token.

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
