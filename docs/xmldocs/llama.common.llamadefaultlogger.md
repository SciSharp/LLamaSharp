# LLamaDefaultLogger

Namespace: LLama.Common

The default logger of LLamaSharp. On default it write to console. User methods of `LLamaLogger.Default` to change the behavior.
 It's more recommended to inherit `ILLamaLogger` to cosutomize the behavior.

```csharp
public sealed class LLamaDefaultLogger : ILLamaLogger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>
Implements [ILLamaLogger](./llama.common.illamalogger.md)

## Properties

### **Default**

```csharp
public static LLamaDefaultLogger Default { get; }
```

#### Property Value

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

## Methods

### **EnableConsole()**

```csharp
public LLamaDefaultLogger EnableConsole()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **DisableConsole()**

```csharp
public LLamaDefaultLogger DisableConsole()
```

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **EnableFile(String, FileMode)**

```csharp
public LLamaDefaultLogger EnableFile(string filename, FileMode mode)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`mode` [FileMode](https://docs.microsoft.com/en-us/dotnet/api/system.io.filemode)<br>

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **DisableFile(String)**

```csharp
public LLamaDefaultLogger DisableFile(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[LLamaDefaultLogger](./llama.common.llamadefaultlogger.md)<br>

### **Log(String, String, LogLevel)**

```csharp
public void Log(string source, string message, LogLevel level)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`level` [LogLevel](./llama.common.illamalogger.loglevel.md)<br>

### **Info(String)**

```csharp
public void Info(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Warn(String)**

```csharp
public void Warn(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Error(String)**

```csharp
public void Error(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
