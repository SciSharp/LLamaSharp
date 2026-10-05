[`< Back`](./)

---

# LLamaVocabType

Namespace: LLama.Native



```csharp
public enum LLamaVocabType
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaVocabType](./llama.native.llamavocabtype.md)<br>
Implements [IComparable](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

llama_vocab_type

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | For models without vocab |
| SentencePiece | 1 | LLaMA tokenizer based on byte-level BPE with byte fallback |
| BytePairEncoding | 2 | GPT-2 tokenizer based on byte-level BPE |
| WordPiece | 3 | BERT tokenizer based on WordPiece |
| Unigram | 4 | T5 tokenizer based on Unigram |
| RWKV | 5 | RWKV tokenizer based on greedy tokenization |
| PLAMO2 | 6 | PLaMo-2 tokenizer based on Aho-Corasick with dynamic programming |

---

[`< Back`](./)
