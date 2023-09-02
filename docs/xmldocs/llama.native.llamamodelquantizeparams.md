# LLamaModelQuantizeParams

Namespace: LLama.Native

Quantizer parameters used in the native API

```csharp
public struct LLamaModelQuantizeParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelQuantizeParams](./llama.native.llamamodelquantizeparams.md)

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
