using LLama;
using LLama.Examples;
using LLama.Types;

int choice = 0;

if(choice == 0)
{
    ChatSession chat = new(@"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin", "Assets/chat-with-bob.txt", new string[] { "User:" });
    chat.Run();
}
else if(choice == 1)
{
    ChatWithLLamaModel chat = new(@"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin", "Assets/chat-with-bob.txt", new string[] { "User:" });
    chat.Run();
}
else if(choice == 2)
{
    ChatWithLLamaModelV1 chat = new(@"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin");
    chat.Run();
}