using LLama;
using LLama.Examples;
using LLama.Types;

int choice = 0;

if(choice == 0)
{
    ChatSession chat = new(@"/home/rinne/downloads/ggml-model-q4_0.bin", $"{AppDomain.CurrentDomain.BaseDirectory}/Assets/chat-with-bob.txt", new string[] { "User:" });
    chat.Run();
}
else if(choice == 1)
{
    ChatWithLLamaModel chat = new(@"/home/rinne/downloads/ggml-model-q4_0.bin", $"{AppDomain.CurrentDomain.BaseDirectory}/Assets/chat-with-bob.txt", new string[] { "User:" });
    chat.Run();
}
else if(choice == 2)
{
    ChatWithLLamaModelV1 chat = new(@"/home/rinne/downloads/ggml-model-q4_0.bin");
    chat.Run();
}