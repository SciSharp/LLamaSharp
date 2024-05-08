using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Benchmark
{
    internal static class Constants
    {
        public static string ModelDir 
        {
            get
            {
                var res = Environment.GetEnvironmentVariable("LLAMA_BENCHMARK_MODEL_DIR") ?? "";
                Console.WriteLine($"+++*************** Got env path: {res}");
                return res;
            }
        }

        public readonly static string Generative7BModelPath = "llama-2-7b-chat.Q3_K_S.gguf";
        public readonly static string EmbeddingModelPath = "all-MiniLM-L12-v2.Q8_0.gguf";

        public readonly static string LLavaModelPath = "llava-v1.6-mistral-7b.Q3_K_XS.gguf";
        public readonly static string LLavaMmpPath = "mmproj-model-f16.gguf";
        public readonly static string LLavaImage = "Assets/extreme-ironing-taxi-610x427.jpg";

        public readonly static string TextCompletionPromptsFilePath = "Assets/TextCompletionPrompts.txt";
    }
}
