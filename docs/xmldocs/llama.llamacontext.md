# LLamaContext

Namespace: LLama

A llama_context, which holds all the context required to interact with a model

```csharp
public sealed class LLamaContext : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaContext](./llama.llamacontext.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **VocabCount**

Total number of tokens in vocabulary of this model

```csharp
public int VocabCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ContextSize**

Total number of tokens in the context

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Params**

The model params set for this model.

```csharp
public IModelParams Params { get; set; }
```

#### Property Value

[IModelParams](./llama.abstractions.imodelparams.md)<br>

### **NativeHandle**

The native handle, which is used to be passed to the native APIs

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

**Remarks:**

Be careful how you use this!

### **Encoding**

The encoding set for this model to deal with text input.

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### **EmbeddingLength**

The embedding length of the model, also known as `n_embed`

```csharp
public int EmbeddingLength { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **LLamaContext(IModelParams, ILLamaLogger)**

#### Caution

Use the LLamaWeights.CreateContext instead

---



```csharp
public LLamaContext(IModelParams params, ILLamaLogger logger)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>
Model params.

`logger` [ILLamaLogger](./llama.common.illamalogger.md)<br>
The logger.

### **LLamaContext(LLamaWeights, IModelParams, ILLamaLogger)**

Create a new LLamaContext for the given LLamaWeights

```csharp
public LLamaContext(LLamaWeights model, IModelParams params, ILLamaLogger logger)
```

#### Parameters

`model` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

`logger` [ILLamaLogger](./llama.common.illamalogger.md)<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

## Methods

### **Clone()**

Create a copy of the current state of this context

```csharp
public LLamaContext Clone()
```

#### Returns

[LLamaContext](./llama.llamacontext.md)<br>

### **Tokenize(String, Boolean)**

Tokenize a string.

```csharp
public Int32[] Tokenize(string text, bool addBos)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to add a bos to the text.

#### Returns

[Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **DeTokenize(IEnumerable&lt;Int32&gt;)**

Detokenize the tokens to text.

```csharp
public string DeTokenize(IEnumerable<int> tokens)
```

#### Parameters

`tokens` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SaveState(String)**

Save the state to specified path.

```csharp
public void SaveState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetStateData()**

#### Caution

Use `GetState` instead, this supports larger states (over 2GB)

---

Get the state data as a byte array.

```csharp
public Byte[] GetStateData()
```

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **GetState()**

Get the state data as an opaque handle

```csharp
public State GetState()
```

#### Returns

[State](./llama.llamacontext.state.md)<br>

### **LoadState(String)**

Load the state from specified path.

```csharp
public void LoadState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **LoadState(Byte[])**

Load the state from memory.

```csharp
public void LoadState(Byte[] stateData)
```

#### Parameters

`stateData` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **LoadState(State)**

Load the state from memory.

```csharp
public void LoadState(State state)
```

#### Parameters

`state` [State](./llama.llamacontext.state.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Sample(LLamaTokenDataArray, Nullable`1&, Single, MirostatType, Single, Single, Int32, Single, Single, Single, SafeLLamaGrammarHandle)**

Perform the sampling. Please don't use it unless you fully know what it does.

```csharp
public int Sample(LLamaTokenDataArray candidates, Nullable`1& mirostat_mu, float temperature, MirostatType mirostat, float mirostatTau, float mirostatEta, int topK, float topP, float tfsZ, float typicalP, SafeLLamaGrammarHandle grammar)
```

#### Parameters

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`mirostat_mu` [Nullable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1&)<br>

`temperature` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat` [MirostatType](./llama.common.mirostattype.md)<br>

`mirostatTau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostatEta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`topK` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`topP` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`tfsZ` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`typicalP` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`grammar` [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ApplyPenalty(IEnumerable&lt;Int32&gt;, Dictionary&lt;Int32, Single&gt;, Int32, Single, Single, Single, Boolean)**

Apply the penalty for the tokens. Please don't use it unless you fully know what it does.

```csharp
public LLamaTokenDataArray ApplyPenalty(IEnumerable<int> lastTokens, Dictionary<int, float> logitBias, int repeatLastTokensCount, float repeatPenalty, float alphaFrequency, float alphaPresence, bool penalizeNL)
```

#### Parameters

`lastTokens` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`logitBias` [Dictionary&lt;Int32, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

`repeatLastTokensCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`repeatPenalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alphaFrequency` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`alphaPresence` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`penalizeNL` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

### **Eval(Int32[], Int32)**



```csharp
public int Eval(Int32[] tokens, int pastTokensCount)
```

#### Parameters

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`pastTokensCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The updated `pastTokensCount`.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Eval(List&lt;Int32&gt;, Int32)**



```csharp
public int Eval(List<int> tokens, int pastTokensCount)
```

#### Parameters

`tokens` [List&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`pastTokensCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The updated `pastTokensCount`.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Eval(ReadOnlyMemory&lt;Int32&gt;, Int32)**



```csharp
public int Eval(ReadOnlyMemory<int> tokens, int pastTokensCount)
```

#### Parameters

`tokens` [ReadOnlyMemory&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlymemory-1)<br>

`pastTokensCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The updated `pastTokensCount`.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Eval(ReadOnlySpan&lt;Int32&gt;, Int32)**



```csharp
public int Eval(ReadOnlySpan<int> tokens, int pastTokensCount)
```

#### Parameters

`tokens` [ReadOnlySpan&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`pastTokensCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The updated `pastTokensCount`.

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **GenerateResult(IEnumerable&lt;Int32&gt;)**

```csharp
internal IEnumerable<string> GenerateResult(IEnumerable<int> ids)
```

#### Parameters

`ids` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **TokenToString(Int32)**

Convert a token into a string

```csharp
public string TokenToString(int token)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Dispose()**

```csharp
public void Dispose()
```
