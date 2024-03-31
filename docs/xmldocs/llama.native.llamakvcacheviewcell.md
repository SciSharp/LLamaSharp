# LLamaKvCacheViewCell

Namespace: LLama.Native

Information associated with an individual cell in the KV cache view (llama_kv_cache_view_cell)

```csharp
public struct LLamaKvCacheViewCell
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaKvCacheViewCell](./llama.native.llamakvcacheviewcell.md)

## Fields

### **pos**

The position for this cell. Takes KV cache shifts into account.
 May be negative if the cell is not populated.

```csharp
public LLamaPos pos;
```
