[`< Back`](./)

---

# TextMessage

Namespace: LLama

A message that has been added to a template

```csharp
internal readonly struct TextMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [TextMessage](./llama.llamatemplate.textmessage.md)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [IsReadOnlyAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isreadonlyattribute)

## Properties

### **Role**

The "role" string for this message

```csharp
public string Role { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Content**

The text content of this message

```csharp
public string Content { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Deconstruct(out String, out String)**

Deconstruct this message into role and content

```csharp
public void Deconstruct(out string role, out string content)
```

#### Parameters

`out` `role` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`out` `content` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

---

[`< Back`](./)
