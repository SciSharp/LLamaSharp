# Coding Assistant

```cs
using LLama.Common;
using System;
using System.Reflection;

internal class CodingAssistant
{
    const string DefaultModelUri = "https://huggingface.co/TheBloke/CodeLlama-7B-Instruct-GGUF/resolve/main/codellama-7b-instruct.Q4_K_S.gguf";

    // Source paper with example prompts:
    // https://doi.org/10.48550/arXiv.2308.12950
    const string InstructionPrefix = "[INST]";
    const string InstructionSuffix = "[/INST]";
    const string SystemInstruction = "You're an intelligent, concise coding assistant. Wrap code in ``` for readability. Don't repeat yourself. Use best practice and good coding standards.";
    private static string ModelsDirectory = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Models");

    public static async Task Run()
    {
        Console.Write("Please input your model path (if left empty, a default model will be downloaded for you): ");
        var modelPath = Console.ReadLine();

        if(string.IsNullOrWhiteSpace(modelPath) )
        {
            modelPath = await GetDefaultModel();
        }

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InstructExecutor(context, InstructionPrefix, InstructionSuffix, null);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions." +
            "\nIt's a 7B Code Llama, so it's trained for programming tasks like \"Write a C# function reading a file name from a given URI\" or \"Write some programming interview questions\"." +
            "\nWrite 'exit' to exit");
        Console.ForegroundColor = ConsoleColor.White;

        var inferenceParams = new InferenceParams() { 
            Temperature = 0.8f, 
            MaxTokens = -1,
        };

        string instruction = $"{SystemInstruction}\n\n";
        await Console.Out.WriteAsync("Instruction: ");
        instruction += Console.ReadLine() ?? "Ask me for instructions.";
        while (instruction != "exit")
        {

            Console.ForegroundColor = ConsoleColor.Green;
            await foreach (var text in executor.InferAsync(instruction + System.Environment.NewLine, inferenceParams))
            {
                Console.Write(text);
            }
            Console.ForegroundColor = ConsoleColor.White;

            await Console.Out.WriteAsync("Instruction: ");
            instruction = Console.ReadLine() ?? "Ask me for instructions.";
        }
    }

    private static async Task<string> GetDefaultModel()
    {
        var uri = new Uri(DefaultModelUri);
        var modelName = uri.Segments[^1];
        await Console.Out.WriteLineAsync($"The following model will be used: {modelName}");
        var modelPath = Path.Combine(ModelsDirectory, modelName);
        if(!Directory.Exists(ModelsDirectory))
        {
            Directory.CreateDirectory(ModelsDirectory);
        }

        if (File.Exists(modelPath))
        {
            await Console.Out.WriteLineAsync($"Existing model found, using {modelPath}");
        }
        else
        {
            await Console.Out.WriteLineAsync($"Model not found locally, downloading {DefaultModelUri}...");
            using var http = new HttpClient();
            await using var downloadStream = await http.GetStreamAsync(uri);
            await using var fileStream = new FileStream(modelPath, FileMode.Create, FileAccess.Write);
            await downloadStream.CopyToAsync(fileStream);
            await Console.Out.WriteLineAsync($"Model downloaded and saved to {modelPath}");
        }


        return modelPath;
    }
}


```