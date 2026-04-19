[`< Back`](./)

---

# LLamaKvCacheViewSafeHandle

Namespace: LLama.Native

A safe handle for a LLamaKvCacheView

```csharp
public sealed class LLamaKvCacheViewSafeHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **CellCount**

Number of KV cache cells. This will be the same as the context size.

```csharp
public int CellCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TokenCount**

Get the total number of tokens in the KV cache.
 
 For example, if there are two populated
 cells, the first with 1 sequence id in it and the second with 2 sequence
 ids then you'll have 3 tokens.

```csharp
public int TokenCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxSequenceCount**

Maximum number of sequences visible for a cell. There may be more sequences than this
 in reality, this is simply the maximum number this view can see.

```csharp
public int MaxSequenceCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **UsedCellCount**

Number of populated cache cells

```csharp
public int UsedCellCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxContiguous**

Maximum contiguous empty slots in the cache.

```csharp
public int MaxContiguous { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MaxContiguousIdx**

Index to the start of the MaxContiguous slot range. Can be negative when cache is full.

```csharp
public int MaxContiguousIdx { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

Read the current KV cache state into this view.

```csharp
public void Update()
```

### **GetCell(Int32)**

Get the cell at the given index

```csharp
public LLamaPos GetCell(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index of the cell [0, CellCount)

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>
Data about the cell at the given index

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if index is out of range (0 &lt;= index &lt; CellCount)

### **GetCellSequences(Int32)**

Get all of the sequences assigned to the cell at the given index. This will contain [LLamaKvCacheViewSafeHandle.MaxSequenceCount](./llama.native.llamakvcacheviewsafehandle.md#maxsequencecount) entries
 sequences even if the cell actually has more than that many sequences, allocate a new view with a larger maxSequences parameter
 if necessary. Invalid sequences will be negative values.

```csharp
public Span<LLamaSeqId> GetCellSequences(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The index of the cell [0, CellCount)

#### Returns

[Span&lt;LLamaSeqId&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span containing the sequences assigned to this cell

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if index is out of range (0 &lt;= index &lt; CellCount)

---

[`< Back`](./)
