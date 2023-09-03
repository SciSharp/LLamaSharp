# Completion

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class Completion : System.IEquatable`1[[LLama.OldVersion.Completion, LLamaSharp, Version=0.5.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Completion](./llama.oldversion.completion.md)<br>
Implements [IEquatable&lt;Completion&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

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

### **Usage**

```csharp
public CompletionUsage Usage { get; set; }
```

#### Property Value

[CompletionUsage](./llama.oldversion.completionusage.md)<br>

## Constructors

### **Completion(String, String, Int32, String, CompletionChoice[], CompletionUsage)**

```csharp
public Completion(string Id, string Object, int Created, string Model, CompletionChoice[] Choices, CompletionUsage Usage)
```

#### Parameters

`Id` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Object` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Created` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Choices` [CompletionChoice[]](./llama.oldversion.completionchoice.md)<br>

`Usage` [CompletionUsage](./llama.oldversion.completionusage.md)<br>

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

### **Equals(Completion)**

```csharp
public bool Equals(Completion other)
```

#### Parameters

`other` [Completion](./llama.oldversion.completion.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public Completion <Clone>$()
```

#### Returns

[Completion](./llama.oldversion.completion.md)<br>

### **Deconstruct(String&, String&, Int32&, String&, CompletionChoice[]&, CompletionUsage&)**

```csharp
public void Deconstruct(String& Id, String& Object, Int32& Created, String& Model, CompletionChoice[]& Choices, CompletionUsage& Usage)
```

#### Parameters

`Id` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Object` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Created` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Model` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Choices` [CompletionChoice[]&](./llama.oldversion.completionchoice&.md)<br>

`Usage` [CompletionUsage&](./llama.oldversion.completionusage&.md)<br>
