[`< Back`](./)

---

# LLamaWeights

Namespace: LLama

A set of model weights, loaded into memory.

```csharp
public sealed class LLamaWeights : System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LLamaWeights](./llama.llamaweights.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **NativeHandle**

The native handle, which is used in the native APIs

```csharp
public SafeLlamaModelHandle NativeHandle { get; }
```

#### Property Value

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

**Remarks:**

Be careful how you use this!

### **ContextSize**

Total number of tokens in the context

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SizeInBytes**

Get the size of this model in bytes

```csharp
public ulong SizeInBytes { get; }
```

#### Property Value

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **ParameterCount**

Get the number of parameters in this model

```csharp
public ulong ParameterCount { get; }
```

#### Property Value

[UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Vocab**

Get the special tokens of this model

```csharp
public Vocabulary Vocab { get; }
```

#### Property Value

[Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

### **Metadata**

All metadata keys in this model

```csharp
public IReadOnlyDictionary<string, string> Metadata { get; set; }
```

#### Property Value

[IReadOnlyDictionary&lt;String, String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br>

## Methods

### **LoadFromFile(IModelParams)**

Load weights into memory

```csharp
public static LLamaWeights LoadFromFile(IModelParams params)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

#### Returns

[LLamaWeights](./llama.llamaweights.md)<br>

### **LoadFromFileAsync(IModelParams, CancellationToken, IProgress&lt;Single&gt;)**

Load weights into memory

```csharp
public static async Task<LLamaWeights> LoadFromFileAsync(IModelParams params, CancellationToken token = null, IProgress<float>? progressReporter = null)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>
Parameters to use to load the model

`token` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token that can interrupt model loading

`progressReporter` [IProgress&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iprogress-1)?<br>
Receives progress updates as the model loads (0 to 1)

#### Returns

[Task&lt;LLamaWeights&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[LoadWeightsFailedException](./llama.exceptions.loadweightsfailedexception.md)<br>
Thrown if weights failed to load for any reason. e.g. Invalid file format or loading cancelled.

[OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception)<br>
Thrown if the cancellation token is cancelled.

### **Dispose()**

```csharp
public void Dispose()
```

### **CreateContext**

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert a string of text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`add_bos` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`special` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[LLamaToken[]](./llama.native.llamatoken.md)<br>

---

[`< Back`](./)
