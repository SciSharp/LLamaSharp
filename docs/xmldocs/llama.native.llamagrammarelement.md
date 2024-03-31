# LLamaGrammarElement

Namespace: LLama.Native

An element of a grammar

```csharp
public struct LLamaGrammarElement
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaGrammarElement](./llama.native.llamagrammarelement.md)<br>
Implements [IEquatable&lt;LLamaGrammarElement&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Type**

The type of this element

```csharp
public LLamaGrammarElementType Type;
```

### **Value**

Unicode code point or rule ID

```csharp
public uint Value;
```

## Constructors

### **LLamaGrammarElement(LLamaGrammarElementType, UInt32)**

Construct a new LLamaGrammarElement

```csharp
LLamaGrammarElement(LLamaGrammarElementType type, uint value)
```

#### Parameters

`type` [LLamaGrammarElementType](./llama.native.llamagrammarelementtype.md)<br>

`value` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

## Methods

### **IsCharElement()**

```csharp
bool IsCharElement()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **Equals(LLamaGrammarElement)**

```csharp
bool Equals(LLamaGrammarElement other)
```

#### Parameters

`other` [LLamaGrammarElement](./llama.native.llamagrammarelement.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
