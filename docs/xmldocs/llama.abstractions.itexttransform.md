# ITextTransform

Namespace: LLama.Abstractions

An interface for text transformations.
 These can be used to compose a pipeline of text transformations, such as:
 - Tokenization
 - Lowercasing
 - Punctuation removal
 - Trimming
 - etc.

```csharp
public interface ITextTransform
```

## Methods

### **Transform(String)**

Takes a string and transforms it.

```csharp
string Transform(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Clone()**

Copy the transform.

```csharp
ITextTransform Clone()
```

#### Returns

[ITextTransform](./llama.abstractions.itexttransform.md)<br>
