[`< Back`](./)

---

# LLamaPerfContextTimings

Namespace: LLama.Native

LLama performance information

```csharp
public struct LLamaPerfContextTimings
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaPerfContextTimings](./llama.native.llamaperfcontexttimings.md)

**Remarks:**

llama_perf_context_data

## Properties

### **ResetTimestamp**

Timestamp when reset was last called

```csharp
public TimeSpan ResetTimestamp { get; }
```

#### Property Value

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **Loading**

Time spent loading

```csharp
public TimeSpan Loading { get; }
```

#### Property Value

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **PromptEval**

total milliseconds spent prompt processing

```csharp
public TimeSpan PromptEval { get; }
```

#### Property Value

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **Eval**

Total milliseconds in eval/decode calls

```csharp
public TimeSpan Eval { get; }
```

#### Property Value

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **PrompTokensEvaluated**

number of tokens in eval calls for the prompt (with batch size &gt; 1)

```csharp
public int PrompTokensEvaluated { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TokensEvaluated**

number of eval calls

```csharp
public int TokensEvaluated { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

---

[`< Back`](./)
