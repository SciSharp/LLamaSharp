[`< Back`](./)

---

# LLamaSeqId

Namespace: LLama.Native

ID for a sequence in a batch

```csharp
public record struct LLamaSeqId
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaSeqId](./llama.native.llamaseqid.md)<br>
Implements [IEquatable&lt;LLamaSeqId&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Value**

The raw value

```csharp
public int Value;
```

### **Zero**

LLamaSeqId with value 0

```csharp
public static LLamaSeqId Zero;
```

## Operators

### **explicit operator int(LLamaSeqId)**

Convert a LLamaSeqId into an integer (extract the raw value)

```csharp
public static explicit operator int(LLamaSeqId pos)
```

#### Parameters

`pos` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **explicit operator LLamaSeqId(Int32)**

Convert an integer into a LLamaSeqId

```csharp
public static explicit operator LLamaSeqId(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaSeqId](./llama.native.llamaseqid.md)<br>

---

[`< Back`](./)
