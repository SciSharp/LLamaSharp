# Utils

Namespace: LLama

Assorted llama utilities

```csharp
public static class Utils
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Utils](./llama.utils.md)

## Methods

### **InitLLamaContextFromModelParams(IModelParams)**

#### Caution

Use LLamaWeights.LoadFromFile and LLamaWeights.CreateContext instead

---

```csharp
public static SafeLLamaContextHandle InitLLamaContextFromModelParams(IModelParams params)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **Tokenize(SafeLLamaContextHandle, String, Boolean, Encoding)**

#### Caution

Use SafeLLamaContextHandle Tokenize method instead

---

```csharp
public static IEnumerable<int> Tokenize(SafeLLamaContextHandle ctx, string text, bool add_bos, Encoding encoding)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[IEnumerable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **GetLogits(SafeLLamaContextHandle, Int32)**

#### Caution

Use SafeLLamaContextHandle GetLogits method instead

---

```csharp
public static Span<float> GetLogits(SafeLLamaContextHandle ctx, int length)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`length` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **Eval(SafeLLamaContextHandle, Int32[], Int32, Int32, Int32, Int32)**

#### Caution

Use SafeLLamaContextHandle Eval method instead

---

```csharp
public static int Eval(SafeLLamaContextHandle ctx, Int32[] tokens, int startIndex, int n_tokens, int n_past, int n_threads)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`tokens` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`startIndex` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_tokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TokenToString(Int32, SafeLLamaContextHandle, Encoding)**

#### Caution

Use SafeLLamaContextHandle TokenToString method instead

---

```csharp
public static string TokenToString(int token, SafeLLamaContextHandle ctx, Encoding encoding)
```

#### Parameters

`token` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PtrToString(IntPtr, Encoding)**

#### Caution

No longer used internally by LlamaSharp

---

```csharp
public static string PtrToString(IntPtr ptr, Encoding encoding)
```

#### Parameters

`ptr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
