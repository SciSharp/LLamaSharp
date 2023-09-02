# ILLamaLogger

Namespace: LLama.Common

receives log messages from LLamaSharp

```csharp
public interface ILLamaLogger
```

## Methods

### **Log(String, String, LogLevel)**

Write the log in customized way

```csharp
void Log(string source, string message, LogLevel level)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The source of the log. It may be a method name or class name.

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The message.

`level` [LogLevel](./llama.common.illamalogger.loglevel.md)<br>
The log level.
