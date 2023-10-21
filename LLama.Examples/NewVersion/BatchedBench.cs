using LLama.Common;
using LLama.Native;

namespace LLama.Examples.NewVersion;

public class BatchedBench
{
    public static async Task Run()
    {
        Console.Write("Please input your model path: ");
        //todo:var modelPath = Console.ReadLine();
        var modelPath = @"C:\Users\Martin\Documents\Python\oobabooga_windows\text-generation-webui\models\llama-2-7b-chat.Q5_K_M.gguf";

        var parameters = new ModelParams(modelPath);
        using var model = LLamaWeights.LoadFromFile(parameters);

        parameters.ContextSize = (uint)model.ContextSize;
        using var context = model.CreateContext(parameters);

        var n_kv_max = 1024;

        using var batch = LLamaBatchSafeHandle.Create(n_kv_max, 0, 1);

    }
}