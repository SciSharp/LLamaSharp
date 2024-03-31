# LLamaBeamView

Namespace: LLama.Native

Information about a single beam in a beam search

```csharp
public struct LLamaBeamView
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaBeamView](./llama.native.llamabeamview.md)

## Fields

### **CumulativeProbability**

Cumulative beam probability (renormalized relative to all beams)

```csharp
public float CumulativeProbability;
```

### **EndOfBeam**

Callback should set this to true when a beam is at end-of-beam.

```csharp
public bool EndOfBeam;
```

## Properties

### **Tokens**

Tokens in this beam

```csharp
public Span<LLamaToken> Tokens { get; }
```

#### Property Value

[Span&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
