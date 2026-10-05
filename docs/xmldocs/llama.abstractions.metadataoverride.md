[`< Back`](./)

---

# MetadataOverride

Namespace: LLama.Abstractions

An override for a single key/value pair in model metadata

```csharp
public sealed record class MetadataOverride : System.IEquatable`1[[LLama.Abstractions.MetadataOverride, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MetadataOverride](./llama.abstractions.metadataoverride.md)<br>
Implements [IEquatable&lt;MetadataOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [JsonConverterAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterattribute)

## Properties

### **Key**

Get the key being overridden by this override

```csharp
public string Key { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **MetadataOverride(String, Int32)**

Create a new override for an int key

```csharp
public MetadataOverride(string key, int value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MetadataOverride(String, Single)**

Create a new override for a float key

```csharp
public MetadataOverride(string key, float value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`value` [Single](https://learn.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MetadataOverride(String, Boolean)**

Create a new override for a boolean key

```csharp
public MetadataOverride(string key, bool value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`value` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **MetadataOverride(String, String)**

Create a new override for a string key

```csharp
public MetadataOverride(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

---

[`< Back`](./)
