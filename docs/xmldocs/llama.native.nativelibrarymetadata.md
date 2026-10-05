[`< Back`](./)

---

# NativeLibraryMetadata

Namespace: LLama.Native

Information of a native library file.

```csharp
public record class NativeLibraryMetadata : System.IEquatable`1[[LLama.Native.NativeLibraryMetadata, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>
Implements [IEquatable&lt;NativeLibraryMetadata&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **NativeLibraryName**

Which kind of library it is.

```csharp
public NativeLibraryName NativeLibraryName { get; init; }
```

#### Property Value

[NativeLibraryName](./llama.native.nativelibraryname.md)<br>

### **UseCuda**

Whether it's compiled with cublas.

```csharp
public bool UseCuda { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseVulkan**

Whether it's compiled with vulkan.

```csharp
public bool UseVulkan { get; init; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **AvxLevel**

Which AvxLevel it's compiled with.

```csharp
public AvxLevel AvxLevel { get; init; }
```

#### Property Value

[AvxLevel](./llama.native.avxlevel.md)<br>

## Constructors

### **NativeLibraryMetadata(NativeLibraryName, Boolean, Boolean, AvxLevel)**

Information of a native library file.

```csharp
public NativeLibraryMetadata(NativeLibraryName NativeLibraryName, bool UseCuda, bool UseVulkan, AvxLevel AvxLevel)
```

#### Parameters

`NativeLibraryName` [NativeLibraryName](./llama.native.nativelibraryname.md)<br>
Which kind of library it is.

`UseCuda` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether it's compiled with cublas.

`UseVulkan` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether it's compiled with vulkan.

`AvxLevel` [AvxLevel](./llama.native.avxlevel.md)<br>
Which AvxLevel it's compiled with.

### **NativeLibraryMetadata(NativeLibraryMetadata)**

```csharp
protected NativeLibraryMetadata(NativeLibraryMetadata original)
```

#### Parameters

`original` [NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

---

[`< Back`](./)
