[`< Back`](./)

---

# NativeLibraryWithAvx

Namespace: LLama.Native

A native library compiled with avx support but without cuda/cublas.

```csharp
public class NativeLibraryWithAvx : LLama.Abstractions.INativeLibrary
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryWithAvx](./llama.native.nativelibrarywithavx.md)<br>
Implements [INativeLibrary](./llama.abstractions.inativelibrary.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Metadata**

Metadata of this library.

```csharp
public NativeLibraryMetadata? Metadata { get; }
```

#### Property Value

[NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

## Constructors

### **NativeLibraryWithAvx(NativeLibraryName, AvxLevel, Boolean)**



```csharp
public NativeLibraryWithAvx(NativeLibraryName libraryName, AvxLevel avxLevel, bool skipCheck)
```

#### Parameters

`libraryName` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

`avxLevel` [AvxLevel](./llama.native.avxlevel.md)<br>

`skipCheck` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Prepare(SystemInfo, LLamaLogCallback)**

Prepare the native library file and returns the local path of it.
 If it's a relative path, LLamaSharp will search the path in the search directies you set.

```csharp
public IEnumerable<string> Prepare(SystemInfo systemInfo, LLamaLogCallback? logCallback)
```

#### Parameters

`systemInfo` [SystemInfo](./llama.native.systeminfo.md)<br>
The system information of the current machine.

`logCallback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)?<br>
The log callback.

#### Returns

[IEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
The relative paths of the library. You could return multiple paths to try them one by one. If no file is available, please return an empty array.

---

[`< Back`](./)
