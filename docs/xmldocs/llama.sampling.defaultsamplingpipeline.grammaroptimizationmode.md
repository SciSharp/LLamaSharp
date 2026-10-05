[`< Back`](./)

---

# GrammarOptimizationMode

Namespace: LLama.Sampling

Grammar Optimization Mode

```csharp
internal enum GrammarOptimizationMode
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [GrammarOptimizationMode](./llama.sampling.defaultsamplingpipeline.grammaroptimizationmode.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | No grammar optimization, slow because it has to apply the grammar to the entire vocab. |
| Basic | 1 | Attempts to return early by only applying the grammar to the selected token and checking if it's valid. |
| Extended | 2 | Attempts to return early by applying the grammar to the top K tokens and checking if the selected token is valid. |

---

[`< Back`](./)
