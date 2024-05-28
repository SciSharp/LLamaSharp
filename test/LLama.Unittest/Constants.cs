using System.Runtime.InteropServices;

namespace LLama.Unittest
{
    internal static class Constants
    {
        public static readonly string GenerativeModelPath = "Models/llama-2-7b-chat.Q3_K_S.gguf";
        public static readonly string EmbeddingModelPath = "Models/all-MiniLM-L12-v2.Q8_0.gguf";

        public static readonly string LLavaModelPath = "Models/llava-v1.6-mistral-7b.Q3_K_XS.gguf";
        public static readonly string LLavaMmpPath = "Models/mmproj-model-f16.gguf";
        public static readonly string LLavaImage = "Models/extreme-ironing-taxi-610x427.jpg";

        /// <summary>
        /// Calculate GpuLayer Count to use in UnitTest
        /// </summary>
        /// <returns> Defaults to 20 in all the cases, except MacOS/OSX release (to disable METAL on github CI)</returns>
        public static int CIGpuLayerCount
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    #if DEBUG
                      return 20;
                    #else
                      return 0;                      
                    #endif
                }
                else return 20;
            }
        }
    }
}
