# LLamaState

Namespace: LLama

```csharp
public class LLamaState : System.IEquatable`1[[LLama.LLamaState, LLamaSharp, Version=0.2.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaState](./llama.llamastate.md)<br>
Implements [IEquatable&lt;LLamaState&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **EvalTokens**

```csharp
public Queue<int> EvalTokens { get; set; }
```

#### Property Value

[Queue&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1)<br>

### **EvalLogits**

```csharp
public Queue<Single[]> EvalLogits { get; set; }
```

#### Property Value

[Queue&lt;Single[]&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1)<br>

### **State**

```csharp
public Byte[] State { get; set; }
```

#### Property Value

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **Size**

```csharp
public int Size { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **LLamaState(Queue&lt;Int32&gt;, Queue&lt;Single[]&gt;, Byte[], Int32)**

```csharp
public LLamaState(Queue<int> EvalTokens, Queue<Single[]> EvalLogits, Byte[] State, int Size)
```

#### Parameters

`EvalTokens` [Queue&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1)<br>

`EvalLogits` [Queue&lt;Single[]&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1)<br>

`State` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

`Size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(LLamaState)**

```csharp
public bool Equals(LLamaState other)
```

#### Parameters

`other` [LLamaState](./llama.llamastate.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public LLamaState <Clone>$()
```

#### Returns

[LLamaState](./llama.llamastate.md)<br>

### **Deconstruct(Queue`1&, Queue`1&, Byte[]&, Int32&)**

```csharp
public void Deconstruct(Queue`1& EvalTokens, Queue`1& EvalLogits, Byte[]& State, Int32& Size)
```

#### Parameters

`EvalTokens` [Queue`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1&)<br>

`EvalLogits` [Queue`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1&)<br>

`State` [Byte[]&](https://docs.microsoft.com/en-us/dotnet/api/system.byte&)<br>

`Size` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
