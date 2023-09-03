# MirostatType

Namespace: LLama.Common

Type of "mirostat" sampling to use.
 https://github.com/basusourya/mirostat

```csharp
public enum MirostatType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [MirostatType](./llama.common.mirostattype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Disable | 0 | Disable Mirostat sampling |
| Mirostat | 1 | Original mirostat algorithm |
| Mirostat2 | 2 | Mirostat 2.0 algorithm |
