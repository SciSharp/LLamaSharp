# FixedSizeQueue&lt;T&gt;

Namespace: LLama.Common

A queue with fixed storage size.
 Currently it's only a naive implementation and needs to be further optimized in the future.

```csharp
public class FixedSizeQueue<T> : , , , System.Collections.IEnumerable
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FixedSizeQueue&lt;T&gt;](./llama.common.fixedsizequeue-1.md)<br>
Implements IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public T Item { get; }
```

#### Property Value

T<br>

### **Count**

Number of items in this queue

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**

Maximum number of items allowed in this queue

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **FixedSizeQueue(Int32)**

Create a new queue

```csharp
public FixedSizeQueue(int size)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
the maximum number of items to store in this queue

### **FixedSizeQueue(Int32, IEnumerable&lt;T&gt;)**

Fill the quene with the data. Please ensure that data.Count &lt;= size

```csharp
public FixedSizeQueue(int size, IEnumerable<T> data)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`data` IEnumerable&lt;T&gt;<br>

## Methods

### **Enqueue(T)**

Enquene an element.

```csharp
public void Enqueue(T item)
```

#### Parameters

`item` T<br>

### **GetEnumerator()**

```csharp
public IEnumerator<T> GetEnumerator()
```

#### Returns

IEnumerator&lt;T&gt;<br>
