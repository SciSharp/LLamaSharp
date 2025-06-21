[`< Back`](./)

---

# DecodeResult

Namespace: LLama.Native

Return codes from llama_decode

```csharp
public enum DecodeResult
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [DecodeResult](./llama.native.decoderesult.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Error | -1 | An unspecified error |
| Ok | 0 | Ok. |
| NoKvSlot | 1 | Could not find a KV slot for the batch (try reducing the size of the batch or increase the context) |
| ComputeAborted | 2 | Compute was aborted (e.g. due to callback request or timeout) |
| AllocationFailed | -2 | Failed to allocate memory or reserve output space |
| DecodeFailed | -3 | General failure during decode (e.g. internal error, slot failure) |

---

[`< Back`](./)
