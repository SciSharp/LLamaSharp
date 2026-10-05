[`< Back`](./)

---

# Description

Namespace: LLama.Native

The description of the native library configurations that's already specified.

```csharp
internal record class Description : System.IEquatable`1[[LLama.Native.NativeLibraryConfig+Description, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Description](./llama.native.nativelibraryconfig.description.md)<br>
Implements [IEquatable&lt;Description&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Path**



```csharp
public string? Path { get; init; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Library**



```csharp
public NativeLibraryName Library { get; init; }
```

#### Property Value

[NativeLibraryName](./llama.native.nativelibraryname.md)<br>

### **UseCuda**



```csharp
public bool UseCuda { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseVulkan**



```csharp
public bool UseVulkan { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **AvxLevel**



```csharp
public AvxLevel AvxLevel { get; init; }
```

#### Property Value

[AvxLevel](./llama.native.avxlevel.md)<br>

### **AllowFallback**



```csharp
public bool AllowFallback { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SkipCheck**



```csharp
public bool SkipCheck { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SearchDirectories**



```csharp
public String[] SearchDirectories { get; init; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **Description(String, NativeLibraryName, Boolean, Boolean, AvxLevel, Boolean, Boolean, String[])**

The description of the native library configurations that's already specified.

```csharp
public Description(string? Path, NativeLibraryName Library, bool UseCuda, bool UseVulkan, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, String[] SearchDirectories)
```

#### Parameters

`Path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

`Library` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>

`UseCuda` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`UseVulkan` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`AvxLevel` [AvxLevel](./llama.native.avxlevel.md)<br>

`AllowFallback` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`SkipCheck` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`SearchDirectories` [String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Description(Description)**

```csharp
protected Description(Description original)
```

#### Parameters

`original` [Description](./llama.native.nativelibraryconfig.description.md)<br>

---

[`< Back`](./)
