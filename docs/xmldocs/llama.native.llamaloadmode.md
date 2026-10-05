[`< Back`](./)

---

# LLamaLoadMode

Namespace: LLama.Native



```csharp
public enum LLamaLoadMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaLoadMode](./llama.native.llamaloadmode.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_load_mode

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | no special loading mode |
| MemoryMap | 1 | memory map the model |
| MemoryLock | 2 | force system to keep model in RAM rather than swapping or compressing |
| MemoryMapAndLock | 3 | mmap + force system to keep model in RAM rather than swapping or compressing |
| DirectIO | 4 | Use direct I/O if available |

---

[`< Back`](./)
