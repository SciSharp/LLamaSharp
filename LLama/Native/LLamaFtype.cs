﻿namespace LLama.Native
{
    /// <summary>
    /// Supported model file types
    /// </summary>
    public enum LLamaFtype
    {
        /// <summary>
        /// All f32
        /// </summary>
        /// <remarks>Benchmark@7B: 26GB</remarks>
        LLAMA_FTYPE_ALL_F32 = 0,

        /// <summary>
        /// Mostly f16
        /// </summary>
        /// <remarks>Benchmark@7B: 13GB</remarks>
        LLAMA_FTYPE_MOSTLY_F16 = 1,

        /// <summary>
        /// Mostly 8 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 6.7GB, +0.0004ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q8_0 = 7,

        /// <summary>
        /// Mostly 4 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 3.50GB, +0.2499 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q4_0 = 2,

        /// <summary>
        /// Mostly 4 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 3.90GB, +0.1846 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q4_1 = 3,

        /// <summary>
        /// Mostly 4 bit, tok_embeddings.weight and output.weight are f16
        /// </summary>
        LLAMA_FTYPE_MOSTLY_Q4_1_SOME_F16 = 4,

        /// <summary>
        /// Mostly 5 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 4.30GB @ 7B tokens, +0.0796 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q5_0 = 8,

        /// <summary>
        /// Mostly 5 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 4.70GB, +0.0415 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q5_1 = 9,

        /// <summary>
        /// K-Quant 2 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 2.67GB @ 7N parameters, +0.8698 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q2_K = 10,

        /// <summary>
        /// K-Quant 3 bit (Small)
        /// </summary>
        /// <remarks>Benchmark@7B: 2.75GB, +0.5505 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q3_K_S = 11,

        /// <summary>
        /// K-Quant 3 bit (Medium)
        /// </summary>
        /// <remarks>Benchmark@7B: 3.06GB, +0.2437 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q3_K_M = 12,

        /// <summary>
        /// K-Quant 3 bit (Large)
        /// </summary>
        /// <remarks>Benchmark@7B: 3.35GB, +0.1803 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q3_K_L = 13,

        /// <summary>
        /// K-Quant 4 bit (Small)
        /// </summary>
        /// <remarks>Benchmark@7B: 3.56GB, +0.1149 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q4_K_S = 14,

        /// <summary>
        /// K-Quant 4 bit (Medium)
        /// </summary>
        /// <remarks>Benchmark@7B: 3.80GB, +0.0535 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q4_K_M = 15,

        /// <summary>
        /// K-Quant 5 bit (Small)
        /// </summary>
        /// <remarks>Benchmark@7B: 4.33GB, +0.0353 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q5_K_S = 16,

        /// <summary>
        /// K-Quant 5 bit (Medium)
        /// </summary>
        /// <remarks>Benchmark@7B: 4.45GB, +0.0142 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q5_K_M = 17,

        /// <summary>
        /// K-Quant 6 bit
        /// </summary>
        /// <remarks>Benchmark@7B: 5.15GB, +0.0044 ppl</remarks>
        LLAMA_FTYPE_MOSTLY_Q6_K = 18,

        /// <summary>
        /// File type was not specified
        /// </summary>
        LLAMA_FTYPE_GUESSED = 1024
    }
}
