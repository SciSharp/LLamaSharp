[`< Back`](./)

---

# Grammar

Namespace: LLama.Sampling

A grammar in GBNF form

```csharp
public class Grammar : System.IEquatable`1[[LLama.Sampling.Grammar, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Grammar](./llama.sampling.grammar.md)<br>
Implements [IEquatable&lt;Grammar&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EqualityContract**

```csharp
protected Type EqualityContract { get; }
```

#### Property Value

[Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

### **Gbnf**



```csharp
public string Gbnf { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Root**



```csharp
public string Root { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **Grammar(String, String)**

A grammar in GBNF form

```csharp
public Grammar(string Gbnf, string Root)
```

#### Parameters

`Gbnf` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Root` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Grammar(Grammar)**

```csharp
protected Grammar(Grammar original)
```

#### Parameters

`original` [Grammar](./llama.sampling.grammar.md)<br>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **Equals(Grammar)**

```csharp
public bool Equals(Grammar other)
```

#### Parameters

`other` [Grammar](./llama.sampling.grammar.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public Grammar <Clone>$()
```

#### Returns

[Grammar](./llama.sampling.grammar.md)<br>

### **Deconstruct(String&, String&)**

```csharp
public void Deconstruct(String& Gbnf, String& Root)
```

#### Parameters

`Gbnf` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Root` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

---

[`< Back`](./)
