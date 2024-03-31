# MetadataOverrideConverter

Namespace: LLama.Abstractions

A JSON converter for [MetadataOverride](./llama.abstractions.metadataoverride.md)

```csharp
public class MetadataOverrideConverter : System.Text.Json.Serialization.JsonConverter`1[[LLama.Abstractions.MetadataOverride, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter&lt;MetadataOverride&gt; → [MetadataOverrideConverter](./llama.abstractions.metadataoverrideconverter.md)

## Properties

### **HandleNull**

```csharp
public bool HandleNull { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **MetadataOverrideConverter()**

```csharp
public MetadataOverrideConverter()
```

## Methods

### **Read(Utf8JsonReader&, Type, JsonSerializerOptions)**

```csharp
public MetadataOverride Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`reader` Utf8JsonReader&<br>

`typeToConvert` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`options` JsonSerializerOptions<br>

#### Returns

[MetadataOverride](./llama.abstractions.metadataoverride.md)<br>

### **Write(Utf8JsonWriter, MetadataOverride, JsonSerializerOptions)**

```csharp
public void Write(Utf8JsonWriter writer, MetadataOverride value, JsonSerializerOptions options)
```

#### Parameters

`writer` Utf8JsonWriter<br>

`value` [MetadataOverride](./llama.abstractions.metadataoverride.md)<br>

`options` JsonSerializerOptions<br>
