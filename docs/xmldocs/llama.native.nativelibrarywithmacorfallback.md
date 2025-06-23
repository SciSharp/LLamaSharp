[`< Back`](./)

---

# NativeLibraryWithMacOrFallback

Namespace: LLama.Native

A native library compiled on Mac, or fallbacks from all other libraries in the selection.

```csharp
public class NativeLibraryWithMacOrFallback : LLama.Abstractions.INativeLibrary
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryWithMacOrFallback](./llama.native.nativelibrarywithmacorfallback.md)<br>
Implements [INativeLibrary](./llama.abstractions.inativelibrary.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Metadata**

```csharp
public NativeLibraryMetadata Metadata { get; }
```

#### Property Value

[NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

## Constructors

### **NativeLibraryWithMacOrFallback(NativeLibraryName)**



```csharp
public NativeLibraryWithMacOrFallback(NativeLibraryName libraryName)
```

#### Parameters

`libraryName` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

## Methods

### **Prepare(SystemInfo, LLamaLogCallback)**

```csharp
public IEnumerable<string> Prepare(SystemInfo systemInfo, LLamaLogCallback logCallback)
```

#### Parameters

`systemInfo` [SystemInfo](./llama.native.systeminfo.md)<br>

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

---

[`< Back`](./)
