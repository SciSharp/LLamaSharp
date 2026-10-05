[`< Back`](./)

---

# DefaultNativeLibrarySelectingPolicy

Namespace: LLama.Native

Decides the selected native library that should be loaded according to the configurations.

```csharp
public class DefaultNativeLibrarySelectingPolicy : LLama.Abstractions.INativeLibrarySelectingPolicy
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultNativeLibrarySelectingPolicy](./llama.native.defaultnativelibraryselectingpolicy.md)<br>
Implements [INativeLibrarySelectingPolicy](./llama.abstractions.inativelibraryselectingpolicy.md)

## Constructors

### **DefaultNativeLibrarySelectingPolicy()**

```csharp
public DefaultNativeLibrarySelectingPolicy()
```

## Methods

### **Apply(Description, SystemInfo, LLamaLogCallback)**

Select the native library.

```csharp
public IEnumerable<INativeLibrary> Apply(Description description, SystemInfo systemInfo, LLamaLogCallback? logCallback)
```

#### Parameters

`description` [Description](./llama.native.nativelibraryconfig.description.md)<br>

`systemInfo` [SystemInfo](./llama.native.systeminfo.md)<br>
The system information of the current machine.

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)?<br>
The log callback.

#### Returns

[IEnumerable&lt;INativeLibrary&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
The information of the selected native library files, in order by priority from the beginning to the end.

---

[`< Back`](./)
