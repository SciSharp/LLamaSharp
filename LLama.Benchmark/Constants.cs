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
        public static readonly string Generative7BModelPath = "Models/llama-2-7b-chat.Q3_K_S.gguf";
        public static readonly string EmbeddingModelPath = "Models/all-MiniLM-L12-v2.Q8_0.gguf";

        public static readonly string LLavaModelPath = "Models/llava-v1.6-mistral-7b.Q3_K_XS.gguf";
        public static readonly string LLavaMmpPath = "Models/mmproj-model-f16.gguf";
        public static readonly string LLavaImage = "Models/extreme-ironing-taxi-610x427.jpg";

        public static readonly string TextCompletionPromptsFilePath = "Assets/TextCompletionPrompts.txt";
    }
}