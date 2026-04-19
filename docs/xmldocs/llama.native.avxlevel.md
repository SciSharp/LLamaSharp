[`< Back`](./)

---

# AvxLevel

Namespace: LLama.Native

Avx support configuration

```csharp
public enum AvxLevel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [AvxLevel](./llama.native.avxlevel.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | No AVX |
| Avx | 1 | Advanced Vector Extensions (supported by most processors after 2011) |
| Avx2 | 2 | AVX2 (supported by most processors after 2013) |
| Avx512 | 3 | AVX512 (supported by some processors after 2016, not widely supported) |

---

[`< Back`](./)
