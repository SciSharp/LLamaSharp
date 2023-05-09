using LLama;
using LLama.Types;

string modelPath = @"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin";
LLamaModel model = new(modelPath, logits_all: false, verbose: false);
List<ChatCompletionMessage> chats = new List<ChatCompletionMessage>();
chats.Add(new ChatCompletionMessage("user", "Hi, Alice, I'm Rinne.", null));
chats.Add(new ChatCompletionMessage("assistant", "Hi, Rinne, I'm Alice. What can I do for you?", null));
while (true)
{
    Console.Write("You: ");
    var question = Console.ReadLine();
    chats.Add(new ChatCompletionMessage("user", question, null));
    var output = model.CreateChatCompletion(chats, max_tokens: 256);
    Console.WriteLine($"LLama AI: {output.Choices[0].Message.Content}");
}