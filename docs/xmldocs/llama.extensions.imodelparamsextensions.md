[`< Back`](./)

---

# IModelParamsExtensions

Namespace: LLama.Extensions

Extension methods to the IModelParams interface

```csharp
public static class IModelParamsExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IModelParamsExtensions](./llama.extensions.imodelparamsextensions.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [ExtensionAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **ToLlamaModelParams(IModelParams, out LLamaModelParams)**

Convert the given `IModelParams` into a `LLamaModelParams`

```csharp
public static IDisposable ToLlamaModelParams(IModelParams params, out LLamaModelParams result)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

`out` `result` [LLamaModelParams](./llama.native.llamamodelparams.md)<br>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>

#### Exceptions

[FileNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

[UnknownDeviceException](./llama.exceptions.unknowndeviceexception.md)<br>
Thrown if a name in [IModelParams.Devices](./llama.abstractions.imodelparams.md#devices) does not match any available device

---

[`< Back`](./)
