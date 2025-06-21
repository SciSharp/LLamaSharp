[`< Back`](./)

---

# ConversationExtensions

Namespace: LLama.Batched

Extension method for [Conversation](./llama.batched.conversation.md)

```csharp
public static class ConversationExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ConversationExtensions](./llama.batched.conversationextensions.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [ExtensionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **Sample(Conversation, SafeLLamaSamplerChainHandle, Int32)**

Sample a token from this conversation using the given sampler chain

```csharp
public static LLamaToken Sample(Conversation conversation, SafeLLamaSamplerChainHandle sampler, int offset)
```

#### Parameters

`conversation` [Conversation](./llama.batched.conversation.md)<br>
[Conversation](./llama.batched.conversation.md) to sample from

`sampler` [SafeLLamaSamplerChainHandle](./llama.native.safellamasamplerchainhandle.md)<br>

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Offset from the end of the conversation to the logits to sample, see [Conversation.GetSampleIndex(Int32)](./llama.batched.conversation.md#getsampleindexint32) for more details

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Sample(Conversation, ISamplingPipeline, Int32)**

Sample a token from this conversation using the given sampling pipeline

```csharp
public static LLamaToken Sample(Conversation conversation, ISamplingPipeline sampler, int offset)
```

#### Parameters

`conversation` [Conversation](./llama.batched.conversation.md)<br>
[Conversation](./llama.batched.conversation.md) to sample from

`sampler` [ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Offset from the end of the conversation to the logits to sample, see [Conversation.GetSampleIndex(Int32)](./llama.batched.conversation.md#getsampleindexint32) for more details

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Rewind(Conversation, Int32)**

Rewind a [Conversation](./llama.batched.conversation.md) back to an earlier state by removing tokens from the end

```csharp
public static void Rewind(Conversation conversation, int tokens)
```

#### Parameters

`conversation` [Conversation](./llama.batched.conversation.md)<br>
The conversation to rewind

`tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of tokens to rewind

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if `tokens` parameter is larger than TokenCount

### **ShiftLeft(Conversation, Int32, Int32)**

Shift all tokens over to the left, removing "count" tokens from the start and shifting everything over.
 Leaves "keep" tokens at the start completely untouched. This can be used to free up space when the context
 gets full, keeping the prompt at the start intact.

```csharp
public static void ShiftLeft(Conversation conversation, int count, int keep)
```

#### Parameters

`conversation` [Conversation](./llama.batched.conversation.md)<br>
The conversation to rewind

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
How much to shift tokens over by

`keep` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The number of tokens at the start which should not be shifted

---

[`< Back`](./)
