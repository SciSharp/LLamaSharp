[`< Back`](./)

---

# NativeLibraryConfigContainer

Namespace: LLama.Native

A class to set same configurations to multiple libraries at the same time.

```csharp
public sealed class NativeLibraryConfigContainer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Methods

### **ForEach(Action&lt;NativeLibraryConfig&gt;)**

Do an action for all the configs in this container.

```csharp
public void ForEach(Action<NativeLibraryConfig> action)
```

#### Parameters

`action` [Action&lt;NativeLibraryConfig&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>

### **WithLibrary(String, String)**

Load a specified native library as backend for LLamaSharp.
 When this method is called, all the other configurations will be ignored.

```csharp
public NativeLibraryConfigContainer WithLibrary(string llamaPath, string llavaPath)
```

#### Parameters

`llamaPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the llama library to load.

`llavaPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the llava library to load.

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithCuda(Boolean)**

Configure whether to use cuda backend if possible.

```csharp
public NativeLibraryConfigContainer WithCuda(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithVulkan(Boolean)**

Configure whether to use vulkan backend if possible.

```csharp
public NativeLibraryConfigContainer WithVulkan(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithAvx(AvxLevel)**

Configure the prefferred avx support level of the backend.

```csharp
public NativeLibraryConfigContainer WithAvx(AvxLevel level)
```

#### Parameters

`level` [AvxLevel](./llama.native.avxlevel.md)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithAutoFallback(Boolean)**

Configure whether to allow fallback when there's no match for preferred settings.

```csharp
public NativeLibraryConfigContainer WithAutoFallback(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **SkipCheck(Boolean)**

Whether to skip the check when you don't allow fallback. This option 
 may be useful under some complex conditions. For example, you're sure 
 you have your cublas configured but LLamaSharp take it as invalid by mistake.

```csharp
public NativeLibraryConfigContainer SkipCheck(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithSearchDirectories(IEnumerable&lt;String&gt;)**

Add self-defined search directories. Note that the file structure of the added 
 directories must be the same as the default directory. Besides, the directory 
 won't be used recursively.

```csharp
public NativeLibraryConfigContainer WithSearchDirectories(IEnumerable<string> directories)
```

#### Parameters

`directories` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

### **WithSearchDirectory(String)**

Add self-defined search directories. Note that the file structure of the added 
 directories must be the same as the default directory. Besides, the directory 
 won't be used recursively.

```csharp
public NativeLibraryConfigContainer WithSearchDirectory(string directory)
```

#### Parameters

`directory` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

### **WithSelectingPolicy(INativeLibrarySelectingPolicy)**

Set the policy which decides how to select the desired native libraries and order them by priority. 
 By default we use [DefaultNativeLibrarySelectingPolicy](./llama.native.defaultnativelibraryselectingpolicy.md).

```csharp
public NativeLibraryConfigContainer WithSelectingPolicy(INativeLibrarySelectingPolicy policy)
```

#### Parameters

`policy` [INativeLibrarySelectingPolicy](./llama.abstractions.inativelibraryselectingpolicy.md)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

### **WithLogCallback(LLamaLogCallback)**

Set the log callback that will be used for all llama.cpp log messages

```csharp
public NativeLibraryConfigContainer WithLogCallback(LLamaLogCallback callback)
```

#### Parameters

`callback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **WithLogCallback(ILogger)**

Set the log callback that will be used for all llama.cpp log messages

```csharp
public NativeLibraryConfigContainer WithLogCallback(ILogger logger)
```

#### Parameters

`logger` ILogger<br>

#### Returns

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **DryRun(INativeLibrary&, INativeLibrary&)**

Try to load the native library with the current configurations, 
 but do not actually set it to [NativeApi](./llama.native.nativeapi.md).
 
 You can still modify the configuration after this calling but only before any call from [NativeApi](./llama.native.nativeapi.md).

```csharp
public bool DryRun(INativeLibrary& loadedLLamaNativeLibrary, INativeLibrary& loadedLLavaNativeLibrary)
```

#### Parameters

`loadedLLamaNativeLibrary` [INativeLibrary&](./llama.abstractions.inativelibrary&.md)<br>

`loadedLLavaNativeLibrary` [INativeLibrary&](./llama.abstractions.inativelibrary&.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the running is successful.

---

[`< Back`](./)
