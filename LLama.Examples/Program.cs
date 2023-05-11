using LLama;
using LLama.Examples;
using LLama.Types;

int choice = 3;

if(choice == 0)
{
    ChatSession chat = new(@"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin", @"D:\development\llama\llama.cpp\prompts\chat-with-bob.txt", new string[] { "User:" });
    chat.Run();
}
else if(choice == 1)
{
    ChatWithLLamaModel chat = new(@"<Your model file path>", "<Your prompt file path>", new string[] { "User:" });
    chat.Run();
}
else if(choice == 2)
{
    ChatWithLLamaModelV1 chat = new(@"<Your model file path>");
    chat.Run();
}
else if (choice == 3) // quantization
{
    Quantize q = new Quantize();
    q.Run(@"D:\development\llama\weights\LLaMA\7B\ggml-model-f16.bin",
        @"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_1.bin", "q4_1");
}