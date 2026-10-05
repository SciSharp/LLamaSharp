[`< Back`](./)

---

# FixedSizeQueue&lt;T&gt;

Namespace: LLama.Common

A queue with fixed storage size backed by a circular buffer.

```csharp
public class FixedSizeQueue<T> : IReadOnlyList`1, IReadOnlyCollection`1, IEnumerable`1, System.Collections.IEnumerable
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FixedSizeQueue&lt;T&gt;](./llama.common.fixedsizequeue-1.md)<br>
Implements IReadOnlyList&lt;T&gt;, IReadOnlyCollection&lt;T&gt;, IEnumerable&lt;T&gt;, [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [DefaultMemberAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.defaultmemberattribute)

## Properties

### **Count**

Number of items in this queue

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**

Maximum number of items allowed in this queue

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Indexers

### **this[Int32]**

```csharp
public T this[int index] { get; }
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Property Value

T<br>

## Constructors

### **FixedSizeQueue(Int32)**

Create a new queue.

```csharp
public FixedSizeQueue(int size)
```

#### Parameters

`size` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The maximum number of items to store in this queue.

### **FixedSizeQueue(Int32, IEnumerable&lt;T&gt;)**

Fill the queue with existing data. Please ensure that data.Count &lt;= size

```csharp
public FixedSizeQueue(int size, IEnumerable<T> data)
```

#### Parameters

`size` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`data` IEnumerable&lt;T&gt;<br>

## Methods

### **Enqueue(T)**

Enqueue an element. When the queue is full the oldest element is overwritten.

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

---

[`< Back`](./)
