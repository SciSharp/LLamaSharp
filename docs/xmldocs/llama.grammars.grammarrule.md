# GrammarRule

Namespace: LLama.Grammars

A single rule in a [Grammar](./llama.grammars.grammar.md)

```csharp
public sealed class GrammarRule : System.IEquatable`1[[LLama.Grammars.GrammarRule, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GrammarRule](./llama.grammars.grammarrule.md)<br>
Implements [IEquatable&lt;GrammarRule&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Name**

Name of this rule

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Elements**

The elements of this grammar rule

```csharp
public IReadOnlyList<LLamaGrammarElement> Elements { get; }
```

#### Property Value

[IReadOnlyList&lt;LLamaGrammarElement&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

## Constructors

### **GrammarRule(String, IReadOnlyList&lt;LLamaGrammarElement&gt;)**

Create a new GrammarRule containing the given elements

```csharp
public GrammarRule(string name, IReadOnlyList<LLamaGrammarElement> elements)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`elements` [IReadOnlyList&lt;LLamaGrammarElement&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(GrammarRule)**

```csharp
public bool Equals(GrammarRule other)
```

#### Parameters

`other` [GrammarRule](./llama.grammars.grammarrule.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public GrammarRule <Clone>$()
```

#### Returns

[GrammarRule](./llama.grammars.grammarrule.md)<br>
