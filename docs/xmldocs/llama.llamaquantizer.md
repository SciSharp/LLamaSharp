# LLamaQuantizer

Namespace: LLama

The quantizer to quantize the model.

```csharp
public static class LLamaQuantizer
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaQuantizer](./llama.llamaquantizer.md)

## Methods

### **Quantize(String, String, LLamaFtype, Int32, Boolean, Boolean)**

Quantize the model.

```csharp
public static bool Quantize(string srcFileName, string dstFilename, LLamaFtype ftype, int nthread, bool allowRequantize, bool quantizeOutputTensor)
```

#### Parameters

`srcFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model file to be quantized.

`dstFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The path to save the quantized model.

`ftype` [LLamaFtype](./llama.native.llamaftype.md)<br>
The type of quantization.

`nthread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Thread to be used during the quantization. By default it's the physical core number.

`allowRequantize` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`quantizeOutputTensor` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the quantization is successful.

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **Quantize(String, String, String, Int32, Boolean, Boolean)**

Quantize the model.

```csharp
public static bool Quantize(string srcFileName, string dstFilename, string ftype, int nthread, bool allowRequantize, bool quantizeOutputTensor)
```

#### Parameters

`srcFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model file to be quantized.

`dstFilename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The path to save the quantized model.

`ftype` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The type of quantization.

`nthread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Thread to be used during the quantization. By default it's the physical core number.

`allowRequantize` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`quantizeOutputTensor` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the quantization is successful.

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
