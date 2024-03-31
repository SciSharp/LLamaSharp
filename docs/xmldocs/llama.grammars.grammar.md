# Grammar

Namespace: LLama.Grammars

A grammar is a set of [GrammarRule](./llama.grammars.grammarrule.md)s for deciding which characters are valid next. Can be used to constrain
 output to certain formats - e.g. force the model to output JSON

```csharp
public sealed class Grammar
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Grammar](./llama.grammars.grammar.md)

## Properties

### **StartRuleIndex**

Index of the initial rule to start from

```csharp
public ulong StartRuleIndex { get; }
```

#### Property Value

[UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>

### **Rules**

The rules which make up this grammar

```csharp
public IReadOnlyList<GrammarRule> Rules { get; }
```

#### Property Value

[IReadOnlyList&lt;GrammarRule&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

## Constructors

### **Grammar(IReadOnlyList&lt;GrammarRule&gt;, UInt64)**

Create a new grammar from a set of rules

```csharp
public Grammar(IReadOnlyList<GrammarRule> rules, ulong startRuleIndex)
```

#### Parameters

`rules` [IReadOnlyList&lt;GrammarRule&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>
The rules which make up this grammar

`startRuleIndex` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
Index of the initial rule to start from

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>

## Methods

### **CreateInstance()**

Create a `SafeLLamaGrammarHandle` instance to use for parsing

```csharp
public SafeLLamaGrammarHandle CreateInstance()
```

#### Returns

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **Parse(String, String)**

Parse a string of GGML BNF into a Grammar

```csharp
public static Grammar Parse(string gbnf, string startRule)
```

#### Parameters

`gbnf` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The string to parse

`startRule` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the start rule of this grammar

#### Returns

[Grammar](./llama.grammars.grammar.md)<br>
A Grammar which can be converted into a SafeLLamaGrammarHandle for sampling

#### Exceptions

[GrammarFormatException](./llama.exceptions.grammarformatexception.md)<br>
Thrown if input is malformed

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
