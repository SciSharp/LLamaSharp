# IModelParamsExtensions

Namespace: LLama.Extensions

Extention methods to the IModelParams interface

```csharp
public static class IModelParamsExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [IModelParamsExtensions](./llama.extensions.imodelparamsextensions.md)

## Methods

### **ToLlamaContextParams(IModelParams, LLamaContextParams&)**

Convert the given `IModelParams` into a `LLamaContextParams`

```csharp
public static MemoryHandle ToLlamaContextParams(IModelParams params, LLamaContextParams& result)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

`result` [LLamaContextParams&](./llama.native.llamacontextparams&.md)<br>

#### Returns

[MemoryHandle](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.memoryhandle)<br>

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
