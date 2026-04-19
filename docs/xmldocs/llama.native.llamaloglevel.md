[`< Back`](./)

---

# LLamaLogLevel

Namespace: LLama.Native

Severity level of a log message. This enum should always be aligned with
 the one defined on llama.cpp side at
 https://github.com/ggerganov/llama.cpp/blob/0eb4e12beebabae46d37b78742f4c5d4dbe52dc1/ggml/include/ggml.h#L559

```csharp
public enum LLamaLogLevel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaLogLevel](./llama.native.llamaloglevel.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | Logs are never written. |
| Debug | 1 | Logs that are used for interactive investigation during development. |
| Info | 2 | Logs that track the general flow of the application. |
| Warning | 3 | Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop. |
| Error | 4 | Logs that highlight when the current flow of execution is stopped due to a failure. |
| Continue | 5 | Continue log level is equivalent to None in the way it is used in llama.cpp. |

---

[`< Back`](./)
