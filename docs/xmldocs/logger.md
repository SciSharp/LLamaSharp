# Logger

Namespace:

```csharp
public sealed class Logger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Logger](./logger.md)

## Properties

### **Default**

```csharp
public static Logger Default { get; }
```

#### Property Value

[Logger](./logger.md)<br>

## Methods

### **ToConsole()**

```csharp
public void ToConsole()
```

### **ToFile(String)**

```csharp
public void ToFile(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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
