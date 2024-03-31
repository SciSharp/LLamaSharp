# StreamingTokenDecoder

Namespace: LLama

Decodes a stream of tokens into a stream of characters

```csharp
public sealed class StreamingTokenDecoder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StreamingTokenDecoder](./llama.streamingtokendecoder.md)

## Properties

### **AvailableCharacters**

The number of decoded characters waiting to be read

```csharp
public int AvailableCharacters { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **&lt;Add&gt;g__TokenToBytes|9_0(Byte[]&, LLamaToken, SafeLlamaModelHandle)**

```csharp
internal static Span<byte> <Add>g__TokenToBytes|9_0(Byte[]& bytes, LLamaToken token, SafeLlamaModelHandle model)
```

#### Parameters

`bytes` [Byte[]&](https://docs.microsoft.com/en-us/dotnet/api/system.byte&)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

#### Returns

[Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
