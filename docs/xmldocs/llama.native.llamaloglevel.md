# LLamaLogLevel

Namespace: LLama.Native

Severity level of a log message

```csharp
public enum LLamaLogLevel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LLamaLogLevel](./llama.native.llamaloglevel.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Error | 2 | Logs that highlight when the current flow of execution is stopped due to a failure. |
| Warning | 3 | Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop. |
| Info | 4 | Logs that track the general flow of the application. |
| Debug | 5 | Logs that are used for interactive investigation during development. |
