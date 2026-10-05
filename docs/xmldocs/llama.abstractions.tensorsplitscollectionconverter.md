[`< Back`](./)

---

# TensorSplitsCollectionConverter

Namespace: LLama.Abstractions

A JSON converter for [TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)

```csharp
public class TensorSplitsCollectionConverter : System.Text.Json.Serialization.JsonConverter`1[[LLama.Abstractions.TensorSplitsCollection, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter) → [JsonConverter&lt;TensorSplitsCollection&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1) → [TensorSplitsCollectionConverter](./llama.abstractions.tensorsplitscollectionconverter.md)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **HandleNull**

```csharp
public virtual bool HandleNull { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Type**

```csharp
public sealed override Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br>

## Constructors

### **TensorSplitsCollectionConverter()**

```csharp
public TensorSplitsCollectionConverter()
```

## Methods

### **Read(ref Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public override TensorSplitsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`ref` `reader` [Utf8JsonReader](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader)<br>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

#### Returns

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **Write(Utf8JsonWriter, TensorSplitsCollection, JsonSerializerOptions)**

```csharp
public override void Write(Utf8JsonWriter writer, TensorSplitsCollection value, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonwriter)<br>

`value` [TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

---

[`< Back`](./)
