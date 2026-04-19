[`< Back`](./)

---

# NativeLibraryWithCuda

Namespace: LLama.Native

A native library compiled with cublas/cuda.

```csharp
public class NativeLibraryWithCuda : LLama.Abstractions.INativeLibrary
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryWithCuda](./llama.native.nativelibrarywithcuda.md)<br>
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

### **NativeLibraryWithCuda(Int32, NativeLibraryName, AvxLevel, Boolean)**



```csharp
public NativeLibraryWithCuda(int majorCudaVersion, NativeLibraryName libraryName, AvxLevel avxLevel, bool skipCheck)
```

#### Parameters

`majorCudaVersion` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`libraryName` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

`avxLevel` [AvxLevel](./llama.native.avxlevel.md)<br>

`skipCheck` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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
