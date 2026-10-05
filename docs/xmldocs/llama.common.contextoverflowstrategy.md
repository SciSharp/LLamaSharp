[`< Back`](./)

---

# ContextOverflowStrategy

Namespace: LLama.Common

Defines how the executor should behave when the context window fills up 
 on a model that does not support native memory shifting (e.g., 2D RoPE models).

```csharp
public enum ContextOverflowStrategy
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [ContextOverflowStrategy](./llama.common.contextoverflowstrategy.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| ThrowException | 0 | The engine will throw a ContextOverflowException.  Use this to manually manage context pruning in your application layer. (Equivalent to llama-cli's --no-context-shift). |
| TruncateAndReprefill | 1 | The engine will silently drop a percentage of the oldest tokens  (preserving the system prompt) and completely re-prefill the context. |

---

[`< Back`](./)
