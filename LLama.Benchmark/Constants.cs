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
        public static string ModelDir { get; set; } = "";

        public static string Generative7BModelPath => Path.Combine(ModelDir, "llama-2-7b-chat.Q3_K_S.gguf");
        public static string EmbeddingModelPath => Path.Combine(ModelDir, "all-MiniLM-L12-v2.Q8_0.gguf");

        public static string LLavaModelPath => Path.Combine(ModelDir, "llava-v1.6-mistral-7b.Q3_K_XS.gguf");
        public static string LLavaMmpPath => Path.Combine(ModelDir, "mmproj-model-f16.gguf");
        public static string LLavaImage => "Assets/extreme-ironing-taxi-610x427.jpg";

        public static string TextCompletionPromptsFilePath = "Assets/TextCompletionPrompts.txt";
    }
}
