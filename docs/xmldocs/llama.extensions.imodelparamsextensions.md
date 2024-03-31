# IModelParamsExtensions

Namespace: LLama.Extensions

Extention methods to the IModelParams interface

```csharp
public static class IModelParamsExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [IModelParamsExtensions](./llama.extensions.imodelparamsextensions.md)

## Methods

### **ToLlamaModelParams(IModelParams, LLamaModelParams&)**

Convert the given `IModelParams` into a `LLamaModelParams`

```csharp
public static IDisposable ToLlamaModelParams(IModelParams params, LLamaModelParams& result)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

`result` [LLamaModelParams&](./llama.native.llamamodelparams&.md)<br>

#### Returns

[IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
