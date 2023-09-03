# SafeLLamaContextHandle

Namespace: LLama.Native

A safe wrapper around a llama_context

```csharp
public sealed class SafeLLamaContextHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
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

### **ModelHandle**

Get the model which this context is using

```csharp
public SafeLlamaModelHandle ModelHandle { get; }
```

#### Property Value

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

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

## Constructors

### **SafeLLamaContextHandle(IntPtr, SafeLlamaModelHandle)**

Create a new SafeLLamaContextHandle

```csharp
public SafeLLamaContextHandle(IntPtr handle, SafeLlamaModelHandle model)
```

#### Parameters

`handle` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
pointer to an allocated llama_context

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
the model which this context was created from

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Create(SafeLlamaModelHandle, LLamaContextParams)**

Create a new llama_state for the given model

```csharp
public static SafeLLamaContextHandle Create(SafeLlamaModelHandle model, LLamaContextParams lparams)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`lparams` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Clone(LLamaContextParams)**

Create a new llama context with a clone of the current llama context state

```csharp
public SafeLLamaContextHandle Clone(LLamaContextParams lparams)
```

#### Parameters

`lparams` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **Tokenize(String, Boolean, Encoding)**

Convert the given text into tokens

```csharp
public Int32[] Tokenize(string text, bool add_bos, Encoding encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The text to tokenize

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the "BOS" token should be added

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Encoding to use for the text

#### Returns

[Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **GetLogits()**

Token logits obtained from the last call to llama_eval()
 The logits for the last token are stored in the last row
 Can be mutated in order to change the probabilities of the next token.<br>
 Rows: n_tokens<br>
 Cols: n_vocab

```csharp
public Span<float> GetLogits()
```

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **TokenToString(Int32, Encoding)**

Convert a token into a string

```csharp
public string TokenToString(int token, Encoding encoding)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Token to decode into a string

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TokenToString(Int32, Encoding, StringBuilder)**

Append a single llama token to a string builder

```csharp
public void TokenToString(int token, Encoding encoding, StringBuilder dest)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Token to decode

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

`dest` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>
string builder to append the result to

### **TokenToSpan(Int32, Span&lt;Byte&gt;)**

Convert a single llama token into bytes

```csharp
public int TokenToSpan(int token, Span<byte> dest)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **Eval(ReadOnlySpan&lt;Int32&gt;, Int32, Int32)**

Run the llama inference to obtain the logits and probabilities for the next token.

```csharp
public bool Eval(ReadOnlySpan<int> tokens, int n_past, int n_threads)
```

#### Parameters

`tokens` [ReadOnlySpan&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The provided batch of new tokens to process

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
the number of tokens to use from previous eval calls

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Returns true on success

### **GetStateSize()**

Get the size of the state, when saved as bytes

```csharp
public ulong GetStateSize()
```

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **GetState(Byte*, UInt64)**

Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public ulong GetState(Byte* dest, ulong size)
```

#### Parameters

`dest` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
The number of bytes written to dest

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if dest is too small

### **GetState(IntPtr, UInt64)**

Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public ulong GetState(IntPtr dest, ulong size)
```

#### Parameters

`dest` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Destination to write to

`size` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
The number of bytes written to dest

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if dest is too small

### **SetState(Byte*)**

Set the raw state of this context

```csharp
public ulong SetState(Byte* src)
```

#### Parameters

`src` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
The pointer to read the state from

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of bytes read from the src pointer

### **SetState(IntPtr)**

Set the raw state of this context

```csharp
public ulong SetState(IntPtr src)
```

#### Parameters

`src` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
The pointer to read the state from

#### Returns

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Number of bytes read from the src pointer
