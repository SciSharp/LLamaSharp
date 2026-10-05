[`< Back`](./)

---

# ExecutorBaseState

Namespace: LLama

Serializable snapshot of executor state used for persistence and restart.

```csharp
internal class ExecutorBaseState
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [JsonConverterAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterattribute)

## Properties

### **PastTokensCount**

```csharp
public int PastTokensCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ConsumedTokensCount**

```csharp
public int ConsumedTokensCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ConsumedSessionCount**

```csharp
public int ConsumedSessionCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MatchingSessionTokensCount**

```csharp
public int MatchingSessionTokensCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SessionFilePath**

```csharp
public string? SessionFilePath { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Embeds**

```csharp
public LLamaToken[] Embeds { get; set; }
```

#### Property Value

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **EmbedInps**

```csharp
public LLamaToken[] EmbedInps { get; set; }
```

#### Property Value

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **SessionTokens**

```csharp
public LLamaToken[] SessionTokens { get; set; }
```

#### Property Value

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **LastTokens**

```csharp
public LLamaToken[] LastTokens { get; set; }
```

#### Property Value

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **LastTokensCapacity**

```csharp
public int LastTokensCapacity { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MirostatMu**

```csharp
public float? MirostatMu { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

## Constructors

### **ExecutorBaseState()**

```csharp
public ExecutorBaseState()
```

---

[`< Back`](./)
