using LLama.Common;
using LLamaSharp.SemanticKernel;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;
using AuthorRole = Microsoft.SemanticKernel.ChatCompletion.AuthorRole;
using ChatHistory = Microsoft.SemanticKernel.ChatCompletion.ChatHistory;

namespace LLama.Examples.Examples
{
    public class SemanticKernelHomeAutomation
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This example was inspired by the HomeAutomation example in SemanticKernel.");
            Console.ForegroundColor = ConsoleColor.White;

            string modelPath = UserSettings.GetModelPath();

            // Load weights into memory
            var parameters = new ModelParams(modelPath);
            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            var slex = new StatelessExecutor(model, parameters);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            // Actual code to execute is found in Worker class
            builder.Services.AddHostedService<Worker>();

            builder.Services.AddSingleton<IChatCompletionService>(sp => 
            { 
                return new LLamaSharpChatCompletion(slex, 
                    new LLamaSharpPromptExecutionSettings()
                        {
                            MaxTokens = -1,
                            Temperature = 0,
                            TopP = 0.1,
                        });
            });

            // Add plugins that can be used by kernels
            // The plugins are added as singletons so that they can be used by multiple kernels
            builder.Services.AddKeyedSingleton<MyLightPlugin>("OfficeLight");

            // Add a home automation kernel to the dependency injection container
            builder.Services.AddKeyedTransient<Kernel>("HomeAutomationKernel", (sp, key) =>
            {
                // Create a collection of plugins that the kernel will use
                KernelPluginCollection pluginCollection = [];
                pluginCollection.AddFromObject(sp.GetRequiredKeyedService<MyLightPlugin>("OfficeLight"), "OfficeLight");

                // When created by the dependency injection container, Semantic Kernel logging is included by default
                return new Kernel(sp, pluginCollection);
            });

            //remove logging
            builder.Services.AddSingleton<ILoggerFactory>(sp => { return NullLoggerFactory.Instance; });

            using IHost host = builder.Build();

            await host.RunAsync();
        }
    }

    class Worker(
        IHostApplicationLifetime hostApplicationLifetime,
        [FromKeyedServices("HomeAutomationKernel")] Kernel kernel) : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime;
        private readonly Kernel _kernel = kernel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Get chat completion service
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            Console.WriteLine("Ask questions or give instructions to the copilot such as:\n" +
                              "- Turn on the light.\n" +
                              "- Which light is currently on?\n");

            Console.Write("> ");

            LLamaSharpPromptExecutionSettings llamaSharpPromptExecutionSettings = new()
            {
                 Temperature = 0.0f,
                 TopP = 0.1f
            };

            string? input = null;

            while ((input = Console.ReadLine()) != null)
            {
                ChatHistory chatHistory = new ChatHistory();
                chatHistory.Add(new ChatMessageContent(AuthorRole.System,
                                """
                You are an assistant who determines the user's intent. Here are the possible intents:                
                - If the user wants to turn the light ON, then answer with the following:
                ´´´answer
                [TURN ON THE LIGHT]
                ´´´
                - If the user wants to turn the light OFF, then answer with the following:
                ´´´answer
                [TURN OFF THE LIGHT]
                ´´´
                - If the user wants to know which light is on currently, then answer with the following:
                ´´´answer
                [WHICH LIGHT IS ON]
                ´´´
                IMPORTANT: only return the answer without further comments with the format ´´´answer{intent}´´´, where {intent} is the user's intent.
                """
                ));

                Console.WriteLine();

                chatHistory.Add(new ChatMessageContent(AuthorRole.User, input));

                ChatMessageContent chatResult = await chatCompletionService.GetChatMessageContentAsync(chatHistory, llamaSharpPromptExecutionSettings, _kernel, stoppingToken);

                FunctionResult? fres = null;
                if (chatResult.Content.Contains("[TURN ON THE LIGHT]"))
                {
                    fres = await _kernel.InvokeAsync("OfficeLight", "TurnOn");
                }
                else if (chatResult.Content.Contains("[TURN OFF THE LIGHT]"))
                {
                    fres = await _kernel.InvokeAsync("OfficeLight", "TurnOff");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                if (fres != null || chatResult.Content.Contains("[WHICH LIGHT IS ON]"))
                {
                    fres = await _kernel.InvokeAsync("OfficeLight", "IsTurnedOn");
                    Console.Write($">>> Result:\n {(fres.GetValue<bool>()==true?"The light is ON.": "The light is OFF.")}\n\n> ");
                }
                else
                {
                    Console.Write($">>> Result: {chatResult}\n\n> ");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            _hostApplicationLifetime.StopApplication();
        }

    }

    /// <summary>
    /// Class that represents a controllable light.
    /// </summary>
    [Description("Represents a light")]
    class MyLightPlugin(bool turnedOn = false)
    {
        private bool _turnedOn = turnedOn;

        [KernelFunction, Description("Returns whether this light is on")]
        public bool IsTurnedOn() => _turnedOn;

        [KernelFunction, Description("Turn on this light")]
        public void TurnOn() => _turnedOn = true;

        [KernelFunction, Description("Turn off this light")]
        public void TurnOff() => _turnedOn = false;
    }
}
