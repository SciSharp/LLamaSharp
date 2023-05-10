using LLama;
using LLama.Types;

//string modelPath = @"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin";
//LLamaModel model = new(modelPath, logits_all: false, verbose: false, n_ctx: 2048);
//List<ChatCompletionMessage> chats = new List<ChatCompletionMessage>();
//chats.Add(new ChatCompletionMessage("user", "Hi, Alice, I'm Rinne.", null));
//chats.Add(new ChatCompletionMessage("assistant", "Hi, Rinne, I'm Alice. What can I do for you?", null));
//Console.Write("You: ");
//var question = "This is a text classification task, below are the category list:\r\n1. Air Handler\r\n2. Tub/Shower\r\n3. Fireplace\r\n4. Bathroom\r\n5. Kitchen\r\n6. Powerwash roof eves and soffits\r\n\r\nFor example:\r\n1. \"Clear drain clog at kitchen sink\": Kitchen\r\n2. \"Change blower motor speed\": Air Handler\r\n3. \"Clear drain clog at tub/shower\": Bathroom\r\n4. \"Clear drain clog at toilet\": Bathroom\r\n\r\nPlease classify this text \"toilet clogged\" in provided list. output in json format: {\"category\": \"\", \"confidence\":0.0}";
//chats.Add(new ChatCompletionMessage("user", question, null));
//var output = model.CreateChatCompletion(chats, max_tokens: 1024);
//Console.WriteLine($"LLama AI: {output.Choices[0].Message.Content}");

string modelPath = @"D:\development\llama\weights\LLaMA\7B\ggml-model-q4_0.bin";
GptModel model = new(new GptParams(model: modelPath, n_ctx: 512, interactive: true, antiprompt: new List<string>(){"User:"}, 
    repeat_penalty: 1.0f));
model = model.WithPrompt("Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:");
while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    var question = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.White;
    var outputs = model.Call(question);
    foreach (var output in outputs)
    {
        Console.Write(output);
    }
}