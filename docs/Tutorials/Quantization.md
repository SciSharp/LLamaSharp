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

There're currently the following types of quantization supported:

```cpp
{ "Q4_0",   LLAMA_FTYPE_MOSTLY_Q4_0,   " 3.56G, +0.2166 ppl @ LLaMA-v1-7B", },
{ "Q4_1",   LLAMA_FTYPE_MOSTLY_Q4_1,   " 3.90G, +0.1585 ppl @ LLaMA-v1-7B", },
{ "Q5_0",   LLAMA_FTYPE_MOSTLY_Q5_0,   " 4.33G, +0.0683 ppl @ LLaMA-v1-7B", },
{ "Q5_1",   LLAMA_FTYPE_MOSTLY_Q5_1,   " 4.70G, +0.0349 ppl @ LLaMA-v1-7B", },
{ "IQ2_XXS",LLAMA_FTYPE_MOSTLY_IQ2_XXS," 2.06 bpw quantization",            },
{ "IQ2_XS", LLAMA_FTYPE_MOSTLY_IQ2_XS, " 2.31 bpw quantization",            },
{ "IQ2_S",  LLAMA_FTYPE_MOSTLY_IQ2_S,  " 2.5  bpw quantization",            },
{ "IQ2_M",  LLAMA_FTYPE_MOSTLY_IQ2_M,  " 2.7  bpw quantization",            },
{ "IQ1_S",  LLAMA_FTYPE_MOSTLY_IQ1_S,  " 1.56 bpw quantization",            },
{ "IQ1_M",  LLAMA_FTYPE_MOSTLY_IQ1_M,  " 1.75 bpw quantization",            },
{ "Q2_K",   LLAMA_FTYPE_MOSTLY_Q2_K,   " 2.63G, +0.6717 ppl @ LLaMA-v1-7B", },
{ "Q2_K_S", LLAMA_FTYPE_MOSTLY_Q2_K_S, " 2.16G, +9.0634 ppl @ LLaMA-v1-7B", },
{ "IQ3_XXS",LLAMA_FTYPE_MOSTLY_IQ3_XXS," 3.06 bpw quantization",            },
{ "IQ3_S",  LLAMA_FTYPE_MOSTLY_IQ3_S,  " 3.44 bpw quantization",            },
{ "IQ3_M",  LLAMA_FTYPE_MOSTLY_IQ3_M,  " 3.66 bpw quantization mix",        },
{ "Q3_K",   LLAMA_FTYPE_MOSTLY_Q3_K_M, "alias for Q3_K_M" },
{ "IQ3_XS", LLAMA_FTYPE_MOSTLY_IQ3_XS, " 3.3 bpw quantization"   ,          },
{ "Q3_K_S", LLAMA_FTYPE_MOSTLY_Q3_K_S, " 2.75G, +0.5551 ppl @ LLaMA-v1-7B", },
{ "Q3_K_M", LLAMA_FTYPE_MOSTLY_Q3_K_M, " 3.07G, +0.2496 ppl @ LLaMA-v1-7B", },
{ "Q3_K_L", LLAMA_FTYPE_MOSTLY_Q3_K_L, " 3.35G, +0.1764 ppl @ LLaMA-v1-7B", },
{ "IQ4_NL", LLAMA_FTYPE_MOSTLY_IQ4_NL, " 4.50 bpw non-linear quantization", },
{ "IQ4_XS", LLAMA_FTYPE_MOSTLY_IQ4_XS, " 4.25 bpw non-linear quantization", },
{ "Q4_K",   LLAMA_FTYPE_MOSTLY_Q4_K_M, "alias for Q4_K_M", },
{ "Q4_K_S", LLAMA_FTYPE_MOSTLY_Q4_K_S, " 3.59G, +0.0992 ppl @ LLaMA-v1-7B", },
{ "Q4_K_M", LLAMA_FTYPE_MOSTLY_Q4_K_M, " 3.80G, +0.0532 ppl @ LLaMA-v1-7B", },
{ "Q5_K",   LLAMA_FTYPE_MOSTLY_Q5_K_M, "alias for Q5_K_M", },
{ "Q5_K_S", LLAMA_FTYPE_MOSTLY_Q5_K_S, " 4.33G, +0.0400 ppl @ LLaMA-v1-7B", },
{ "Q5_K_M", LLAMA_FTYPE_MOSTLY_Q5_K_M, " 4.45G, +0.0122 ppl @ LLaMA-v1-7B", },
{ "Q6_K",   LLAMA_FTYPE_MOSTLY_Q6_K,   " 5.15G, +0.0008 ppl @ LLaMA-v1-7B", },
{ "Q8_0",   LLAMA_FTYPE_MOSTLY_Q8_0,   " 6.70G, +0.0004 ppl @ LLaMA-v1-7B", },
{ "F16",    LLAMA_FTYPE_MOSTLY_F16,    "13.00G              @ 7B", },
{ "F32",    LLAMA_FTYPE_ALL_F32,       "26.00G              @ 7B", },
// Note: Ensure COPY comes after F32 to avoid ftype 0 from matching.
{ "COPY",   LLAMA_FTYPE_ALL_F32,       "only copy tensors, no quantizing", },
```