using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama
{
    public class Quantizer
    {
        /// <summary>
        /// Quantize the model.
        /// </summary>
        /// <param name="srcFileName">The model file to be quantized.</param>
        /// <param name="dstFilename">The path to save the quantized model.</param>
        /// <param name="ftype">The type of quantization.</param>
        /// <param name="nthread">Thread to be used during the quantization. By default it's the physical core number.</param>
        /// <returns>Whether the quantization is successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Quantize(string srcFileName, string dstFilename, LLamaFtype ftype, int nthread = -1)
        {
            if (!ValidateFtype(ftype))
            {
                throw new ArgumentException($"The type {Enum.GetName(typeof(LLamaFtype), ftype)} is not a valid type " +
                    $"to perform quantization.");
            }
            return NativeApi.llama_model_quantize(srcFileName, dstFilename, ftype, nthread) == 0;
        }

        /// <summary>
        /// Quantize the model.
        /// </summary>
        /// <param name="srcFileName">The model file to be quantized.</param>
        /// <param name="dstFilename">The path to save the quantized model.</param>
        /// <param name="ftype">The type of quantization.</param>
        /// <param name="nthread">Thread to be used during the quantization. By default it's the physical core number.</param>
        /// <returns>Whether the quantization is successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Quantize(string srcFileName, string dstFilename, string ftype, int nthread = -1)
        {
            return Quantize(srcFileName, dstFilename, StringToFtype(ftype), nthread);
        }

        private static bool ValidateFtype(string ftype)
        {
            return new string[] { "q4_0", "q4_1", "q5_0", "q5_1", "q8_0" }.Contains(ftype);
        }

        private static bool ValidateFtype(LLamaFtype ftype)
        {
            return ftype is LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_0 or LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_1
                or LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_0 or LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_1 or LLamaFtype.LLAMA_FTYPE_MOSTLY_Q8_0;
        }

        private static string FtypeToString(LLamaFtype ftype)
        {
            return ftype switch
            {
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_0 => "q4_0",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_1 => "q4_1",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_0 => "q5_0",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_1 => "q5_1",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q8_0 => "q8_0",
                _ => throw new ArgumentException($"The type {Enum.GetName(typeof(LLamaFtype), ftype)} is not a valid type " +
                    $"to perform quantization.")
            };
        }

        private static LLamaFtype StringToFtype(string str)
        {
            return str switch
            {
                "q4_0" => LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_0,
                "q4_1" => LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_1,
                "q5_0" => LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_0,
                "q5_1" => LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_1,
                "q8_0" => LLamaFtype.LLAMA_FTYPE_MOSTLY_Q8_0,
            };
        }
    }
}
