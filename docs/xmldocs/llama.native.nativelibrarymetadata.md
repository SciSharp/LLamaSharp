[`< Back`](./)

---

# NativeLibraryMetadata

Namespace: LLama.Native

Information of a native library file.

```csharp
public class NativeLibraryMetadata : System.IEquatable`1[[LLama.Native.NativeLibraryMetadata, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>
Implements [IEquatable&lt;NativeLibraryMetadata&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EqualityContract**

```csharp
protected Type EqualityContract { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **NativeLibraryName**

Which kind of library it is.

```csharp
public NativeLibraryName NativeLibraryName { get; set; }
```

#### Property Value

[NativeLibraryName](./llama.native.nativelibraryname.md)<br>

### **UseCuda**

Whether it's compiled with cublas.

```csharp
public bool UseCuda { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseVulkan**

Whether it's compiled with vulkan.

```csharp
public bool UseVulkan { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **AvxLevel**

Which AvxLevel it's compiled with.

```csharp
public AvxLevel AvxLevel { get; set; }
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

`UseCuda` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether it's compiled with cublas.

`UseVulkan` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether it's compiled with vulkan.

`AvxLevel` [AvxLevel](./llama.native.avxlevel.md)<br>
Which AvxLevel it's compiled with.

### **NativeLibraryMetadata(NativeLibraryMetadata)**

```csharp
protected NativeLibraryMetadata(NativeLibraryMetadata original)
```

#### Parameters

`original` [NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(NativeLibraryMetadata)**

```csharp
public bool Equals(NativeLibraryMetadata other)
```

#### Parameters

`other` [NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public NativeLibraryMetadata <Clone>$()
```

#### Returns

[NativeLibraryMetadata](./llama.native.nativelibrarymetadata.md)<br>

### **Deconstruct(NativeLibraryName&, Boolean&, Boolean&, AvxLevel&)**

```csharp
public void Deconstruct(NativeLibraryName& NativeLibraryName, Boolean& UseCuda, Boolean& UseVulkan, AvxLevel& AvxLevel)
```

#### Parameters

`NativeLibraryName` [NativeLibraryName&](./llama.native.nativelibraryname&.md)<br>

`UseCuda` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>

`UseVulkan` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>

`AvxLevel` [AvxLevel&](./llama.native.avxlevel&.md)<br>

---

[`< Back`](./)
