# LLamaModel

Namespace: LLama

The abstraction of a LLama model, which holds the context in the native library.

```csharp
public class LLamaModel : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaModel](./llama.llamamodel.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **ContextSize**

The context size.

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Params**

The model params set for this model.

```csharp
public ModelParams Params { get; set; }
```

#### Property Value

[ModelParams](./llama.common.modelparams.md)<br>

### **NativeHandle**

The native handle, which is used to be passed to the native APIs. Please avoid using it 
 unless you know what is the usage of the Native API.

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **Encoding**

The encoding set for this model to deal with text input.

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

## Constructors

### **LLamaModel(ModelParams, String, ILLamaLogger)**



```csharp
public LLamaModel(ModelParams Params, string encoding, ILLamaLogger logger)
```

#### Parameters

`Params` [ModelParams](./llama.common.modelparams.md)<br>
Model params.

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Encoding to deal with text input.

`logger` [ILLamaLogger](./llama.common.illamalogger.md)<br>
The logger.

## Methods

### **Tokenize(String, Boolean)**

Tokenize a string.

```csharp
public IEnumerable<int> Tokenize(string text, bool addBos)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to add a bos to the text.

#### Returns

[IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

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

Get the state data as a byte array.

```csharp
public Byte[] GetStateData()
```

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

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

### **Sample(LLamaTokenDataArray, Single, MiroStateType, Single, Single, Int32, Single, Single, Single)**

Perform the sampling. Please don't use it unless you fully know what it does.

```csharp
public int Sample(LLamaTokenDataArray candidates, float temperature, MiroStateType mirostat, float mirostatTau, float mirostatEta, int topK, float topP, float tfsZ, float typicalP)
```

#### Parameters

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`temperature` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat` [MiroStateType](./llama.common.mirostatetype.md)<br>

`mirostatTau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostatEta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`topK` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`topP` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`tfsZ` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`typicalP` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

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

### **GenerateResult(IEnumerable&lt;Int32&gt;)**

```csharp
internal IEnumerable<string> GenerateResult(IEnumerable<int> ids)
```

#### Parameters

`ids` [IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Dispose()**



```csharp
public void Dispose()
```
