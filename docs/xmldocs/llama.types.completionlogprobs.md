# CompletionLogprobs

Namespace: LLama.Types

```csharp
public class CompletionLogprobs : System.IEquatable`1[[LLama.Types.CompletionLogprobs, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CompletionLogprobs](./llama.types.completionlogprobs.md)<br>
Implements [IEquatable&lt;CompletionLogprobs&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **TextOffset**

```csharp
public Int32[] TextOffset { get; set; }
```

#### Property Value

[Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TokenLogProbs**

```csharp
public Single[] TokenLogProbs { get; set; }
```

#### Property Value

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Tokens**

```csharp
public String[] Tokens { get; set; }
```

#### Property Value

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TopLogprobs**

```csharp
public Dictionary`2[] TopLogprobs { get; set; }
```

#### Property Value

[Dictionary`2[]](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

## Constructors

### **CompletionLogprobs(Int32[], Single[], String[], Dictionary`2[])**

```csharp
public CompletionLogprobs(Int32[] TextOffset, Single[] TokenLogProbs, String[] Tokens, Dictionary`2[] TopLogprobs)
```

#### Parameters

`TextOffset` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`TokenLogProbs` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`Tokens` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`TopLogprobs` [Dictionary`2[]](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

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

### **Equals(CompletionLogprobs)**

```csharp
public bool Equals(CompletionLogprobs other)
```

#### Parameters

`other` [CompletionLogprobs](./llama.types.completionlogprobs.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public CompletionLogprobs <Clone>$()
```

#### Returns

[CompletionLogprobs](./llama.types.completionlogprobs.md)<br>

### **Deconstruct(Int32[]&, Single[]&, String[]&, Dictionary`2[]&)**

```csharp
public void Deconstruct(Int32[]& TextOffset, Single[]& TokenLogProbs, String[]& Tokens, Dictionary`2[]& TopLogprobs)
```

#### Parameters

`TextOffset` [Int32[]&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`TokenLogProbs` [Single[]&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>

`Tokens` [String[]&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`TopLogprobs` [Dictionary`2[]&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2&)<br>
