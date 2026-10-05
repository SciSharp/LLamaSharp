[`< Back`](./)

---

# KvAccessor

Namespace: LLama.Batched

#### Caution

Types with embedded references are not supported in this version of your compiler.

---

Provides direct access to the KV cache of a [Conversation](./llama.batched.conversation.md).
 See [Conversation.Modify(ModifyKvCache)](./llama.batched.conversation.md#modifymodifykvcache) for how to use this.

```csharp
internal readonly ref struct KvAccessor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [KvAccessor](./llama.batched.conversation.kvaccessor.md)<br>
Attributes [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute), [IsByRefLikeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isbyreflikeattribute), [ObsoleteAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.obsoleteattribute), [CompilerFeatureRequiredAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilerfeaturerequiredattribute), [IsReadOnlyAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isreadonlyattribute)

## Methods

### **Remove(LLamaPos, LLamaPos)**

Removes all tokens that have positions in [start, end)

```csharp
public void Remove(LLamaPos start, LLamaPos end)
```

#### Parameters

`start` [LLamaPos](./llama.native.llamapos.md)<br>
Start position (inclusive)

`end` [LLamaPos](./llama.native.llamapos.md)<br>
End position (exclusive)

### **Remove(LLamaPos, Int32)**

Removes `count` tokens starting from `start`

```csharp
public void Remove(LLamaPos start, int count)
```

#### Parameters

`start` [LLamaPos](./llama.native.llamapos.md)<br>
Start position (inclusive)

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of tokens

### **Add(LLamaPos, LLamaPos, Int32)**

Adds relative position "delta" to all tokens that have positions in [p0, p1).
 If the KV cache is RoPEd, the KV data is updated
 accordingly

```csharp
public void Add(LLamaPos start, LLamaPos end, int delta)
```

#### Parameters

`start` [LLamaPos](./llama.native.llamapos.md)<br>
Start position (inclusive)

`end` [LLamaPos](./llama.native.llamapos.md)<br>
End position (exclusive)

`delta` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Amount to add on to each token position

### **Divide(LLamaPos, LLamaPos, Int32)**

Integer division of the positions by factor of `d &gt; 1`.
 If the KV cache is RoPEd, the KV data is updated accordingly.

```csharp
public void Divide(LLamaPos start, LLamaPos end, int divisor)
```

#### Parameters

`start` [LLamaPos](./llama.native.llamapos.md)<br>
Start position (inclusive). If less than zero, it is clamped to zero.

`end` [LLamaPos](./llama.native.llamapos.md)<br>
End position (exclusive). If less than zero, it is treated as "infinity".

`divisor` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
Amount to divide each position by.

---

[`< Back`](./)
