# LLamaGrammarElementType

Namespace: LLama.Native

grammar element type

```csharp
public enum LLamaGrammarElementType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaGrammarElementType](./llama.native.llamagrammarelementtype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| END | 0 | end of rule definition |
| ALT | 1 | start of alternate definition for rule |
| RULE_REF | 2 | non-terminal element: reference to rule |
| CHAR | 3 | terminal element: character (code point) |
| CHAR_NOT | 4 | inverse char(s) ([^a], [^a-b] [^abc]) |
| CHAR_RNG_UPPER | 5 | modifies a preceding CHAR or CHAR_ALT to be an inclusive range ([a-z]) |
| CHAR_ALT | 6 | modifies a preceding CHAR or CHAR_RNG_UPPER to add an alternate char to match ([ab], [a-zA]) |
