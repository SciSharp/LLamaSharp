[`< Back`](./)

---

# LLamaExecutorExtensions

Namespace: LLama.Abstractions

Extension methods to the [LLamaExecutorExtensions](./llama.abstractions.llamaexecutorextensions.md) interface.

```csharp
public static class LLamaExecutorExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaExecutorExtensions](./llama.abstractions.llamaexecutorextensions.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [ExtensionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **AsChatClient(ILLamaExecutor, IHistoryTransform, ITextStreamTransform)**

Gets an  instance for the specified [ILLamaExecutor](./llama.abstractions.illamaexecutor.md).

```csharp
public static IChatClient AsChatClient(ILLamaExecutor executor, IHistoryTransform historyTransform, ITextStreamTransform outputTransform)
```

#### Parameters

`executor` [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)<br>
The executor.

`historyTransform` [IHistoryTransform](./llama.abstractions.ihistorytransform.md)<br>
The [IHistoryTransform](./llama.abstractions.ihistorytransform.md) to use to transform an input list messages into a prompt.

`outputTransform` [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>
The [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md) to use to transform the output into text.

#### Returns

IChatClient<br>
An  instance for the provided [ILLamaExecutor](./llama.abstractions.illamaexecutor.md).

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
`executor` is null.

---

[`< Back`](./)
