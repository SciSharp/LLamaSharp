# LLamaModelQuantizeParams

Namespace: LLama.Native

Quantizer parameters used in the native API

```csharp
public struct LLamaModelQuantizeParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelQuantizeParams](./llama.native.llamamodelquantizeparams.md)

**Remarks:**

llama_model_quantize_params

## Fields

### **nthread**

number of threads to use for quantizing, if &lt;=0 will use std::thread::hardware_concurrency()

```csharp
public int nthread;
```

### **ftype**

quantize to this llama_ftype

```csharp
public LLamaFtype ftype;
```

### **imatrix**

pointer to importance matrix data

```csharp
public IntPtr imatrix;
```

## Properties

### **allow_requantize**

allow quantizing non-f32/f16 tensors

```csharp
public bool allow_requantize { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **quantize_output_tensor**

quantize output.weight

```csharp
public bool quantize_output_tensor { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **only_copy**

only copy tensors - ftype, allow_requantize and quantize_output_tensor are ignored

```csharp
public bool only_copy { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **pure**

disable k-quant mixtures and quantize all tensors to the same type

```csharp
public bool pure { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
