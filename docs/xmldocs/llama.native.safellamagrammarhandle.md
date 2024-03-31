# SafeLLamaGrammarHandle

Namespace: LLama.Native

A safe reference to a `llama_grammar`

```csharp
public class SafeLLamaGrammarHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **IsInvalid**

```csharp
public bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Create(IReadOnlyList&lt;GrammarRule&gt;, UInt64)**

Create a new llama_grammar

```csharp
public static SafeLLamaGrammarHandle Create(IReadOnlyList<GrammarRule> rules, ulong start_rule_index)
```

#### Parameters

`rules` [IReadOnlyList&lt;GrammarRule&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>
A list of list of elements, each inner list makes up one grammar rule

`start_rule_index` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
The index (in the outer list) of the start rule

#### Returns

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Create(LLamaGrammarElement**, UInt64, UInt64)**

Create a new llama_grammar

```csharp
public static SafeLLamaGrammarHandle Create(LLamaGrammarElement** rules, ulong nrules, ulong start_rule_index)
```

#### Parameters

`rules` [LLamaGrammarElement**](./llama.native.llamagrammarelement**.md)<br>
rules list, each rule is a list of rule elements (terminated by a LLamaGrammarElementType.END element)

`nrules` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
total number of rules

`start_rule_index` [UInt64](https://docs.microsoft.com/en-us/dotnet/api/system.uint64)<br>
index of the start rule of the grammar

#### Returns

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Clone()**

Create a copy of this grammar instance

```csharp
public SafeLLamaGrammarHandle Clone()
```

#### Returns

[SafeLLamaGrammarHandle](./llama.native.safellamagrammarhandle.md)<br>

### **AcceptToken(SafeLLamaContextHandle, LLamaToken)**

Accepts the sampled token into the grammar

```csharp
public void AcceptToken(SafeLLamaContextHandle ctx, LLamaToken token)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
