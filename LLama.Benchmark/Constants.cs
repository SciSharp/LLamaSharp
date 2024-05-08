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
        public const string ModelDir = "/llamasharp_ci/models_benchmark";

        public const string Generative7BModelPath = "llama-2-7b-chat.Q3_K_S.gguf";
        public const string EmbeddingModelPath = "all-MiniLM-L12-v2.Q8_0.gguf";

        public const string LLavaModelPath = "llava-v1.6-mistral-7b.Q3_K_XS.gguf";
        public const string LLavaMmpPath = "mmproj-model-f16.gguf";
        public const string LLavaImage = "Assets/extreme-ironing-taxi-610x427.jpg";

        public const string TextCompletionPromptsFilePath = "Assets/TextCompletionPrompts.txt";
    }
}
