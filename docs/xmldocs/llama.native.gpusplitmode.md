[`< Back`](./)

---

# GPUSplitMode

Namespace: LLama.Native



```csharp
public enum GPUSplitMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [GPUSplitMode](./llama.native.gpusplitmode.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_split_mode

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | Single GPU |
| Layer | 1 | Split layers and KV across GPUs |
| Row | 2 | split layers and KV across GPUs, use tensor parallelism if supported |
| Tensor | 3 |  |

---

[`< Back`](./)
