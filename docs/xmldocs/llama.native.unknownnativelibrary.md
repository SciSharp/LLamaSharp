[`< Back`](./)

---

# UnknownNativeLibrary

Namespace: LLama.Native

When you are using .NET standard2.0, dynamic native library loading is not supported.
 This class will be returned in [NativeLibraryConfig.DryRun(out INativeLibrary)](./llama.native.nativelibraryconfig.md#dryrunout-inativelibrary).

```csharp
public class UnknownNativeLibrary : LLama.Abstractions.INativeLibrary
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [UnknownNativeLibrary](./llama.native.unknownnativelibrary.md)<br>
Implements [INativeLibrary](./llama.abstractions.inativelibrary.md)

## Properties

### **Metadata**

Metadata of this library.

```csharp
public NativeLibraryMetadata? Metadata { get; }
```

#### Property Value

[NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

## Constructors

### **UnknownNativeLibrary()**

```csharp
public UnknownNativeLibrary()
```

## Methods

### **Prepare(SystemInfo, LLamaLogCallback)**

Prepare the native library file and returns the local path of it.
 If it's a relative path, LLamaSharp will search the path in the search directies you set.

```csharp
public IEnumerable<string> Prepare(SystemInfo systemInfo, LLamaLogCallback? logCallback = null)
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
