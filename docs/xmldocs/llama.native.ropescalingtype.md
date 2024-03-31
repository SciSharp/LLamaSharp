# RopeScalingType

Namespace: LLama.Native

RoPE scaling type.

```csharp
public enum RopeScalingType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [RopeScalingType](./llama.native.ropescalingtype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

C# equivalent of llama_rope_scaling_type

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Unspecified | -1 | No particular scaling type has been specified |
| None | 0 | Do not apply any RoPE scaling |
| Linear | 1 | Positional linear interpolation, as described by kaikendev: https://kaiokendev.github.io/til#extending-context-to-8k |
| Yarn | 2 | YaRN scaling: https://arxiv.org/pdf/2309.00071.pdf |
