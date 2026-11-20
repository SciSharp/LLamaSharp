[`< Back`](./)

---

# NuintConverter

Namespace: LLama.Common

A JSON converter for [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr), which System.Text.Json does not support natively.
 Values are written as unsigned 64-bit numbers.

```csharp
public class NuintConverter : System.Text.Json.Serialization.JsonConverter`1[[System.UIntPtr, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter) → [JsonConverter&lt;UIntPtr&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1) → [NuintConverter](./llama.common.nuintconverter.md)<br>
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

### **NuintConverter()**

```csharp
public NuintConverter()
```

## Methods

### **Read(ref Utf8JsonReader, Type, JsonSerializerOptions)**

```csharp
public override UIntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

#### Parameters

`ref` `reader` [Utf8JsonReader](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader)<br>

`typeToConvert` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **Write(Utf8JsonWriter, UIntPtr, JsonSerializerOptions)**

```csharp
public override void Write(Utf8JsonWriter writer, UIntPtr value, JsonSerializerOptions options)
```

#### Parameters

`writer` [Utf8JsonWriter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonwriter)<br>

`value` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

`options` [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)<br>

---

[`< Back`](./)
