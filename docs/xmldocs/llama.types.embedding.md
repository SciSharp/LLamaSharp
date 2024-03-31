# Embedding

Namespace: LLama.Types

```csharp
public class Embedding : System.IEquatable`1[[LLama.Types.Embedding, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Embedding](./llama.types.embedding.md)<br>
Implements [IEquatable&lt;Embedding&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Object**

```csharp
public string Object { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Model**

```csharp
public string Model { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Data**

```csharp
public EmbeddingData[] Data { get; set; }
```

#### Property Value

[EmbeddingData[]](./llama.types.embeddingdata.md)<br>

### **Usage**

```csharp
public EmbeddingUsage Usage { get; set; }
```

#### Property Value

[EmbeddingUsage](./llama.types.embeddingusage.md)<br>

## Constructors

### **Embedding(String, String, EmbeddingData[], EmbeddingUsage)**

```csharp
public Embedding(string Object, string Model, EmbeddingData[] Data, EmbeddingUsage Usage)
```

#### Parameters

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Data` [EmbeddingData[]](./llama.types.embeddingdata.md)<br>

`Usage` [EmbeddingUsage](./llama.types.embeddingusage.md)<br>

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

### **Equals(Embedding)**

```csharp
public bool Equals(Embedding other)
```

#### Parameters

`other` [Embedding](./llama.types.embedding.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public Embedding <Clone>$()
```

#### Returns

[Embedding](./llama.types.embedding.md)<br>

### **Deconstruct(String&, String&, EmbeddingData[]&, EmbeddingUsage&)**

```csharp
public void Deconstruct(String& Object, String& Model, EmbeddingData[]& Data, EmbeddingUsage& Usage)
```

#### Parameters

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Model` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Data` [EmbeddingData[]&](./llama.types.embeddingdata&.md)<br>

`Usage` [EmbeddingUsage&](./llama.types.embeddingusage&.md)<br>
