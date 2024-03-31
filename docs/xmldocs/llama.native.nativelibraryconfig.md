# NativeLibraryConfig

Namespace: LLama.Native

Allows configuration of the native llama.cpp libraries to load and use.
 All configuration must be done before using **any** other LLamaSharp methods!

```csharp
public sealed class NativeLibraryConfig
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryConfig](./llama.native.nativelibraryconfig.md)

## Properties

### **Instance**

Get the config instance

```csharp
public static NativeLibraryConfig Instance { get; }
```

#### Property Value

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **LibraryHasLoaded**

Check if the native library has already been loaded. Configuration cannot be modified if this is true.

```csharp
public static bool LibraryHasLoaded { get; internal set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **WithLibrary(String, String)**

Load a specified native library as backend for LLamaSharp.
 When this method is called, all the other configurations will be ignored.

```csharp
public NativeLibraryConfig WithLibrary(string llamaPath, string llavaPath)
```

#### Parameters

`llamaPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the llama library to load.

`llavaPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the llava library to load.

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithCuda(Boolean)**

Configure whether to use cuda backend if possible.

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

### **WithAvx(AvxLevel)**

Configure the prefferred avx support level of the backend.

```csharp
public NativeLibraryConfig WithAvx(AvxLevel level)
```

#### Parameters

`level` [AvxLevel](./llama.native.nativelibraryconfig.avxlevel.md)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithAutoFallback(Boolean)**

Configure whether to allow fallback when there's no match for preferred settings.

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
 you have your cublas configured but LLamaSharp take it as invalid by mistake.

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

### **WithLogs(Boolean)**

Whether to output the logs to console when loading the native library with your configuration.

```csharp
public NativeLibraryConfig WithLogs(bool enable)
```

#### Parameters

`enable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithLogs(LLamaLogLevel)**

Enable console logging with the specified log logLevel.

```csharp
public NativeLibraryConfig WithLogs(LLamaLogLevel logLevel)
```

#### Parameters

`logLevel` [LLamaLogLevel](./llama.native.llamaloglevel.md)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
Thrown if `LibraryHasLoaded` is true.

### **WithSearchDirectories(IEnumerable&lt;String&gt;)**

Add self-defined search directories. Note that the file stucture of the added 
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

Add self-defined search directories. Note that the file stucture of the added 
 directories must be the same as the default directory. Besides, the directory 
 won't be used recursively.

```csharp
public NativeLibraryConfig WithSearchDirectory(string directory)
```

#### Parameters

`directory` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[NativeLibraryConfig](./llama.native.nativelibraryconfig.md)<br>

### **CheckAndGatherDescription(LibraryName)**

```csharp
internal static Description CheckAndGatherDescription(LibraryName library)
```

#### Parameters

`library` [LibraryName](./llama.native.libraryname.md)<br>

#### Returns

[Description](./llama.native.nativelibraryconfig.description.md)<br>

### **AvxLevelToString(AvxLevel)**

```csharp
internal static string AvxLevelToString(AvxLevel level)
```

#### Parameters

`level` [AvxLevel](./llama.native.nativelibraryconfig.avxlevel.md)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
