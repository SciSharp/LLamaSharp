[`< Back`](./)

---

# LLamaBatchEmbeddings

Namespace: LLama.Native

An embeddings batch allows submitting embeddings to multiple sequences simultaneously

```csharp
public class LLamaBatchEmbeddings
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EmbeddingDimensions**

Size of an individual embedding

```csharp
public int EmbeddingDimensions { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EmbeddingsCount**

The number of items in this batch

```csharp
public int EmbeddingsCount { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SequenceCapacity**

Maximum number of sequences an item can be assigned to (automatically grows if exceeded)

```csharp
public int SequenceCapacity { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **LLamaBatchEmbeddings(Int32)**

Create a new batch for submitting inputs to llama.cpp

```csharp
public LLamaBatchEmbeddings(int embeddingDimensions)
```

#### Parameters

`embeddingDimensions` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **Add(ReadOnlySpan&lt;Single&gt;, LLamaPos, ReadOnlySpan&lt;LLamaSeqId&gt;, Boolean)**

Add a single embedding to the batch at the same position in several sequences

```csharp
public int Add(ReadOnlySpan<float> embedding, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
```

#### Parameters

`embedding` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The embedding to add

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

### **Add(ReadOnlySpan&lt;Single&gt;, LLamaPos, LLamaSeqId, Boolean)**

Add a single embedding to the batch for a single sequence

```csharp
public int Add(ReadOnlySpan<float> embedding, LLamaPos pos, LLamaSeqId sequence, bool logits)
```

#### Parameters

`embedding` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

### **Add&lt;TParam&gt;(TParam, WriteEmbeddingsDelegate&lt;TParam&gt;, LLamaPos, ReadOnlySpan&lt;LLamaSeqId&gt;, Boolean)**

Add a single embedding to the batch at the same position in several sequences

```csharp
public int Add<TParam>(TParam parameter, WriteEmbeddingsDelegate<TParam> write, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
```

#### Type Parameters

`TParam`<br>
Type of userdata passed to write delegate

#### Parameters

`parameter` TParam<br>
Userdata passed to write delegate

`write` WriteEmbeddingsDelegate&lt;TParam&gt;<br>
Delegate called once to write data into a span

`pos` [LLamaPos](./llama.native.llamapos.md)<br>
Position to write this embedding to

`sequences` [ReadOnlySpan&lt;LLamaSeqId&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
All sequences to assign this embedding to

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether logits should be generated for this embedding

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

**Remarks:**

https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2

### **Add&lt;TParam&gt;(TParam, WriteEmbeddingsDelegate&lt;TParam&gt;, LLamaPos, LLamaSeqId, Boolean)**

Add a single embedding to the batch at a position for one sequence

```csharp
public int Add<TParam>(TParam parameter, WriteEmbeddingsDelegate<TParam> write, LLamaPos pos, LLamaSeqId sequence, bool logits)
```

#### Type Parameters

`TParam`<br>
Type of userdata passed to write delegate

#### Parameters

`parameter` TParam<br>
Userdata passed to write delegate

`write` WriteEmbeddingsDelegate&lt;TParam&gt;<br>
Delegate called once to write data into a span

`pos` [LLamaPos](./llama.native.llamapos.md)<br>
Position to write this embedding to

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
Sequence to assign this embedding to

`logits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether logits should be generated for this embedding

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index that the token was added at. Use this for GetLogitsIth

**Remarks:**

https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2

### **Clear()**

Set EmbeddingsCount to zero for this batch

```csharp
public void Clear()
```

---

[`< Back`](./)
