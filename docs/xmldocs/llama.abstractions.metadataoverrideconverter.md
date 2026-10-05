[`< Back`](./)

---

# MetadataOverrideConverter

Namespace: LLama.Abstractions

A JSON converter for [MetadataOverride](./llama.abstractions.metadataoverride.md)

```csharp
public class MetadataOverrideConverter : System.Text.Json.Serialization.JsonConverter`1[[LLama.Abstractions.MetadataOverride, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter) → [JsonConverter&lt;MetadataOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1) → [MetadataOverrideConverter](./llama.abstractions.metadataoverrideconverter.md)<br>
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

### **MetadataOverrideConverter()**

```csharp
public MetadataOverrideConverter()
```

## Methods

### **Read(ref Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public override MetadataOverride Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`ref` `reader` [Utf8JsonReader](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader)<br>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

#### Returns

[MetadataOverride](./llama.abstractions.metadataoverride.md)<br>

### **Write(Utf8JsonWriter, MetadataOverride, JsonSerializerOptions)**

```csharp
public override void Write(Utf8JsonWriter writer, MetadataOverride value, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonwriter)<br>

`value` [MetadataOverride](./llama.abstractions.metadataoverride.md)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

---

[`< Back`](./)
