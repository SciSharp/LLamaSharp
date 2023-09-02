# LLamaDefaultLogger

Namespace: LLama.Common

The default logger of LLamaSharp. On default it write to console. Use methods of `LLamaLogger.Default` to change the behavior.
 It's recommended to inherit `ILLamaLogger` to customize the behavior.

```csharp
public sealed class LLamaDefaultLogger : ILLamaLogger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>
Implements [ILLamaLogger](./llama.common.illamalogger.md)

## Properties

### **Default**

Get the default logger instance

```csharp
public static LLamaDefaultLogger Default { get; }
```

#### Property Value

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

## Methods

### **EnableNative()**

Enable logging output from llama.cpp

```csharp
public LLamaDefaultLogger EnableNative()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **EnableConsole()**

Enable writing log messages to console

```csharp
public LLamaDefaultLogger EnableConsole()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **DisableConsole()**

Disable writing messages to console

```csharp
public LLamaDefaultLogger DisableConsole()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **EnableFile(String, FileMode)**

Enable writing log messages to file

```csharp
public LLamaDefaultLogger EnableFile(string filename, FileMode mode)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`mode` [FileMode](https://docs.microsoft.com/en-us/dotnet/api/system.io.filemode)<br>

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **DisableFile(String)**

#### Caution

Use DisableFile method without 'filename' parameter

---

Disable writing log messages to file

```csharp
public LLamaDefaultLogger DisableFile(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
unused!

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **DisableFile()**

Disable writing log messages to file

```csharp
public LLamaDefaultLogger DisableFile()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **Log(String, String, LogLevel)**

Log a message

```csharp
public void Log(string source, string message, LogLevel level)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The source of this message (e.g. class name)

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The message to log

`level` [LogLevel](./llama.common.illamalogger.loglevel.md)<br>
Severity level of this message

### **Info(String)**

Write a log message with "Info" severity

```csharp
public void Info(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Warn(String)**

Write a log message with "Warn" severity

```csharp
public void Warn(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Error(String)**

Write a log message with "Error" severity

```csharp
public void Error(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
