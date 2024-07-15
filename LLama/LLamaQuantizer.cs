using LLama.Native;
using System;
using System.Collections.Generic;

namespace LLama
{
    /// <summary>
    /// The quantizer to quantize the model.
    /// </summary>
    public static class LLamaQuantizer
    {
        /// <summary>
        /// Quantize the model.
        /// </summary>
        /// <param name="srcFileName">The model file to be quantized.</param>
        /// <param name="dstFilename">The path to save the quantized model.</param>
        /// <param name="ftype">The type of quantization.</param>
        /// <param name="nthread">Thread to be used during the quantization. By default it's the physical core number.</param>
        /// <param name="allowRequantize"></param>
        /// <param name="quantizeOutputTensor"></param>
        /// <returns>Whether the quantization is successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Quantize(
            string srcFileName, string dstFilename, LLamaFtype ftype, int nthread = -1, bool allowRequantize = true, bool quantizeOutputTensor = false)
        {
            if (!ValidateFtype(ftype))
            {
                throw new ArgumentException($"The type {Enum.GetName(typeof(LLamaFtype), ftype)} is not a valid type " +
                    $"to perform quantization.");
            }

            var quantizeParams = LLamaModelQuantizeParams.Default();
            quantizeParams.ftype = ftype;
            quantizeParams.nthread = nthread;
            quantizeParams.allow_requantize = allowRequantize;
            quantizeParams.quantize_output_tensor = quantizeOutputTensor;

            // todo: fill in other quantize params fields.
            // This method could probably do with a redesign - passing in a config object (maybe directly
            // expose `LLamaModelQuantizeParams`) instead of an ever growing list of method parameters!

            return NativeApi.llama_model_quantize(srcFileName, dstFilename, ref quantizeParams) == 0;
        }

        /// <summary>
        /// Quantize the model.
        /// </summary>
        /// <param name="srcFileName">The model file to be quantized.</param>
        /// <param name="dstFilename">The path to save the quantized model.</param>
        /// <param name="ftype">The type of quantization.</param>
        /// <param name="nthread">Thread to be used during the quantization. By default it's the physical core number.</param>
        /// <param name="allowRequantize"></param>
        /// <param name="quantizeOutputTensor"></param>
        /// <returns>Whether the quantization is successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Quantize(string srcFileName, string dstFilename, string ftype, int nthread = -1, bool allowRequantize = true,
                                    bool quantizeOutputTensor = false)
        {
            return Quantize(srcFileName, dstFilename, StringToFtype(ftype), nthread, allowRequantize, quantizeOutputTensor);
        }

        private static bool ValidateFtype(LLamaFtype ftype)
        {
            // Validation copies from here:
            // https://github.com/ggerganov/llama.cpp/blob/f7001ccc5aa359fcf41bba19d1c99c3d25c9bcc7/llama.cpp#L13450

            switch (ftype)
            {
                case LLamaFtype.MOSTLY_Q4_0:
                case LLamaFtype.MOSTLY_Q4_1:
                case LLamaFtype.MOSTLY_Q5_0:
                case LLamaFtype.MOSTLY_Q5_1:
                case LLamaFtype.MOSTLY_Q8_0:
                case LLamaFtype.MOSTLY_F16:
                case LLamaFtype.ALL_F32:

                case LLamaFtype.MOSTLY_Q2_K_S:
                case LLamaFtype.MOSTLY_Q2_K:

                case LLamaFtype.MOSTLY_IQ3_K_XS:
                case LLamaFtype.MOSTLY_Q3_K_S:
                case LLamaFtype.MOSTLY_Q3_K_M:
                case LLamaFtype.MOSTLY_Q3_K_L:

                case LLamaFtype.MOSTLY_Q4_K_S:
                case LLamaFtype.MOSTLY_Q4_K_M:

                case LLamaFtype.MOSTLY_Q5_K_S:
                case LLamaFtype.MOSTLY_Q5_K_M:

                case LLamaFtype.MOSTLY_Q6_K:

                case LLamaFtype.MOSTLY_IQ2_XXS:
                case LLamaFtype.MOSTLY_IQ2_XS:
                case LLamaFtype.MOSTLY_IQ2_S:
                case LLamaFtype.MOSTLY_IQ2_M:

                case LLamaFtype.MOSTLY_IQ3_XXS:

                case LLamaFtype.MOSTLY_IQ1_S:
                case LLamaFtype.MOSTLY_IQ1_M:

                case LLamaFtype.MOSTLY_IQ4_NL:
                case LLamaFtype.MOSTLY_IQ4_XS:

                case LLamaFtype.MOSTLY_IQ3_S:
                case LLamaFtype.MOSTLY_IQ3_M:
                    return true;

                case LLamaFtype.MOSTLY_Q4_1_SOME_F16:
                case LLamaFtype.GUESSED:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Parse a string into a LLamaFtype. This is a "relaxed" parsing, which allows any string which is contained within
        /// the enum name to be used.
        ///
        /// For example "Q5_K_M" will convert to "LLAMA_FTYPE_MOSTLY_Q5_K_M"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static LLamaFtype StringToFtype(string str)
        {
            // Find all variants which contain the input string
            var matches = new List<LLamaFtype>();
            foreach (LLamaFtype ftype in Enum.GetValues(typeof(LLamaFtype)))
            {
                var name = Enum.GetName(typeof(LLamaFtype), ftype);
                
                // Note: this is using "IndexOf" instead of "Contains" to be compatible with netstandard2.0
#pragma warning disable CA2249
                if (name != null && name.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                    matches.Add(ftype);
#pragma warning restore CA2249
            }

            // If there was just one match, success!
            if (matches.Count == 1)
                return matches[0];

            // If none matched throw a generic error
            if (matches.Count == 0)
                throw new ArgumentException($"Unknown ftype \"{str}\" for quantization.");

            // There were several matches, throw an error asking the user to be more specific
            throw new ArgumentException($"\"{str}\" matches multiple potential ftypes: {string.Join(",", matches)}");
        }
    }
}
