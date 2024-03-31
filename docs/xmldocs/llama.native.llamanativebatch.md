# LLamaNativeBatch

Namespace: LLama.Native

Input data for llama_decode
 A llama_batch object can contain input about one or many sequences
 The provided arrays (i.e. token, embd, pos, etc.) must have size of n_tokens

```csharp
public struct LLamaNativeBatch
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaNativeBatch](./llama.native.llamanativebatch.md)

## Fields

### **n_tokens**

The number of items pointed at by pos, seq_id and logits.

```csharp
public int n_tokens;
```

### **tokens**

Either `n_tokens` of `llama_token`, or `NULL`, depending on how this batch was created

```csharp
public LLamaToken* tokens;
```

### **embd**

Either `n_tokens * embd * sizeof(float)` or `NULL`, depending on how this batch was created

```csharp
public Single* embd;
```

### **pos**

the positions of the respective token in the sequence

```csharp
public LLamaPos* pos;
```

### **n_seq_id**

https://github.com/ggerganov/llama.cpp/blob/master/llama.h#L139 ???

```csharp
public Int32* n_seq_id;
```

### **seq_id**

the sequence to which the respective token belongs

```csharp
public LLamaSeqId** seq_id;
```

### **logits**

if zero, the logits for the respective token will not be output

```csharp
public Byte* logits;
```
