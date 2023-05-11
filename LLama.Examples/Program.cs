using LLama;
using LLama.Examples;
using LLama.Types;

int choice = 0;

if(choice == 0)
{
    ChatSession chat = new(@"<Your model file path>", "<Your prompt file path>", new string[] { "User:" });
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