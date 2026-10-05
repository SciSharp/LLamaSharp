[`< Back`](./)

---

# NaiveTextInputTransform

Namespace: LLama

A text input transform that only trims the text.

```csharp
internal class NaiveTextInputTransform : LLama.Abstractions.ITextTransform
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NaiveTextInputTransform](./llama.llamatransforms.naivetextinputtransform.md)<br>
Implements [ITextTransform](./llama.abstractions.itexttransform.md)

## Constructors

### **NaiveTextInputTransform()**

```csharp
public NaiveTextInputTransform()
```

## Methods

### **Transform(String)**

Takes a string and transforms it.

```csharp
public string Transform(string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Clone()**

Copy the transform.

```csharp
public ITextTransform Clone()
```

#### Returns

[ITextTransform](./llama.abstractions.itexttransform.md)<br>

---

[`< Back`](./)
