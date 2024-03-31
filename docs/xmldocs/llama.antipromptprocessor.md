# AntipromptProcessor

Namespace: LLama

AntipromptProcessor keeps track of past tokens looking for any set Anti-Prompts

```csharp
public sealed class AntipromptProcessor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AntipromptProcessor](./llama.antipromptprocessor.md)

## Constructors

### **AntipromptProcessor(IEnumerable&lt;String&gt;)**

Initializes a new instance of the [AntipromptProcessor](./llama.antipromptprocessor.md) class.

```csharp
public AntipromptProcessor(IEnumerable<string> antiprompts)
```

#### Parameters

`antiprompts` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
The antiprompts.

## Methods

### **AddAntiprompt(String)**

Add an antiprompt to the collection

```csharp
public void AddAntiprompt(string antiprompt)
```

#### Parameters

`antiprompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SetAntiprompts(IEnumerable&lt;String&gt;)**

Overwrite all current antiprompts with a new set

```csharp
public void SetAntiprompts(IEnumerable<string> antiprompts)
```

#### Parameters

`antiprompts` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Add(String)**

Add some text and check if the buffer now ends with any antiprompt

```csharp
public bool Add(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the text buffer ends with any antiprompt
