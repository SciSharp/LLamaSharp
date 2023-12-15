namespace LLama.Native;

/// <summary>
/// Possible GGML quantisation types
/// </summary>
public enum GGMLType
{
    /// <summary>
    /// Full 32 bit float
    /// </summary>
    GGML_TYPE_F32 = 0,

    /// <summary>
    /// 16 bit float
    /// </summary>
    GGML_TYPE_F16 = 1,

    /// <summary>
    /// 4 bit float
    /// </summary>
    GGML_TYPE_Q4_0 = 2,

    /// <summary>
    /// 4 bit float
    /// </summary>
    GGML_TYPE_Q4_1 = 3,

    // GGML_TYPE_Q4_2 = 4, support has been removed
    // GGML_TYPE_Q4_3 (5) support has been removed

    /// <summary>
    /// 5 bit float
    /// </summary>
    GGML_TYPE_Q5_0 = 6,

    /// <summary>
    /// 5 bit float
    /// </summary>
    GGML_TYPE_Q5_1 = 7,

    /// <summary>
    /// 8 bit float
    /// </summary>
    GGML_TYPE_Q8_0 = 8,

    /// <summary>
    /// 8 bit float
    /// </summary>
    GGML_TYPE_Q8_1 = 9,

    // k-quantizations

    /// <summary>
    /// "type-1" 2-bit quantization in super-blocks containing 16 blocks, each block having 16 weight.
    /// Block scales and mins are quantized with 4 bits. This ends up effectively using 2.5625 bits per weight (bpw)
    /// </summary>
    GGML_TYPE_Q2_K = 10,

    /// <summary>
    /// "type-0" 3-bit quantization in super-blocks containing 16 blocks, each block having 16 weights.
    /// Scales are quantized with 6 bits. This end up using 3.4375 bpw.
    /// </summary>
    GGML_TYPE_Q3_K = 11,

    /// <summary>
    /// "type-1" 4-bit quantization in super-blocks containing 8 blocks, each block having 32 weights.
    /// Scales and mins are quantized with 6 bits. This ends up using 4.5 bpw.
    /// </summary>
    GGML_TYPE_Q4_K = 12,

    /// <summary>
    /// "type-1" 5-bit quantization. Same super-block structure as GGML_TYPE_Q4_K resulting in 5.5 bpw
    /// </summary>
    GGML_TYPE_Q5_K = 13,

    /// <summary>
    /// "type-0" 6-bit quantization. Super-blocks with 16 blocks, each block having 16 weights.
    /// Scales are quantized with 8 bits. This ends up using 6.5625 bpw
    /// </summary>
    GGML_TYPE_Q6_K = 14,

    /// <summary>
    /// "type-0" 8-bit quantization. Only used for quantizing intermediate results.
    /// The difference to the existing Q8_0 is that the block size is 256. All 2-6 bit dot products are implemented for this quantization type.
    /// </summary>
    GGML_TYPE_Q8_K = 15,

    /// <summary>
    /// Integer, 8 bit
    /// </summary>
    GGML_TYPE_I8 = 16,

    /// <summary>
    /// Integer, 16 bit
    /// </summary>
    GGML_TYPE_I16 = 17,

    /// <summary>
    /// Integer, 32 bit
    /// </summary>
    GGML_TYPE_I32 = 18,

    /// <summary>
    /// The value of this entry is the count of the number of possible quant types.
    /// </summary>
    GGML_TYPE_COUNT,
}