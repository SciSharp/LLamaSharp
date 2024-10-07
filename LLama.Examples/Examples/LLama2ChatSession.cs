using LLama.Abstractions;
using LLama.Common;
using LLama.Sampling;
using System.Text;

namespace LLama.Examples.Examples;

/// <summary>
/// This sample shows a simple chatbot
/// It's configured to use custom prompt template as provided by llama.cpp and supports
/// models such as LLama 2 and Mistral Instruct
/// </summary>
public class LLama2ChatSession
{
    public static async Task Run()
    {
        var modelPath = UserSettings.GetModelPath();
        var parameters = new ModelParams(modelPath)
        {
            GpuLayerCount = 10
        };

        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        var chatHistoryJson = await File.ReadAllTextAsync("Assets/chat-with-bob.json");
        var chatHistory = ChatHistory.FromJson(chatHistoryJson) ?? new ChatHistory();

        ChatSession session = new(executor, chatHistory);

        // add custom templator 
        session.WithHistoryTransform(new Llama2HistoryTransformer());

        session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
            [model.Tokens.EndOfTurnToken ?? "User:", "�"],
            redundancyLength: 5));

        var inferenceParams = new InferenceParams
        {
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f
            },

            MaxTokens = -1, // keep generating tokens until the anti prompt is encountered
            AntiPrompts = [model.Tokens.EndOfTurnToken ?? "User:"] // model specific end of turn string (or default)
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("User> ");
        var userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Assistant> ");

            // as each token (partial or whole word is streamed back) print it to the console, stream to web client, etc
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
            Console.Write("User> ");
            userInput = Console.ReadLine() ?? "";
        }
    }

    /// <summary>
    /// Chat History transformer for Llama 2 family.
    /// https://huggingface.co/blog/llama2#how-to-prompt-llama-2
    /// </summary>
    public class Llama2HistoryTransformer : IHistoryTransform
    {
        public string Name => "Llama2";

        /// <inheritdoc/>
        public IHistoryTransform Clone()
        {
            return new Llama2HistoryTransformer();
        }

        /// <inheritdoc/>
        public string HistoryToText(ChatHistory history)
        {
            //More info on template format for llama2 https://huggingface.co/blog/llama2#how-to-prompt-llama-2
            //We don't have to insert <BOS> token for the first message, as it's done automatically by LLamaSharp.InteractExecutor and LLama.cpp
            //See more in https://github.com/ggerganov/llama.cpp/pull/7107
            if (history.Messages.Count == 0)
                return string.Empty;

            var builder = new StringBuilder(64 * history.Messages.Count);

            int i = 0;
            if (history.Messages[i].AuthorRole == AuthorRole.System)
            {
                builder.Append($"[INST] <<SYS>>\n").Append(history.Messages[0].Content.Trim()).Append("\n<</SYS>>\n");
                i++;

                if (history.Messages.Count > 1)
                {
                    builder.Append(history.Messages[1].Content.Trim()).Append(" [/INST]");
                    i++;
                }
            }

            for (; i < history.Messages.Count; i++)
            {
                if (history.Messages[i].AuthorRole == AuthorRole.User)
                {
                    builder.Append(i == 0 ? "[INST] " : "<s>[INST] ").Append(history.Messages[i].Content.Trim()).Append(" [/INST]");
                }
                else
                {
                    builder.Append(' ').Append(history.Messages[i].Content.Trim()).Append(" </s>");
                }
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
        public ChatHistory TextToHistory(AuthorRole role, string text)
        {
            return new ChatHistory([new ChatHistory.Message(role, text)]);
        }
    }
}
