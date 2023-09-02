# CompletionChunk

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class CompletionChunk : System.IEquatable`1[[LLama.OldVersion.CompletionChunk, LLamaSharp, Version=0.5.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CompletionChunk](./llama.oldversion.completionchunk.md)<br>
Implements [IEquatable&lt;CompletionChunk&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Id**

```csharp
public string Id { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Object**

```csharp
public string Object { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Created**

```csharp
public int Created { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Model**

```csharp
public string Model { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Choices**

```csharp
public CompletionChoice[] Choices { get; set; }
```

#### Property Value

[CompletionChoice[]](./llama.oldversion.completionchoice.md)<br>

## Constructors

### **CompletionChunk(String, String, Int32, String, CompletionChoice[])**

```csharp
public CompletionChunk(string Id, string Object, int Created, string Model, CompletionChoice[] Choices)
```

#### Parameters

`Id` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Created` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Choices` [CompletionChoice[]](./llama.oldversion.completionchoice.md)<br>

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

### **Equals(CompletionChunk)**

```csharp
public bool Equals(CompletionChunk other)
```

#### Parameters

`other` [CompletionChunk](./llama.oldversion.completionchunk.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public CompletionChunk <Clone>$()
```

#### Returns

[CompletionChunk](./llama.oldversion.completionchunk.md)<br>

### **Deconstruct(String&, String&, Int32&, String&, CompletionChoice[]&)**

```csharp
public void Deconstruct(String& Id, String& Object, Int32& Created, String& Model, CompletionChoice[]& Choices)
```

#### Parameters

`Id` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Created` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Model` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Choices` [CompletionChoice[]&](./llama.oldversion.completionchoice&.md)<br>
