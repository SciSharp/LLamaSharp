[`< Back`](./)

---

# INativeLibrarySelectingPolicy

Namespace: LLama.Abstractions

Decides the selected native library that should be loaded according to the configurations.

```csharp
public interface INativeLibrarySelectingPolicy
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Methods

### **Apply(Description, SystemInfo, LLamaLogCallback)**

Select the native library.

```csharp
IEnumerable<INativeLibrary> Apply(Description description, SystemInfo systemInfo, LLamaLogCallback logCallback)
```

#### Parameters

`description` [Description](./llama.native.nativelibraryconfig.description.md)<br>

`systemInfo` [SystemInfo](./llama.native.systeminfo.md)<br>
The system information of the current machine.

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>
The log callback.

#### Returns

[IEnumerable&lt;INativeLibrary&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
The information of the selected native library files, in order by priority from the beginning to the end.

---

[`< Back`](./)
