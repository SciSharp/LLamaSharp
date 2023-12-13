namespace LLama.Native;

public enum GGMLType
{
    GGML_TYPE_F32 = 0,
    GGML_TYPE_F16 = 1,
    GGML_TYPE_Q4_0 = 2,
    GGML_TYPE_Q4_1 = 3,

    // GGML_TYPE_Q4_2 = 4, support has been removed
    // GGML_TYPE_Q4_3 (5) support has been removed

    GGML_TYPE_Q5_0 = 6,
    GGML_TYPE_Q5_1 = 7,
    GGML_TYPE_Q8_0 = 8,
    GGML_TYPE_Q8_1 = 9,

    // k-quantizations
    GGML_TYPE_Q2_K = 10,
    GGML_TYPE_Q3_K = 11,
    GGML_TYPE_Q4_K = 12,
    GGML_TYPE_Q5_K = 13,
    GGML_TYPE_Q6_K = 14,
    GGML_TYPE_Q8_K = 15,
    GGML_TYPE_I8 = 16,
    GGML_TYPE_I16 = 17,
    GGML_TYPE_I32 = 18,

    GGML_TYPE_COUNT,
}