# ResettableLLamaModel

Namespace: LLama

A LLamaModel what could be reset. Note that using this class will consume about 10% more memories.

```csharp
public class ResettableLLamaModel : LLamaModel, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaModel](./llama.llamamodel.md) → [ResettableLLamaModel](./llama.resettablellamamodel.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **OriginalState**

The initial state of the model

```csharp
public Byte[] OriginalState { get; set; }
```

#### Property Value

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **ContextSize**

The context size.

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Params**

The model params set for this model.

```csharp
public ModelParams Params { get; set; }
```

#### Property Value

[ModelParams](./llama.common.modelparams.md)<br>

### **NativeHandle**

The native handle, which is used to be passed to the native APIs. Please avoid using it 
 unless you know what is the usage of the Native API.

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

### **Encoding**

The encoding set for this model to deal with text input.

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

## Constructors

### **ResettableLLamaModel(ModelParams, String)**



```csharp
public ResettableLLamaModel(ModelParams Params, string encoding)
```

#### Parameters

`Params` [ModelParams](./llama.common.modelparams.md)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **Reset()**

Reset the state to the initial state.

```csharp
public void Reset()
```
