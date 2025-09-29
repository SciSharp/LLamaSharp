using System.Runtime.InteropServices;

namespace LLama.Unittest
{
    internal static class Constants
    {
        public static readonly string GenerativeModelPath = "Models/Llama-3.2-1B-Instruct-Q4_0.gguf";
        public static readonly string GenerativeModelPath2 = "Models/smollm-360m-instruct-add-basics-q8_0.gguf";
        public static readonly string EmbeddingModelPath = "Models/all-MiniLM-L12-v2.Q8_0.gguf";
        public static readonly string RerankingModelPath = "Models/jina-reranker-v1-tiny-en-FP16.gguf";

        public static readonly string MtmdModelPath = "Models/gemma-3-4b-it-Q4_K_M.gguf";
        public static readonly string MtmdMmpPath = "Models/gemma-mmproj-model-f16.gguf";
        public static readonly string MtmdImage = "Models/extreme-ironing-taxi-610x427.jpg";

        /// <summary>
        /// Calculate GpuLayer Count to use in UnitTest
        /// </summary>
        /// <returns> Defaults to 20 in all the cases, except MacOS/OSX release (to disable METAL on github CI)</returns>
        public static int CIGpuLayerCount
        {
            get
            {
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    #if DEBUG
                      return 20;
                    #else
                      return 0;                      
                    #endif
                }
                //else return 20;
            }
        }
    }
}
