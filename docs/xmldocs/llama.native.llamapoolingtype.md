[`< Back`](./)

---

# LLamaPoolingType

Namespace: LLama.Native



```csharp
public enum LLamaPoolingType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaPoolingType](./llama.native.llamapoolingtype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_pooling_type

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Unspecified | -1 | No specific pooling type. Use the model default if this is specific in [IContextParams.PoolingType](./llama.abstractions.icontextparams.md#poolingtype) |
| None | 0 | Do not pool embeddings (per-token embeddings) |
| Mean | 1 | Take the mean of every token embedding |
| CLS | 2 | Return the embedding for the special "CLS" token |
| Last | 3 | Return the embeddings of the last token |
| Rank | 4 | Used by reranking models to attach the classification head to the graph |

---

[`< Back`](./)
