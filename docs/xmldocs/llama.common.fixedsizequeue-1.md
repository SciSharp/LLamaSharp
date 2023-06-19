# FixedSizeQueue&lt;T&gt;

Namespace: LLama.Common

A queue with fixed storage size.
 Currently it's only a naive implementation and needs to be further optimized in the future.

```csharp
public class FixedSizeQueue<T> : , System.Collections.IEnumerable
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FixedSizeQueue&lt;T&gt;](./llama.common.fixedsizequeue-1.md)<br>
Implements IEnumerable&lt;T&gt;, [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Capacity**

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **FixedSizeQueue(Int32)**

```csharp
public FixedSizeQueue(int size)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **FixedSizeQueue(Int32, IEnumerable&lt;T&gt;)**

```csharp
public FixedSizeQueue(int size, IEnumerable<T> data)
```

#### Parameters

`size` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`data` IEnumerable&lt;T&gt;<br>

## Methods

### **FillWith(T)**

```csharp
public FixedSizeQueue<T> FillWith(T value)
```

#### Parameters

`value` T<br>

#### Returns

[FixedSizeQueue&lt;T&gt;](./llama.common.fixedsizequeue-1.md)<br>

### **Enqueue(T)**

Enquene an element.

```csharp
public void Enqueue(T item)
```

#### Parameters

`item` T<br>

### **ToArray()**

```csharp
public T[] ToArray()
```

#### Returns

T[]<br>

### **GetEnumerator()**

```csharp
public IEnumerator<T> GetEnumerator()
```

#### Returns

IEnumerator&lt;T&gt;<br>
