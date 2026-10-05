[`< Back`](./)

---

# EmptyTextOutputStreamTransform

Namespace: LLama

A no-op text input transform.

```csharp
internal class EmptyTextOutputStreamTransform : LLama.Abstractions.ITextStreamTransform
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EmptyTextOutputStreamTransform](./llama.llamatransforms.emptytextoutputstreamtransform.md)<br>
Implements [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)

## Constructors

### **EmptyTextOutputStreamTransform()**

```csharp
public EmptyTextOutputStreamTransform()
```

## Methods

### **TransformAsync(IAsyncEnumerable&lt;String&gt;)**

Takes a stream of tokens and transforms them, returning a new stream of tokens asynchronously.

```csharp
public IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens)
```

#### Parameters

`tokens` [IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

### **Clone()**

Copy the transform.

```csharp
public ITextStreamTransform Clone()
```

#### Returns

[ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

---

[`< Back`](./)
