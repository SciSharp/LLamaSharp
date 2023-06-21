# ITextStreamTransform

Namespace: LLama.Abstractions

Takes a stream of tokens and transforms them.

```csharp
public interface ITextStreamTransform
```

## Methods

### **Transform(IEnumerable&lt;String&gt;)**

Takes a stream of tokens and transforms them, returning a new stream of tokens.

```csharp
IEnumerable<string> Transform(IEnumerable<string> tokens)
```

#### Parameters

`tokens` [IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **TransformAsync(IAsyncEnumerable&lt;String&gt;)**

Takes a stream of tokens and transforms them, returning a new stream of tokens asynchronously.

```csharp
IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens)
```

#### Parameters

`tokens` [IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
