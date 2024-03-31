# LLamaTokenType

Namespace: LLama.Native

Token Types

```csharp
public enum LLamaTokenType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaTokenType](./llama.native.llamatokentype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

C# equivalent of llama_token_get_type

## Fields

| Name | Value | Description |
| --- | --: | --- |
| LLAMA_TOKEN_TYPE_UNDEFINED | 0 | No specific type has been set for this token |
| LLAMA_TOKEN_TYPE_NORMAL | 1 | This is a "normal" token |
| LLAMA_TOKEN_TYPE_UNKNOWN | 2 | An "unknown" character/text token e.g. &lt;unk&gt; |
| LLAMA_TOKEN_TYPE_CONTROL | 3 | A special control token e.g. &lt;/s&gt; |
