[`< Back`](./)

---

# SystemInfo

Namespace: LLama.Native

Operating system information.

```csharp
public record class SystemInfo : System.IEquatable`1[[LLama.Native.SystemInfo, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SystemInfo](./llama.native.systeminfo.md)<br>
Implements [IEquatable&lt;SystemInfo&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **OSPlatform**



```csharp
public OSPlatform OSPlatform { get; init; }
```

#### Property Value

[OSPlatform](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.osplatform)<br>

### **CudaMajorVersion**



```csharp
public int CudaMajorVersion { get; init; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **VulkanVersion**



```csharp
public string? VulkanVersion { get; init; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **SystemInfo(OSPlatform, Int32, String)**

Operating system information.

```csharp
public SystemInfo(OSPlatform OSPlatform, int CudaMajorVersion, string? VulkanVersion)
```

#### Parameters

`OSPlatform` [OSPlatform](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.osplatform)<br>

`CudaMajorVersion` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`VulkanVersion` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)?<br>

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

[PlatformNotSupportedException](https://learn.microsoft.com/en-us/dotnet/api/system.platformnotsupportedexception)<br>

---

[`< Back`](./)
