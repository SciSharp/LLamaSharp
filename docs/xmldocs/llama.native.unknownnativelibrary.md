[`< Back`](./)

---

# UnknownNativeLibrary

Namespace: LLama.Native

When you are using .NET standard2.0, dynamic native library loading is not supported.
 This class will be returned in [NativeLibraryConfig.DryRun(INativeLibrary&)](./llama.native.nativelibraryconfig.md#dryruninativelibrary&).

```csharp
public class UnknownNativeLibrary : LLama.Abstractions.INativeLibrary
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnknownNativeLibrary](./llama.native.unknownnativelibrary.md)<br>
Implements [INativeLibrary](./llama.abstractions.inativelibrary.md)

## Properties

### **Metadata**

```csharp
public NativeLibraryMetadata Metadata { get; }
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
