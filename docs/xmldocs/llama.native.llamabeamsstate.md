# LLamaBeamsState

Namespace: LLama.Native

Passed to beam_search_callback function.
 Whenever 0 &lt; common_prefix_length, this number of tokens should be copied from any of the beams
 (e.g. beams[0]) as they will be removed (shifted) from all beams in all subsequent callbacks.

```csharp
public struct LLamaBeamsState
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaBeamsState](./llama.native.llamabeamsstate.md)

## Fields

### **CommonPrefixLength**

Current max length of prefix tokens shared by all beams.

```csharp
public ulong CommonPrefixLength;
```

### **LastCall**

True iff this is the last callback invocation.

```csharp
public bool LastCall;
```

## Properties

### **Beams**

The current state of each beam

```csharp
public Span<LLamaBeamView> Beams { get; }
```

#### Property Value

[Span&lt;LLamaBeamView&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
