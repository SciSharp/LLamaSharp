# EmbeddingData

Namespace: LLama.Types

```csharp
public class EmbeddingData : System.IEquatable`1[[LLama.Types.EmbeddingData, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [EmbeddingData](./llama.types.embeddingdata.md)<br>
Implements [IEquatable&lt;EmbeddingData&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Index**

```csharp
public int Index { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Object**

```csharp
public string Object { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Embedding**

```csharp
public Single[] Embedding { get; set; }
```

#### Property Value

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

## Constructors

### **EmbeddingData(Int32, String, Single[])**

```csharp
public EmbeddingData(int Index, string Object, Single[] Embedding)
```

#### Parameters

`Index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Embedding` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

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

### **Equals(EmbeddingData)**

```csharp
public bool Equals(EmbeddingData other)
```

#### Parameters

`other` [EmbeddingData](./llama.types.embeddingdata.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public EmbeddingData <Clone>$()
```

#### Returns

[EmbeddingData](./llama.types.embeddingdata.md)<br>

### **Deconstruct(Int32&, String&, Single[]&)**

```csharp
public void Deconstruct(Int32& Index, String& Object, Single[]& Embedding)
```

#### Parameters

`Index` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Embedding` [Single[]&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
