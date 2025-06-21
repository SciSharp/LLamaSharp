[`< Back`](./)

---

# LLamaAttentionType

Namespace: LLama.Native



```csharp
public enum LLamaAttentionType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaAttentionType](./llama.native.llamaattentiontype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_attention_type

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Unspecified | -1 | Unspecified attention type. The library will attempt to find the best fit |
| Causal | 0 | The causal mask will be applied, causing tokens to only see previous tokens in the same sequence, and not future ones |
| NonCausal | 1 | The causal mask will not be applied, and tokens of the same sequence will be able to see each other |

---

[`< Back`](./)
