using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Common;

namespace LLama.Examples.Examples
{
    public class ChatChineseGB2312
    {
        private static string ConvertFromEncodingToAnother(string input, Encoding original, Encoding target)
        {
            byte[] bytes = original.GetBytes(input);
            var convertedBytes = Encoding.Convert(original, target, bytes);
            return target.GetString(convertedBytes);
        }

        public static async Task Run()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Register gb2312 encoding
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();
            var prompt = File.ReadAllText("Assets/chat-with-kunkun-chinese.txt", encoding: Encoding.GetEncoding("gb2312")).Trim();
            prompt = ConvertFromEncodingToAnother(prompt, Encoding.GetEncoding("gb2312"), Encoding.UTF8);

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 20,
                Encoding = Encoding.UTF8
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InteractiveExecutor(context);

            var session = new ChatSession(executor).WithHistoryTransform(new LLamaTransforms.DefaultHistoryTransform("用户"));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This example shows how to use Chinese with gb2312 encoding, which is common in windows. It's recommended" +
                " to use https://huggingface.co/hfl/chinese-alpaca-2-7b-gguf/blob/main/ggml-model-q5_0.gguf, which has been verified by LLamaSharp developers.");
            Console.ForegroundColor = ConsoleColor.White;

            // show the prompt
            Console.Write(prompt);
            while (true)
            {
                await foreach (var text in session.ChatAsync(prompt, new InferenceParams()
                {
                    Temperature = 0.3f,
                    TopK = 5,
                    TopP = 0.85f,
                    AntiPrompts = new List<string> { "用户：" },
                    MaxTokens = 2048,
                    RepeatPenalty = 1.05f
                }))
                {
                    //Console.Write(text);
                    Console.Write(ConvertFromEncodingToAnother(text, Encoding.UTF8, Encoding.GetEncoding("gb2312")));
                }

                Console.ForegroundColor = ConsoleColor.Green;
                prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
