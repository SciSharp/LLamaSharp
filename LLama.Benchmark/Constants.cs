
namespace LLama.Benchmark
{
    internal static class Constants
    {
        public static string ModelDir
        {
            get
            {
                return Environment.GetEnvironmentVariable("BENCHMARK_MODEL_DIR") ?? "";
            }
        }

        public static string Generative7BModelPath =>  Path.Combine(ModelDir, "llama-2-7b-chat.Q3_K_S.gguf");
        public static string EmbeddingModelPath => Path.Combine(ModelDir, "all-MiniLM-L12-v2.Q8_0.gguf");

        public static string LLavaModelPath => Path.Combine("llava-v1.6-mistral-7b.Q3_K_XS.gguf");
        public static string LLavaMmpPath => Path.Combine("mmproj-model-f16.gguf");
        public static string LLavaImage => "Assets/extreme-ironing-taxi-610x427.jpg";

        public static string TextCompletionPromptsFilePath => "Assets/TextCompletionPrompts.txt";
    }
}
