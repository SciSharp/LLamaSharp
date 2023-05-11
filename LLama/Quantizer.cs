using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama
{
    public class Quantizer
    {
        public static bool Quantize(string srcFileName, string dstFilename, LLamaFtype ftype, int nthread = 0, bool printInfo = true)
        {
            return Quantize(srcFileName, dstFilename, FtypeToString(ftype), nthread, printInfo);
        }

        public static bool Quantize(string srcFileName, string dstFilename, string ftype, int nthread = 0, bool printInfo = true)
        {
            if (!ValidateFtype(ftype))
            {
                throw new ArgumentException($"The type {Enum.GetName(typeof(LLamaFtype), ftype)} is not a valid type " +
                    $"to perform quantization.");
            }

            return NativeApi.ggml_custom_quantize(srcFileName, dstFilename, ftype, nthread, printInfo);
        }

        private static bool ValidateFtype(string ftype)
        {
            return new string[] { "q4_0", "q4_1", "q4_2", "q5_0", "q5_1", "q8_0" }.Contains(ftype);
        }

        private static string FtypeToString(LLamaFtype ftype)
        {
            return ftype switch
            {
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_0 => "q4_0",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_1 => "q4_1",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q4_2 => "q4_2",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_0 => "q5_0",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q5_1 => "q5_1",
                LLamaFtype.LLAMA_FTYPE_MOSTLY_Q8_0 => "q8_0",
                _ => throw new ArgumentException($"The type {Enum.GetName(typeof(LLamaFtype), ftype)} is not a valid type " +
                    $"to perform quantization.")
            };
        }
    }
}
