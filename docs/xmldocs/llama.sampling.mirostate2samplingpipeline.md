# Mirostate2SamplingPipeline

Namespace: LLama.Sampling

A sampling pipeline which uses mirostat (v2) to select tokens

```csharp
public class Mirostate2SamplingPipeline : BaseSamplingPipeline, ISamplingPipeline, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BaseSamplingPipeline](./llama.sampling.basesamplingpipeline.md) → [Mirostate2SamplingPipeline](./llama.sampling.mirostate2samplingpipeline.md)<br>
Implements [ISamplingPipeline](./llama.sampling.isamplingpipeline.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Mu**

Currently learned mu value

```csharp
public float Mu { get; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Tau**

target entropy

```csharp
public float Tau { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Eta**

learning rate

```csharp
public float Eta { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Grammar**

Grammar to constrain valid tokens

```csharp
public SafeLLamaGrammarHandle Grammar { get; set; }
```

#### Property Value

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

## Constructors

### **Mirostate2SamplingPipeline()**

```csharp
public Mirostate2SamplingPipeline()
```

## Methods

### **ProcessLogits(SafeLLamaContextHandle, Span&lt;Single&gt;, ReadOnlySpan&lt;LLamaToken&gt;)**

```csharp
protected void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`logits` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

### **ProcessTokenDataArray(SafeLLamaContextHandle, LLamaTokenDataArray, ReadOnlySpan&lt;LLamaToken&gt;)**

```csharp
protected LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`candidates` [LLamaTokenDataArray](./llama.native.llamatokendataarray.md)<br>

`lastTokens` [ReadOnlySpan&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[LLamaToken](./llama.native.llamatoken.md)<br>

### **Reset()**

```csharp
public void Reset()
```

### **Clone()**

```csharp
public ISamplingPipeline Clone()
```

#### Returns

[ISamplingPipeline](./llama.sampling.isamplingpipeline.md)<br>
