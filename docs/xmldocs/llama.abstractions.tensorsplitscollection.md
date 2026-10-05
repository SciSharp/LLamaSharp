[`< Back`](./)

---

# TensorSplitsCollection

Namespace: LLama.Abstractions

A fixed size array to set the tensor splits across multiple GPUs

```csharp
public sealed class TensorSplitsCollection : System.Collections.Generic.IEnumerable`1[[System.Single, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Collections.IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>
Implements [IEnumerable&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [DefaultMemberAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.defaultmemberattribute), [JsonConverterAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterattribute)

## Properties

### **Length**

The size of this array

```csharp
public int Length { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Indexers

### **this[Int32]**

```csharp
public float this[int index] { get; set; }
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Property Value

[Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

## Constructors

### **TensorSplitsCollection(Single[])**

Create a new tensor splits collection, copying the given values

```csharp
public TensorSplitsCollection(Single[] splits)
```

#### Parameters

`splits` [Single[]](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **TensorSplitsCollection()**

Create a new tensor splits collection with all values initialised to the default

```csharp
public TensorSplitsCollection()
```

## Methods

### **Clear()**

Set all values to zero

```csharp
public void Clear()
```

### **GetEnumerator()**

```csharp
public IEnumerator<float> GetEnumerator()
```

#### Returns

[IEnumerator&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br>

---

[`< Back`](./)
