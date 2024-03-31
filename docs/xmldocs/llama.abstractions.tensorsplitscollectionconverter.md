# TensorSplitsCollectionConverter

Namespace: LLama.Abstractions

A JSON converter for [TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)

```csharp
public class TensorSplitsCollectionConverter : System.Text.Json.Serialization.JsonConverter`1[[LLama.Abstractions.TensorSplitsCollection, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter&lt;TensorSplitsCollection&gt; → [TensorSplitsCollectionConverter](./llama.abstractions.tensorsplitscollectionconverter.md)

## Properties

### **HandleNull**

```csharp
public bool HandleNull { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **TensorSplitsCollectionConverter()**

```csharp
public TensorSplitsCollectionConverter()
```

## Methods

### **Read(Utf8JsonReader&, Type, JsonSerializerOptions)**

```csharp
public TensorSplitsCollection Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader&<br>

`typeToConvert` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`options` JsonSerializerOptions<br>

#### Returns

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **Write(Utf8JsonWriter, TensorSplitsCollection, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, TensorSplitsCollection value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br>

`value` [TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

`options` JsonSerializerOptions<br>
