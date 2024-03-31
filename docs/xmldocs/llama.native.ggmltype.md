# GGMLType

Namespace: LLama.Native

Possible GGML quantisation types

```csharp
public enum GGMLType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [GGMLType](./llama.native.ggmltype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| GGML_TYPE_F32 | 0 | Full 32 bit float |
| GGML_TYPE_F16 | 1 | 16 bit float |
| GGML_TYPE_Q4_0 | 2 | 4 bit float |
| GGML_TYPE_Q4_1 | 3 | 4 bit float |
| GGML_TYPE_Q5_0 | 6 | 5 bit float |
| GGML_TYPE_Q5_1 | 7 | 5 bit float |
| GGML_TYPE_Q8_0 | 8 | 8 bit float |
| GGML_TYPE_Q8_1 | 9 | 8 bit float |
| GGML_TYPE_Q2_K | 10 | "type-1" 2-bit quantization in super-blocks containing 16 blocks, each block having 16 weight. Block scales and mins are quantized with 4 bits. This ends up effectively using 2.5625 bits per weight (bpw) |
| GGML_TYPE_Q3_K | 11 | "type-0" 3-bit quantization in super-blocks containing 16 blocks, each block having 16 weights. Scales are quantized with 6 bits. This end up using 3.4375 bpw. |
| GGML_TYPE_Q4_K | 12 | "type-1" 4-bit quantization in super-blocks containing 8 blocks, each block having 32 weights. Scales and mins are quantized with 6 bits. This ends up using 4.5 bpw. |
| GGML_TYPE_Q5_K | 13 | "type-1" 5-bit quantization. Same super-block structure as GGML_TYPE_Q4_K resulting in 5.5 bpw |
| GGML_TYPE_Q6_K | 14 | "type-0" 6-bit quantization. Super-blocks with 16 blocks, each block having 16 weights. Scales are quantized with 8 bits. This ends up using 6.5625 bpw |
| GGML_TYPE_Q8_K | 15 | "type-0" 8-bit quantization. Only used for quantizing intermediate results. The difference to the existing Q8_0 is that the block size is 256. All 2-6 bit dot products are implemented for this quantization type. |
| GGML_TYPE_I8 | 16 | Integer, 8 bit |
| GGML_TYPE_I16 | 17 | Integer, 16 bit |
| GGML_TYPE_I32 | 18 | Integer, 32 bit |
| GGML_TYPE_COUNT | 19 | The value of this entry is the count of the number of possible quant types. |
