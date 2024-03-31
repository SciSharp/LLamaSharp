# LLamaBatch

Namespace: LLama.Native

A batch allows submitting multiple tokens to multiple sequences simultaneously

```csharp
public class LLamaBatch
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaBatch](./llama.native.llamabatch.md)

## Properties

### **TokenCount**

The number of tokens in this batch

```csharp
public int TokenCount { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SequenceCapacity**

Maximum number of sequences a token can be assigned to (automatically grows if exceeded)

```csharp
public int SequenceCapacity { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **LLamaBatch()**

Create a new batch for submitting inputs to llama.cpp

```csharp
public LLamaBatch()
```

## Methods

### **ToNativeBatch(LLamaNativeBatch&)**

```csharp
internal GroupDisposable ToNativeBatch(LLamaNativeBatch& batch)
```

#### Parameters

`batch` [LLamaNativeBatch&](./llama.native.llamanativebatch&.md)<br>

#### Returns

[GroupDisposable](./llama.native.groupdisposable.md)<br>

### **Add(LLamaToken, LLamaPos, ReadOnlySpan&lt;LLamaSeqId&gt;, Boolean)**

Add a single token to the batch at the same position in several sequences

```csharp
public int Add(LLamaToken token, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
The token to add

`pos` [LLamaPos](./llama.native.llamapos.md)<br>
The position to add it att

`sequences` [ReadOnlySpan&lt;LLamaSeqId&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The set of sequences to add this token to

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

**Remarks:**

https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2

### **Add(LLamaToken, LLamaPos, List&lt;LLamaSeqId&gt;, Boolean)**

Add a single token to the batch at the same position in several sequences

```csharp
public int Add(LLamaToken token, LLamaPos pos, List<LLamaSeqId> sequences, bool logits)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
The token to add

`pos` [LLamaPos](./llama.native.llamapos.md)<br>
The position to add it att

`sequences` [List&lt;LLamaSeqId&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
The set of sequences to add this token to

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

**Remarks:**

https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2

### **Add(LLamaToken, LLamaPos, LLamaSeqId, Boolean)**

Add a single token to the batch at a certain position for a single sequences

```csharp
public int Add(LLamaToken token, LLamaPos pos, LLamaSeqId sequence, bool logits)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
The token to add

`pos` [LLamaPos](./llama.native.llamapos.md)<br>
The position to add it att

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
The sequence to add this token to

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

**Remarks:**

https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2

### **AddRange(ReadOnlySpan&lt;LLamaToken&gt;, LLamaPos, LLamaSeqId, Boolean)**

Add a range of tokens to a single sequence, start at the given position.

```csharp
public int AddRange(ReadOnlySpan<LLamaToken> tokens, LLamaPos start, LLamaSeqId sequence, bool logitsLast)
```

#### Parameters

`tokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The tokens to add

`start` [LLamaPos](./llama.native.llamapos.md)<br>
The starting position to add tokens at

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
The sequence to add this token to

`logitsLast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the final token should generate logits

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the final token was added at. Use this for GetLogitsIth

### **Clear()**

Set TokenCount to zero for this batch

```csharp
public void Clear()
```

### **GetLogitPositions(Span&lt;ValueTuple&lt;LLamaSeqId, Int32&gt;&gt;)**

Get the positions where logits can be sampled from

```csharp
internal Span<ValueTuple<LLamaSeqId, int>> GetLogitPositions(Span<ValueTuple<LLamaSeqId, int>> dest)
```

#### Parameters

`dest` [Span&lt;ValueTuple&lt;LLamaSeqId, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Returns

[Span&lt;ValueTuple&lt;LLamaSeqId, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
