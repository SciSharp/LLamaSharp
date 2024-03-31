# Quantizer

Namespace: LLama

```csharp
public class Quantizer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Quantizer](./llama.quantizer.md)

## Constructors

### **Quantizer()**

```csharp
public Quantizer()
```

## Methods

### **Quantize(String, String, LLamaFtype, Int32, Boolean)**

```csharp
public static bool Quantize(string srcFileName, string dstFilename, LLamaFtype ftype, int nthread, bool printInfo)
```

#### Parameters

`srcFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`dstFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ftype` [LLamaFtype](./llama.native.llamaftype.md)<br>

`nthread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`printInfo` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Quantize(String, String, String, Int32, Boolean)**

```csharp
public static bool Quantize(string srcFileName, string dstFilename, string ftype, int nthread, bool printInfo)
```

#### Parameters

`srcFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`dstFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`ftype` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`nthread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`printInfo` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
