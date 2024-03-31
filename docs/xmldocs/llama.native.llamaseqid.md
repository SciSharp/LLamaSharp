# LLamaSeqId

Namespace: LLama.Native

ID for a sequence in a batch

```csharp
public struct LLamaSeqId
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaSeqId](./llama.native.llamaseqid.md)<br>
Implements [IEquatable&lt;LLamaSeqId&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Value**

The raw value

```csharp
public int Value;
```

### **Zero**

LLamaSeqId with value 0

```csharp
public static LLamaSeqId Zero;
```

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

### **Equals(LLamaSeqId)**

```csharp
bool Equals(LLamaSeqId other)
```

#### Parameters

`other` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
