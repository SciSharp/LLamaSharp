[`< Back`](./)

---

# LLamaFtype

Namespace: LLama.Native

Supported model file types

```csharp
public enum LLamaFtype
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaFtype](./llama.native.llamaftype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

C# representation of llama_ftype

## Fields

| Name | Value | Description |
| --- | --: | --- |
| ALL_F32 | 0 | All f32 |
| MOSTLY_F16 | 1 | Mostly f16 |
| MOSTLY_Q8_0 | 7 | Mostly 8 bit |
| MOSTLY_Q4_0 | 2 | Mostly 4 bit |
| MOSTLY_Q4_1 | 3 | Mostly 4 bit |
| MOSTLY_Q5_0 | 8 | Mostly 5 bit |
| MOSTLY_Q5_1 | 9 | Mostly 5 bit |
| MOSTLY_Q2_K | 10 | K-Quant 2 bit |
| MOSTLY_Q3_K_S | 11 | K-Quant 3 bit (Small) |
| MOSTLY_Q3_K_M | 12 | K-Quant 3 bit (Medium) |
| MOSTLY_Q3_K_L | 13 | K-Quant 3 bit (Large) |
| MOSTLY_Q4_K_S | 14 | K-Quant 4 bit (Small) |
| MOSTLY_Q4_K_M | 15 | K-Quant 4 bit (Medium) |
| MOSTLY_Q5_K_S | 16 | K-Quant 5 bit (Small) |
| MOSTLY_Q5_K_M | 17 | K-Quant 5 bit (Medium) |
| MOSTLY_Q6_K | 18 | K-Quant 6 bit |
| MOSTLY_IQ2_XXS | 19 | except 1d tensors |
| MOSTLY_IQ2_XS | 20 | except 1d tensors |
| MOSTLY_Q2_K_S | 21 | except 1d tensors |
| MOSTLY_IQ3_K_XS | 22 | except 1d tensors |
| MOSTLY_IQ3_XXS | 23 | except 1d tensors |
| MOSTLY_IQ1_S | 24 | except 1d tensors |
| MOSTLY_IQ4_NL | 25 | except 1d tensors |
| MOSTLY_IQ3_S | 26 | except 1d tensors |
| MOSTLY_IQ3_M | 27 | except 1d tensors |
| MOSTLY_IQ2_S | 28 | except 1d tensors |
| MOSTLY_IQ2_M | 29 | except 1d tensors |
| MOSTLY_IQ4_XS | 30 | except 1d tensors |
| MOSTLY_IQ1_M | 31 | except 1d tensors |
| MOSTLY_BF16 | 32 | except 1d tensors |
| LLAMA_FTYPE_MOSTLY_TQ1_0 | 36 | except 1d tensors |
| LLAMA_FTYPE_MOSTLY_TQ2_0 | 37 | except 1d tensors |
| GUESSED | 1024 | File type was not specified |

---

[`< Back`](./)
