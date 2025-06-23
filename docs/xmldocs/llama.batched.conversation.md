[`< Back`](./)

---

# Conversation

Namespace: LLama.Batched

A single conversation thread that can be prompted (adding tokens from the user) or inferred (extracting a token from the LLM)

```csharp
public sealed class Conversation : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Conversation](./llama.batched.conversation.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Executor**

The executor which this conversation belongs to

```csharp
public BatchedExecutor Executor { get; }
```

#### Property Value

[BatchedExecutor](./llama.batched.batchedexecutor.md)<br>

### **ConversationId**

Unique ID for this conversation

```csharp
public LLamaSeqId ConversationId { get; }
```

#### Property Value

[LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **TokenCount**

Total number of tokens in this conversation, cannot exceed the context length.

```csharp
public int TokenCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsDisposed**

Indicates if this conversation has been disposed, nothing can be done with a disposed conversation

```csharp
public bool IsDisposed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **RequiresInference**

Indicates if this conversation is waiting for inference to be run on the executor. "Prompt" and "Sample" cannot be called when this is true.

```csharp
public bool RequiresInference { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **RequiresSampling**

Indicates that this conversation should be sampled.

```csharp
public bool RequiresSampling { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Finalize()**

Finalizer for Conversation

```csharp
protected void Finalize()
```

### **Dispose()**

End this conversation, freeing all resources used by it

```csharp
public void Dispose()
```

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

### **Fork()**

Create a copy of the current conversation

```csharp
public Conversation Fork()
```

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

**Remarks:**

The copy shares internal state, so consumes very little extra memory.

### **GetSampleIndex(Int32)**

Get the index in the context which each token can be sampled from, the return value of this function get be used to retrieve logits
 ([SafeLLamaContextHandle.GetLogitsIth(Int32)](./llama.native.safellamacontexthandle.md#getlogitsithint32)) or to sample a token ([SafeLLamaSamplerChainHandle.Sample(SafeLLamaContextHandle, Int32)](./llama.native.safellamasamplerchainhandle.md#samplesafellamacontexthandle-int32).

```csharp
public int GetSampleIndex(int offset)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
How far from the end of the previous prompt should logits be sampled. Any value other than 0 requires
 allLogits to have been set during prompting.<br>
 For example if 5 tokens were supplied in the last prompt call:

- 
- 
- 
- 
-

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

[CannotSampleRequiresPromptException](./llama.batched.cannotsamplerequirespromptexception.md)<br>
Thrown if this conversation was not prompted before the previous call to infer

[CannotSampleRequiresInferenceException](./llama.batched.cannotsamplerequiresinferenceexception.md)<br>
Thrown if Infer() must be called on the executor

### **Sample(Int32)**

Get the logits from this conversation, ready for sampling

```csharp
public Span<float> Sample(int offset)
```

#### Parameters

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
How far from the end of the previous prompt should logits be sampled. Any value other than 0 requires allLogits to have been set during prompting

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

[CannotSampleRequiresPromptException](./llama.batched.cannotsamplerequirespromptexception.md)<br>
Thrown if this conversation was not prompted before the previous call to infer

[CannotSampleRequiresInferenceException](./llama.batched.cannotsamplerequiresinferenceexception.md)<br>
Thrown if Infer() must be called on the executor

### **Prompt(List&lt;LLamaToken&gt;, Boolean)**

Add tokens to this conversation

```csharp
public void Prompt(List<LLamaToken> tokens, bool allLogits)
```

#### Parameters

`tokens` [List&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`allLogits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, generate logits for all tokens. If false, only generate logits for the last token.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

[AlreadyPromptedConversationException](./llama.batched.alreadypromptedconversationexception.md)<br>

### **Prompt(ReadOnlySpan&lt;LLamaToken&gt;, Boolean)**

Add tokens to this conversation

```csharp
public void Prompt(ReadOnlySpan<LLamaToken> tokens, bool allLogits)
```

#### Parameters

`tokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`allLogits` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, generate logits for all tokens. If false, only generate logits for the last token.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

[AlreadyPromptedConversationException](./llama.batched.alreadypromptedconversationexception.md)<br>

### **Prompt(LLamaToken)**

Add a single token to this conversation

```csharp
public void Prompt(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

[AlreadyPromptedConversationException](./llama.batched.alreadypromptedconversationexception.md)<br>

### **Prompt(SafeLlavaImageEmbedHandle)**

Prompt this conversation with an image embedding

```csharp
public void Prompt(SafeLlavaImageEmbedHandle embedding)
```

#### Parameters

`embedding` [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>

### **Prompt(ReadOnlySpan&lt;Single&gt;)**

Prompt this conversation with embeddings

```csharp
public void Prompt(ReadOnlySpan<float> embeddings)
```

#### Parameters

`embeddings` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
The raw values of the embeddings. This span must divide equally by the embedding size of this model.

### **Modify(ModifyKvCache)**

Directly modify the KV cache of this conversation

```csharp
public void Modify(ModifyKvCache modifier)
```

#### Parameters

`modifier` [ModifyKvCache](./llama.batched.conversation.modifykvcache.md)<br>

#### Exceptions

[CannotModifyWhileRequiresInferenceException](./llama.batched.cannotmodifywhilerequiresinferenceexception.md)<br>
Thrown if this method is called while [Conversation.RequiresInference](./llama.batched.conversation.md#requiresinference) == true

### **Save(String)**

Save the complete state of this conversation to a file. if the file already exists it will be overwritten.

```csharp
public void Save(string filepath)
```

#### Parameters

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[CannotSaveWhileRequiresInferenceException](./llama.batched.cannotsavewhilerequiresinferenceexception.md)<br>

### **Save()**

Save the complete state of this conversation in system memory.

```csharp
public State Save()
```

#### Returns

[State](./llama.batched.conversation.state.md)<br>

---

[`< Back`](./)
