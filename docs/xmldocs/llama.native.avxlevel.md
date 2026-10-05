[`< Back`](./)

---

# AvxLevel

Namespace: LLama.Native

Avx support configuration

```csharp
public enum AvxLevel
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [AvxLevel](./llama.native.avxlevel.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | No AVX or SSE4.2. For x86_64 CPUs that only provide SSSE3 / SSE2. |
| Avx | 1 | Advanced Vector Extensions (supported by most processors after 2011) |
| Avx2 | 2 | AVX2 (supported by most processors after 2013) |
| Avx512 | 3 | AVX512 (supported by some processors after 2016, not widely supported) |

---

[`< Back`](./)
