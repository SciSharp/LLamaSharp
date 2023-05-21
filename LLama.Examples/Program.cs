using LLama;
using LLama.Examples;

Console.WriteLine("================LLamaSharp Examples==================\n");

Console.WriteLine("Please input a number to choose an example to run:");
Console.WriteLine("0: Run a chat session.");
Console.WriteLine("1: Run a LLamaModel to chat.");
Console.WriteLine("2: Quantize a model.");
Console.WriteLine("3: Get the embeddings of a message.");
Console.WriteLine("4: Run a LLamaModel with instruct mode.");
Console.WriteLine("5: Load and save state of LLamaModel.");


while (true)
{
    Console.Write("\nYour choice: ");
    int choice = int.Parse(Console.ReadLine());

    if (choice == 0)
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
        ChatSession chat = new(modelPath, "Assets/chat-with-bob.txt", new string[] { "User:" });
        chat.Run();
    }
    else if (choice == 1)
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
        ChatWithLLamaModel chat = new(modelPath, "Assets/chat-with-bob.txt", new string[] { "User:" });
        chat.Run();
    }
    else if (choice == 2) // quantization
    {
        Console.Write("Please input your original model path: ");
        var inputPath = Console.ReadLine();
        Console.Write("Please input your output model path: ");
        var outputPath = Console.ReadLine();
        Console.Write("Please input the quantize type (one of q4_0, q4_1, q5_0, q5_1, q8_0): ");
        var quantizeType = Console.ReadLine();
        Quantize q = new Quantize();
        q.Run(inputPath, outputPath, quantizeType);
    }
    else if (choice == 3) // get the embeddings only
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
        GetEmbeddings em = new GetEmbeddings(modelPath);
        Console.Write("Please input the text: ");
        var text = Console.ReadLine();
        em.Run(text);
    }
    else if (choice == 4) // instruct mode
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
        InstructMode im = new InstructMode(modelPath, "Assets/alpaca.txt");
        Console.WriteLine("Here's a simple example for using instruct mode. You can input some words and let AI " +
            "complete it for you. For example: Write a story about a fox that wants to make friend with human. No less than 200 words.");
        im.Run();
    }
    else if (choice == 5) // load and save state
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
        Console.Write("Please input your state file path: ");
        var statePath = Console.ReadLine();
        SaveAndLoadState sals = new(modelPath, File.ReadAllText(@"D:\development\llama\llama.cpp\prompts\alpaca.txt"));
        sals.Run("Write a story about a fox that wants to make friend with human. No less than 200 words.");
        sals.SaveState(statePath);
        sals.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // create a new model to load the state.
        SaveAndLoadState sals2 = new(modelPath, "");
        sals2.LoadState(statePath);
        sals2.Run("Tell me more things about the fox in the story you told me.");
    }
    else
    {
        Console.WriteLine("Cannot parse your choice. Please select again.");
        continue;
    }
    break;
}