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
public uint ContextSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchSize**

Get the maximum batch size for this context

```csharp
public uint BatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

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

### **SafeLLamaContextHandle()**

```csharp
public SafeLLamaContextHandle()
```

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

### **GetLogits()**

Token logits obtained from the last call to llama_decode
 The logits for the last token are stored in the last row
 Can be mutated in order to change the probabilities of the next token.<br>
 Rows: n_tokens<br>
 Cols: n_vocab

```csharp
public Span<float> GetLogits()
```

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **GetLogitsIth(Int32)**

Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab

```csharp
public Span<float> GetLogitsIth(int i)
```

#### Parameters

`i` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert the given text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The text to tokenize

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the "BOS" token should be added

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Encoding to use for the text

#### Returns

[LLamaToken[]](./llama.native.llamatoken.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **TokenToSpan(LLamaToken, Span&lt;Byte&gt;)**

Convert a single llama token into bytes

```csharp
public uint TokenToSpan(LLamaToken token, Span<byte> dest)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **Decode(LLamaBatch)**



```csharp
public DecodeResult Decode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>
Positive return values does not mean a fatal error, but rather a warning:<br>
 - 0: success<br>
 - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br>
 - &lt; 0: error<br>

### **Decode(List&lt;LLamaToken&gt;, LLamaSeqId, LLamaBatch, Int32&)**

Decode a set of tokens in batch-size chunks.

```csharp
internal ValueTuple<DecodeResult, int> Decode(List<LLamaToken> tokens, LLamaSeqId id, LLamaBatch batch, Int32& n_past)
```

#### Parameters

`tokens` [List&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

`n_past` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[ValueTuple&lt;DecodeResult, Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.valuetuple-2)<br>
A tuple, containing the decode result and the number of tokens that have not been decoded yet.

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

### **SetSeed(UInt32)**

Set the RNG seed

```csharp
public void SetSeed(uint seed)
```

#### Parameters

`seed` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **SetThreads(UInt32, UInt32)**

Set the number of threads used for decoding

```csharp
public void SetThreads(uint threads, uint threadsBatch)
```

#### Parameters

`threads` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
n_threads is the number of threads used for generation (single token)

`threadsBatch` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
n_threads_batch is the number of threads used for prompt and batch processing (multiple tokens)

### **KvCacheGetDebugView(Int32)**

Get a new KV cache view that can be used to debug the KV cache

```csharp
public LLamaKvCacheViewSafeHandle KvCacheGetDebugView(int maxSequences)
```

#### Parameters

`maxSequences` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)<br>

### **KvCacheCountCells()**

Count the number of used cells in the KV cache (i.e. have at least one sequence assigned to them)

```csharp
public int KvCacheCountCells()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheCountTokens()**

Returns the number of tokens in the KV cache (slow, use only for debug)
 If a KV cell has multiple sequences assigned to it, it will be counted multiple times

```csharp
public int KvCacheCountTokens()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheClear()**

Clear the KV cache

```csharp
public void KvCacheClear()
```

### **KvCacheRemove(LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public void KvCacheRemove(LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **KvCacheSequenceCopy(LLamaSeqId, LLamaSeqId, LLamaPos, LLamaPos)**

Copy all tokens that belong to the specified sequence to another sequence. Note that
 this does not allocate extra KV cache memory - it simply assigns the tokens to the
 new sequence

```csharp
public void KvCacheSequenceCopy(LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`src` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`dest` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **KvCacheSequenceKeep(LLamaSeqId)**

Removes all tokens that do not belong to the specified sequence

```csharp
public void KvCacheSequenceKeep(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **KvCacheSequenceAdd(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Adds relative position "delta" to all tokens that belong to the specified sequence
 and have positions in [p0, p1. If the KV cache is RoPEd, the KV data is updated
 accordingly

```csharp
public void KvCacheSequenceAdd(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`delta` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheSequenceDivide(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Integer division of the positions by factor of `d &gt; 1`.
 If the KV cache is RoPEd, the KV data is updated accordingly.<br>
 p0 &lt; 0 : [0, p1]<br>
 p1 &lt; 0 : [p0, inf)

```csharp
public void KvCacheSequenceDivide(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int divisor)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`divisor` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
