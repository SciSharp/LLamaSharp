[`< Back`](./)

---

# StreamingTokenDecoder

Namespace: LLama

Decodes a stream of tokens into a stream of characters

```csharp
public sealed class StreamingTokenDecoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StreamingTokenDecoder](./llama.streamingtokendecoder.md)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **AvailableCharacters**

The number of decoded characters waiting to be read

```csharp
public int AvailableCharacters { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **DecodeSpecialTokens**

If true, special characters will be converted to text. If false they will be invisible.

```csharp
public bool DecodeSpecialTokens { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **StreamingTokenDecoder(Encoding, LLamaWeights)**

Create a new decoder

```csharp
public StreamingTokenDecoder(Encoding encoding, LLamaWeights weights)
```

#### Parameters

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Text encoding to use

`weights` [LLamaWeights](./llama.llamaweights.md)<br>
Model weights

### **StreamingTokenDecoder(LLamaContext)**

Create a new decoder

```csharp
public StreamingTokenDecoder(LLamaContext context)
```

#### Parameters

`context` [LLamaContext](./llama.llamacontext.md)<br>
Context to retrieve encoding and model weights from

### **StreamingTokenDecoder(Encoding, SafeLLamaContextHandle)**

Create a new decoder

```csharp
public StreamingTokenDecoder(Encoding encoding, SafeLLamaContextHandle context)
```

#### Parameters

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Text encoding to use

`context` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Context to retrieve model weights from

### **StreamingTokenDecoder(Encoding, SafeLlamaModelHandle)**

Create a new decoder

```csharp
public StreamingTokenDecoder(Encoding encoding, SafeLlamaModelHandle weights)
```

#### Parameters

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Text encoding to use

`weights` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>
Models weights to use

## Methods

### **Add(LLamaToken)**

Add a single token to the decoder

```csharp
public void Add(LLamaToken token)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

### **Add(Int32)**

Add a single token to the decoder

```csharp
public void Add(int token)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **AddRange&lt;T&gt;(T)**

Add all tokens in the given enumerable

```csharp
public void AddRange<T>(T tokens)
```

#### Type Parameters

`T`<br>

#### Parameters

`tokens` T<br>

### **AddRange(ReadOnlySpan&lt;LLamaToken&gt;)**

Add all tokens in the given span

```csharp
public void AddRange(ReadOnlySpan<LLamaToken> tokens)
```

#### Parameters

`tokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **Read(List&lt;Char&gt;)**

Read all decoded characters and clear the buffer

```csharp
public void Read(List<char> dest)
```

#### Parameters

`dest` [List&lt;Char&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Read()**

Read all decoded characters as a string and clear the buffer

```csharp
public string Read()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Reset()**

Set the decoder back to its initial state

```csharp
public void Reset()
```

---

[`< Back`](./)
