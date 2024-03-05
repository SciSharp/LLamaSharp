// See https://aka.ms/new-console-template for more information
using LLama;
using LLama.Common;
using LLama.Native;
using System.Runtime.CompilerServices;
using System.Text;



internal class Program
{
    private static void Main(string[] args)
    {
        Test();
    }

    static void Test()
    {
        string modelPath = @".\model\llava-v1.5-7b-Q4_K.gguf";
        string clipModelPath = @".\model\llava-v1.5-7b-mmproj-Q4_0.gguf";
        string imagePath = @".\image\demo.jpg";
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Seed = 1337,
            GpuLayerCount = 32,
            Encoding = Encoding.UTF8,
        };

        byte[] bytes = File.ReadAllBytes(imagePath);
        SafeLlavaModelHandle model = SafeLlavaModelHandle.LoadModel(parameters, clipModelPath);
        int tokenCount = model.EvalPrompts("describe the image.", bytes);
        InferenceParams inferenceParams = new InferenceParams() { Temperature = 0.2f };
        model.Infer(inferenceParams, tokenCount);
        model.Dispose();
        Console.WriteLine();
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}