# SafeLlamaModelHandle

Namespace: LLama.Native

A reference to a set of llama model weights

```csharp
public sealed class SafeLlamaModelHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
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

### **IsInvalid**

```csharp
public bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LoadFromFile(String, LLamaContextParams)**

Load a model from the given file path into memory

```csharp
public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaContextParams lparams)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`lparams` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **ApplyLoraFromFile(String, String, Int32)**

Apply a LoRA adapter to a loaded model

```csharp
public void ApplyLoraFromFile(string lora, string modelBase, int threads)
```

#### Parameters

`lora` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`modelBase` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A path to a higher quality model to use as a base for the layers modified by the
 adapter. Can be NULL to use the current loaded model.

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **TokenToSpan(Int32, Span&lt;Byte&gt;)**

Convert a single llama token into bytes

```csharp
public int TokenToSpan(int llama_token, Span<byte> dest)
```

#### Parameters

`llama_token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **TokenToString(Int32, Encoding)**

Convert a single llama token into a string

```csharp
public string TokenToString(int llama_token, Encoding encoding)
```

#### Parameters

`llama_token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Encoding to use to decode the bytes into a string

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TokenToString(Int32, Encoding, StringBuilder)**

Append a single llama token to a string builder

```csharp
public void TokenToString(int llama_token, Encoding encoding, StringBuilder dest)
```

#### Parameters

`llama_token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Token to decode

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

`dest` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>
string builder to append the result to

### **Tokenize(String, Boolean, Encoding)**

Convert a string of text into tokens

```csharp
public Int32[] Tokenize(string text, bool add_bos, Encoding encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **CreateContext(LLamaContextParams)**

Create a new context for this model

```csharp
public SafeLLamaContextHandle CreateContext(LLamaContextParams params)
```

#### Parameters

`params` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
