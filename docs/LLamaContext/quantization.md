# Quantization

Quantization is significant to accelerate the model inference. Since there's little accuracy (performance) reduction when quantizing the model, get it easy to quantize it!

To quantize the model, please call `Quantize` from `LLamaQuantizer`, which is a static method.

```cs
string srcPath = "<model.bin>";
string dstPath = "<model_q4_0.bin>";
LLamaQuantizer.Quantize(srcPath, dstPath, "q4_0");
// The following overload is also okay.
// LLamaQuantizer.Quantize(srcPath, dstPath, LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_0);
```

After calling it, a quantized model file will be saved.

There're currently 5 types of quantization supported:

- q4_0
- q4_1
- q5_0
- q5_1
- q8_0