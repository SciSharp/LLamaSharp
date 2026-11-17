[`< Back`](./)

---

# LLamaLazyMode

Namespace: LLama.Native

Controls on-demand reading of model tensors when memory mapping is enabled.

```csharp
public enum LLamaLazyMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaLazyMode](./llama.native.llamalazymode.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_lazy_mode

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Off | 0 | Read each tensor in full when loading the model. |
| Auto | 1 | Read marked tensors larger than 4 GiB on demand. |
| On | 2 | Read rows of marked tensors on demand. |

---

[`< Back`](./)
