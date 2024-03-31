# LoraAdapter

Namespace: LLama.Abstractions

A LoRA adapter to apply to a model

```csharp
public struct LoraAdapter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LoraAdapter](./llama.abstractions.loraadapter.md)<br>
Implements [IEquatable&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Path**

Path to the LoRA file

```csharp
public string Path { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Scale**

Strength of this LoRA

```csharp
public float Scale { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

## Constructors

### **LoraAdapter(String, Single)**

A LoRA adapter to apply to a model

```csharp
LoraAdapter(string Path, float Scale)
```

#### Parameters

`Path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the LoRA file

`Scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
Strength of this LoRA

## Methods

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(LoraAdapter)**

```csharp
bool Equals(LoraAdapter other)
```

#### Parameters

`other` [LoraAdapter](./llama.abstractions.loraadapter.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Deconstruct(String&, Single&)**

```csharp
void Deconstruct(String& Path, Single& Scale)
```

#### Parameters

`Path` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>

`Scale` [Single&](https://docs.microsoft.com/en-us/dotnet/api/system.single&)<br>
