[`< Back`](./)

---

# LLamaPos

Namespace: LLama.Native

Indicates position in a sequence

```csharp
public record struct LLamaPos
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaPos](./llama.native.llamapos.md)<br>
Implements [IEquatable&lt;LLamaPos&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Fields

### **Value**

The raw value

```csharp
public int Value;
```

## Operators

### **explicit operator int(LLamaPos)**

Convert a LLamaPos into an integer (extract the raw value)

```csharp
public static explicit operator int(LLamaPos pos)
```

#### Parameters

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **implicit operator LLamaPos(Int32)**

Convert an integer into a LLamaPos

```csharp
public static implicit operator LLamaPos(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

### **operator ++(LLamaPos)**

Increment this position

```csharp
public static LLamaPos operator ++(LLamaPos pos)
```

#### Parameters

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

### **operator --(LLamaPos)**

Increment this position

```csharp
public static LLamaPos operator --(LLamaPos pos)
```

#### Parameters

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

---

[`< Back`](./)
