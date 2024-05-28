using LLama.Abstractions;
using LLama.Common;

namespace LLama.Examples.Examples;

// When using chatsession, it's a common case that you want to strip the role names
// rather than display them. This example shows how to use transforms to strip them.
public class LLama3ChatSession
{
    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath)
        {
            Seed = 1337,
            GpuLayerCount = 10
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        var chatHistoryJson = File.ReadAllText("Assets/chat-with-bob.json");
        ChatHistory chatHistory = ChatHistory.FromJson(chatHistoryJson) ?? new ChatHistory();

        ChatSession session = new(executor, chatHistory);
        session.WithHistoryTransform(new LLama3HistoryTransform());
        session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
            new string[] { "User:", "Assistant:", "�" },
            redundancyLength: 5));

        InferenceParams inferenceParams = new InferenceParams()
        {
            Temperature = 0.6f,
            AntiPrompts = new List<string> { "User:" }
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            await foreach (
                var text
                in session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, userInput),
                    inferenceParams))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(text);
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            userInput = Console.ReadLine() ?? "";

            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    class LLama3HistoryTransform : IHistoryTransform
    {
        /// <summary>
        /// Convert a ChatHistory instance to plain text.
        /// </summary>
        /// <param name="history">The ChatHistory instance</param>
        /// <returns></returns>
        public string HistoryToText(ChatHistory history)
        {
            string res = Bos;
            foreach (var message in history.Messages)
            {
                res += EncodeMessage(message);
            }
            res += EncodeHeader(new ChatHistory.Message(AuthorRole.Assistant, ""));
            return res;
        }

        private string EncodeHeader(ChatHistory.Message message)
        {
            string res = StartHeaderId;
            res += message.AuthorRole.ToString();
            res += EndHeaderId;
            res += "\n\n";
            return res;
        }

        private string EncodeMessage(ChatHistory.Message message)
        {
            string res = EncodeHeader(message);
            res += message.Content;
            res += EndofTurn;
            return res;
        }

        /// <summary>
        /// Converts plain text to a ChatHistory instance.
        /// </summary>
        /// <param name="role">The role for the author.</param>
        /// <param name="text">The chat history as plain text.</param>
        /// <returns>The updated history.</returns>
        public ChatHistory TextToHistory(AuthorRole role, string text)
        {
            return new ChatHistory(new ChatHistory.Message[] { new ChatHistory.Message(role, text) });
        }

        /// <summary>
        /// Copy the transform.
        /// </summary>
        /// <returns></returns>
        public IHistoryTransform Clone()
        {
            return new LLama3HistoryTransform();
        }

        private const string StartHeaderId = "<|start_header_id|>";
        private const string EndHeaderId = "<|end_header_id|>";
        private const string Bos = "<|begin_of_text|>";
        private const string Eos = "<|end_of_text|>";
        private const string EndofTurn = "<|eot_id|>";
    }
}
