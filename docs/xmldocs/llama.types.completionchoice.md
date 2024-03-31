# CompletionChoice

Namespace: LLama.Types

```csharp
public class CompletionChoice : System.IEquatable`1[[LLama.Types.CompletionChoice, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CompletionChoice](./llama.types.completionchoice.md)<br>
Implements [IEquatable&lt;CompletionChoice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Text**

```csharp
public string Text { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Index**

```csharp
public int Index { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Logprobs**

```csharp
public CompletionLogprobs Logprobs { get; set; }
```

#### Property Value

[CompletionLogprobs](./llama.types.completionlogprobs.md)<br>

### **FinishReason**

```csharp
public string FinishReason { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **CompletionChoice(String, Int32, CompletionLogprobs, String)**

```csharp
public CompletionChoice(string Text, int Index, CompletionLogprobs Logprobs, string FinishReason)
```

#### Parameters

`Text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`Index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`Logprobs` [CompletionLogprobs](./llama.types.completionlogprobs.md)<br>

`FinishReason` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

### **Equals(CompletionChoice)**

```csharp
public bool Equals(CompletionChoice other)
```

#### Parameters

`other` [CompletionChoice](./llama.types.completionchoice.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public CompletionChoice <Clone>$()
```

#### Returns

[CompletionChoice](./llama.types.completionchoice.md)<br>

### **Deconstruct(String&, Int32&, CompletionLogprobs&, String&)**

```csharp
public void Deconstruct(String& Text, Int32& Index, CompletionLogprobs& Logprobs, String& FinishReason)
```

#### Parameters

`Text` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Index` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`Logprobs` [CompletionLogprobs&](./llama.types.completionlogprobs&.md)<br>

`FinishReason` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
