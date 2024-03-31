# IContextParamsExtensions

Namespace: LLama.Extensions

Extention methods to the IContextParams interface

```csharp
public static class IContextParamsExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [IContextParamsExtensions](./llama.extensions.icontextparamsextensions.md)

## Methods

### **ToLlamaContextParams(IContextParams, LLamaContextParams&)**

Convert the given `IModelParams` into a `LLamaContextParams`

```csharp
public static void ToLlamaContextParams(IContextParams params, LLamaContextParams& result)
```

#### Parameters

`params` [IContextParams](./llama.abstractions.icontextparams.md)<br>

`result` [LLamaContextParams&](./llama.native.llamacontextparams&.md)<br>

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
