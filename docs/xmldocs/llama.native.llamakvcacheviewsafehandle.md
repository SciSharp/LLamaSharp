# LLamaKvCacheViewSafeHandle

Namespace: LLama.Native

A safe handle for a LLamaKvCacheView

```csharp
public class LLamaKvCacheViewSafeHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)<br>
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

## Constructors

### **LLamaKvCacheViewSafeHandle(SafeLLamaContextHandle, LLamaKvCacheView)**

Initialize a LLamaKvCacheViewSafeHandle which will call `llama_kv_cache_view_free` when disposed

```csharp
public LLamaKvCacheViewSafeHandle(SafeLLamaContextHandle ctx, LLamaKvCacheView view)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`view` [LLamaKvCacheView](./llama.native.llamakvcacheview.md)<br>

## Methods

### **Allocate(SafeLLamaContextHandle, Int32)**

Allocate a new KV cache view which can be used to inspect the KV cache

```csharp
public static LLamaKvCacheViewSafeHandle Allocate(SafeLLamaContextHandle ctx, int maxSequences)
```

#### Parameters

`ctx` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`maxSequences` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The maximum number of sequences visible in this view per cell

#### Returns

[LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)<br>

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Update()**

Update this view

```csharp
public void Update()
```

### **GetView()**

Get the raw KV cache view

```csharp
public LLamaKvCacheView& GetView()
```

#### Returns

[LLamaKvCacheView&](./llama.native.llamakvcacheview&.md)<br>
