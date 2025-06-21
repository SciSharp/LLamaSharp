[`< Back`](./)

---

# SpanNormalizationExtensions

Namespace: LLama.Extensions

Extensions to span which apply in-place normalization

```csharp
public static class SpanNormalizationExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SpanNormalizationExtensions](./llama.extensions.spannormalizationextensions.md)<br>
Attributes [ExtensionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

## Methods

### **MaxAbsoluteNormalization(Single[])**

In-place multiple every element by 32760 and divide every element in the span by the max absolute value in the span

```csharp
public static Single[] MaxAbsoluteNormalization(Single[] vector)
```

#### Parameters

`vector` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The same array

### **MaxAbsoluteNormalization(Span&lt;Single&gt;)**

In-place multiple every element by 32760 and divide every element in the span by the max absolute value in the span

```csharp
public static Span<float> MaxAbsoluteNormalization(Span<float> vector)
```

#### Parameters

`vector` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The same span

### **TaxicabNormalization(Single[])**

In-place divide every element in the array by the sum of absolute values in the array

```csharp
public static Single[] TaxicabNormalization(Single[] vector)
```

#### Parameters

`vector` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The same array

**Remarks:**

Also known as "Manhattan normalization".

### **TaxicabNormalization(Span&lt;Single&gt;)**

In-place divide every element in the span by the sum of absolute values in the span

```csharp
public static Span<float> TaxicabNormalization(Span<float> vector)
```

#### Parameters

`vector` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The same span

**Remarks:**

Also known as "Manhattan normalization".

### **EuclideanNormalization(Single[])**

In-place divide every element by the euclidean length of the vector

```csharp
public static Single[] EuclideanNormalization(Single[] vector)
```

#### Parameters

`vector` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The same array

**Remarks:**

Also known as "L2 normalization".

### **EuclideanNormalization(Span&lt;Single&gt;)**

In-place divide every element by the euclidean length of the vector

```csharp
public static Span<float> EuclideanNormalization(Span<float> vector)
```

#### Parameters

`vector` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The same span

**Remarks:**

Also known as "L2 normalization".

### **EuclideanNormalization(ReadOnlySpan&lt;Single&gt;)**

Creates a new array containing an L2 normalization of the input vector.

```csharp
public static Single[] EuclideanNormalization(ReadOnlySpan<float> vector)
```

#### Parameters

`vector` [ReadOnlySpan&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The same span

### **PNormalization(Single[], Int32)**

In-place apply p-normalization. https://en.wikipedia.org/wiki/Norm_(mathematics)#p-norm

- 
- 
-

```csharp
public static Single[] PNormalization(Single[] vector, int p)
```

#### Parameters

`vector` [Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`p` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
The same array

### **PNormalization(Span&lt;Single&gt;, Int32)**

In-place apply p-normalization. https://en.wikipedia.org/wiki/Norm_(mathematics)#p-norm

- 
- 
-

```csharp
public static Span<float> PNormalization(Span<float> vector, int p)
```

#### Parameters

`vector` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`p` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
The same span

---

[`< Back`](./)
