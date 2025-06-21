[`< Back`](./)

---

# NativeLibraryConfig

Namespace: LLama.Native

Allows configuration of the native llama.cpp libraries to load and use.
 All configuration must be done before using **any** other LLamaSharp methods!

```csharp
public sealed class NativeLibraryConfig
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Instance**

#### Caution

Please use NativeLibraryConfig.All instead, or set configurations for NativeLibraryConfig.LLama and NativeLibraryConfig.LLavaShared respectively.

---

Set configurations for all the native libraries, including LLama and LLava

```csharp
public static NativeLibraryConfigContainer Instance { get; }
```

#### Property Value

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

### **All**

Set configurations for all the native libraries, including LLama and LLava

```csharp
public static NativeLibraryConfigContainer All { get; }
```

#### Property Value

[NativeLibraryConfigContainer](./llama.native.nativelibraryconfigcontainer.md)<br>

### **LLama**

Configuration for LLama native library

```csharp
public static NativeLibraryConfig LLama { get; }
```

#### Property Value

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **LLava**

Configuration for LLava native library

```csharp
public static NativeLibraryConfig LLava { get; }
```

#### Property Value

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **LibraryHasLoaded**

Check if the native library has already been loaded. Configuration cannot be modified if this is true.

```csharp
public bool LibraryHasLoaded { get; internal set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **WithLibrary(String)**

Load a specified native library as backend for LLamaSharp.
 When this method is called, all the other configurations will be ignored.

```csharp
public NativeLibraryConfig WithLibrary(string libraryPath)
```

#### Parameters

`libraryPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the native library to load.

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithCuda(Boolean)**

Configure whether to use cuda backend if possible. Default is true.

```csharp
public NativeLibraryConfig WithCuda(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithVulkan(Boolean)**

Configure whether to use vulkan backend if possible. Default is true.

```csharp
public NativeLibraryConfig WithVulkan(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithAvx(AvxLevel)**

Configure the prefferred avx support level of the backend. 
 Default value is detected automatically due to your operating system.

```csharp
public NativeLibraryConfig WithAvx(AvxLevel level)
```

#### Parameters

`level` [AvxLevel](./llama.native.avxlevel.md)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithAutoFallback(Boolean)**

Configure whether to allow fallback when there's no match for preferred settings. Default is true.

```csharp
public NativeLibraryConfig WithAutoFallback(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **SkipCheck(Boolean)**

Whether to skip the check when you don't allow fallback. This option 
 may be useful under some complex conditions. For example, you're sure 
 you have your cublas configured but LLamaSharp take it as invalid by mistake. Default is false;

```csharp
public NativeLibraryConfig SkipCheck(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithSearchDirectories(IEnumerable&lt;String&gt;)**

Add self-defined search directories. Note that the file structure of the added 
 directories must be the same as the default directory. Besides, the directory 
 won't be used recursively.

```csharp
public NativeLibraryConfig WithSearchDirectories(IEnumerable<string> directories)
```

#### Parameters

`directories` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **WithSearchDirectory(String)**

Add self-defined search directories. Note that the file structure of the added 
 directories must be the same as the default directory. Besides, the directory 
 won't be used recursively.

```csharp
public NativeLibraryConfig WithSearchDirectory(string directory)
```

#### Parameters

`directory` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **WithSelectingPolicy(INativeLibrarySelectingPolicy)**

Set the policy which decides how to select the desired native libraries and order them by priority. 
 By default we use [DefaultNativeLibrarySelectingPolicy](./llama.native.defaultnativelibraryselectingpolicy.md).

```csharp
public NativeLibraryConfig WithSelectingPolicy(INativeLibrarySelectingPolicy policy)
```

#### Parameters

`policy` [INativeLibrarySelectingPolicy](./llama.abstractions.inativelibraryselectingpolicy.md)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **WithLogCallback(LLamaLogCallback)**

Set the log callback that will be used for all llama.cpp log messages

```csharp
public NativeLibraryConfig WithLogCallback(LLamaLogCallback callback)
```

#### Parameters

`callback` [LLamaLogCallback](./llama.native.nativelogconfig.llamalogcallback.md)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **WithLogCallback(ILogger)**

Set the log callback that will be used for all llama.cpp log messages

```csharp
public NativeLibraryConfig WithLogCallback(ILogger logger)
```

#### Parameters

`logger` ILogger<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **DryRun(INativeLibrary&)**

Try to load the native library with the current configurations, 
 but do not actually set it to [NativeApi](./llama.native.nativeapi.md).
 
 You can still modify the configuration after this calling but only before any call from [NativeApi](./llama.native.nativeapi.md).

```csharp
public bool DryRun(INativeLibrary& loadedLibrary)
```

#### Parameters

`loadedLibrary` [INativeLibrary&](./llama.abstractions.inativelibrary&.md)<br>
The loaded livrary. When the loading failed, this will be null. 
 However if you are using .NET standard2.0, this will never return null.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the running is successful.

---

[`< Back`](./)
