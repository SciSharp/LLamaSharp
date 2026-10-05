[`< Back`](./)

---

# KeywordTextOutputStreamTransform

Namespace: LLama

A text output transform that removes the keywords from the response.

```csharp
internal class KeywordTextOutputStreamTransform : LLama.Abstractions.ITextStreamTransform
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [KeywordTextOutputStreamTransform](./llama.llamatransforms.keywordtextoutputstreamtransform.md)<br>
Implements [ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **Keywords**

Keywords that you want to remove from the response.
 This property is used for JSON serialization.

```csharp
public HashSet<string> Keywords { get; }
```

#### Property Value

[HashSet&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br>

### **MaxKeywordLength**

Maximum length of the keywords.
 This property is used for JSON serialization.

```csharp
public int MaxKeywordLength { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **RemoveAllMatchedTokens**

If set to true, when getting a matched keyword, all the related tokens will be removed. 
 Otherwise only the part of keyword will be removed.
 This property is used for JSON serialization.

```csharp
public bool RemoveAllMatchedTokens { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **KeywordTextOutputStreamTransform(HashSet&lt;String&gt;, Int32, Boolean)**

JSON constructor.

```csharp
public KeywordTextOutputStreamTransform(HashSet<string> keywords, int maxKeywordLength, bool removeAllMatchedTokens)
```

#### Parameters

`keywords` [HashSet&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1)<br>

`maxKeywordLength` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`removeAllMatchedTokens` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **KeywordTextOutputStreamTransform(IEnumerable&lt;String&gt;, Int32, Boolean)**



```csharp
public KeywordTextOutputStreamTransform(IEnumerable<string> keywords, int redundancyLength = 3, bool removeAllMatchedTokens = false)
```

#### Parameters

`keywords` [IEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
Keywords that you want to remove from the response.

`redundancyLength` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The extra length when searching for the keyword. For example, if your only keyword is "highlight", 
 maybe the token you get is "\r\nhighligt". In this condition, if redundancyLength=0, the token cannot be successfully matched because the length of "\r\nhighligt" (10)
 has already exceeded the maximum length of the keywords (8). On the contrary, setting redundancyLengyh &gt;= 2 leads to successful match.
 The larger the redundancyLength is, the lower the processing speed. But as an experience, it won't introduce too much performance impact when redundancyLength &lt;= 5

`removeAllMatchedTokens` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If set to true, when getting a matched keyword, all the related tokens will be removed. Otherwise only the part of keyword will be removed.

## Methods

### **Clone()**

Copy the transform.

```csharp
public ITextStreamTransform Clone()
```

#### Returns

[ITextStreamTransform](./llama.abstractions.itextstreamtransform.md)<br>

### **TransformAsync(IAsyncEnumerable&lt;String&gt;)**

Takes a stream of tokens and transforms them, returning a new stream of tokens asynchronously.

```csharp
public IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens)
```

#### Parameters

`tokens` [IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

---

[`< Back`](./)
