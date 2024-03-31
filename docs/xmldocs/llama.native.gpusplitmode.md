# GPUSplitMode

Namespace: LLama.Native



```csharp
public enum GPUSplitMode
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [GPUSplitMode](./llama.native.gpusplitmode.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_split_mode

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | Single GPU |
| Layer | 1 | Split layers and KV across GPUs |
| Row | 2 | split rows across GPUs |
