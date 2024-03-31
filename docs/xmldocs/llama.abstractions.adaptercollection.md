# AdapterCollection

Namespace: LLama.Abstractions

A list of LoraAdapter objects

```csharp
public sealed class AdapterCollection : System.Collections.Generic.List`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.Generic.IList`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.Generic.ICollection`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.Generic.IEnumerable`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.IEnumerable, System.Collections.IList, System.Collections.ICollection, System.Collections.Generic.IReadOnlyList`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Collections.Generic.IReadOnlyCollection`1[[LLama.Abstractions.LoraAdapter, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], System.IEquatable`1[[LLama.Abstractions.AdapterCollection, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [List&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1) → [AdapterCollection](./llama.abstractions.adaptercollection.md)<br>
Implements [IList&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable), [IList](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ilist), [ICollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection), [IReadOnlyList&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1), [IReadOnlyCollection&lt;LoraAdapter&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1), [IEquatable&lt;AdapterCollection&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **Capacity**

```csharp
public int Capacity { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Item**

```csharp
public LoraAdapter Item { get; set; }
```

#### Property Value

[LoraAdapter](./llama.abstractions.loraadapter.md)<br>

## Constructors

### **AdapterCollection()**

```csharp
public AdapterCollection()
```

## Methods

### **Equals(AdapterCollection)**

```csharp
public bool Equals(AdapterCollection other)
```

#### Parameters

`other` [AdapterCollection](./llama.abstractions.adaptercollection.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
