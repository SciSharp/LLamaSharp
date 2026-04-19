[`< Back`](./)

---

# DefaultNativeLibrarySelectingPolicy

Namespace: LLama.Native

```csharp
public class DefaultNativeLibrarySelectingPolicy : LLama.Abstractions.INativeLibrarySelectingPolicy
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DefaultNativeLibrarySelectingPolicy](./llama.native.defaultnativelibraryselectingpolicy.md)<br>
Implements [INativeLibrarySelectingPolicy](./llama.abstractions.inativelibraryselectingpolicy.md)

## Constructors

### **DefaultNativeLibrarySelectingPolicy()**

```csharp
public DefaultNativeLibrarySelectingPolicy()
```

## Methods

### **Apply(Description, SystemInfo, LLamaLogCallback)**

```csharp
public IEnumerable<INativeLibrary> Apply(Description description, SystemInfo systemInfo, LLamaLogCallback logCallback)
```

#### Parameters

`description` [Description](./llama.native.nativelibraryconfig.description.md)<br>

`systemInfo` [SystemInfo](./llama.native.systeminfo.md)<br>

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

#### Returns

[IEnumerable&lt;INativeLibrary&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

---

[`< Back`](./)
