[`< Back`](./)

---

# SystemInfo

Namespace: LLama.Native

Operating system information.

```csharp
public class SystemInfo : System.IEquatable`1[[LLama.Native.SystemInfo, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SystemInfo](./llama.native.systeminfo.md)<br>
Implements [IEquatable&lt;SystemInfo&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EqualityContract**

```csharp
protected Type EqualityContract { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **OSPlatform**



```csharp
public OSPlatform OSPlatform { get; set; }
```

#### Property Value

[OSPlatform](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.osplatform)<br>

### **CudaMajorVersion**



```csharp
public int CudaMajorVersion { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **VulkanVersion**



```csharp
public string VulkanVersion { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **SystemInfo(OSPlatform, Int32, String)**

Operating system information.

```csharp
public SystemInfo(OSPlatform OSPlatform, int CudaMajorVersion, string VulkanVersion)
```

#### Parameters

`OSPlatform` [OSPlatform](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.osplatform)<br>

`CudaMajorVersion` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`VulkanVersion` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SystemInfo(SystemInfo)**

```csharp
protected SystemInfo(SystemInfo original)
```

#### Parameters

`original` [SystemInfo](./llama.native.systeminfo.md)<br>

## Methods

### **Get()**

Get the system information of the current machine.

```csharp
public static SystemInfo Get()
```

#### Returns

[SystemInfo](./llama.native.systeminfo.md)<br>

#### Exceptions

[PlatformNotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.platformnotsupportedexception)<br>

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

### **Equals(SystemInfo)**

```csharp
public bool Equals(SystemInfo other)
```

#### Parameters

`other` [SystemInfo](./llama.native.systeminfo.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public SystemInfo <Clone>$()
```

#### Returns

[SystemInfo](./llama.native.systeminfo.md)<br>

### **Deconstruct(OSPlatform&, Int32&, String&)**

```csharp
public void Deconstruct(OSPlatform& OSPlatform, Int32& CudaMajorVersion, String& VulkanVersion)
```

#### Parameters

`OSPlatform` [OSPlatform&](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.osplatform&)<br>

`CudaMajorVersion` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`VulkanVersion` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

---

[`< Back`](./)
