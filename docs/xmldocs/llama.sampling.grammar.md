[`< Back`](./)

---

# Grammar

Namespace: LLama.Sampling

A grammar in GBNF form

```csharp
public record class Grammar : System.IEquatable`1[[LLama.Sampling.Grammar, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Grammar](./llama.sampling.grammar.md)<br>
Implements [IEquatable&lt;Grammar&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Gbnf**



```csharp
public string Gbnf { get; init; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Root**



```csharp
public string Root { get; init; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **Grammar(String, String)**

A grammar in GBNF form

```csharp
public Grammar(string Gbnf, string Root)
```

#### Parameters

`Gbnf` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`Root` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Grammar(Grammar)**

```csharp
protected Grammar(Grammar original)
```

#### Parameters

`original` [Grammar](./llama.sampling.grammar.md)<br>

---

[`< Back`](./)
