using System.Diagnostics.CodeAnalysis;
using System.Text;
using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating multiple replies to the same prompt, with a shared cache
/// </summary>
public class BatchedExecutorSimple
{
    /// <summary>
    /// Set total length of the sequence to generate
    /// </summary>
    private const int TokenCount = 72;

    public static async Task Run()
    {
        // Load model weights
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);

        // Create an executor that can evaluate a batch of conversations together
        using var executor = new BatchedExecutor(model, parameters);
        
        // we'll need this for evaluating if we are at the end of generation
        var modelTokens = executor.Context.NativeHandle.ModelHandle.Tokens;
        
        // Print some info
        var name = model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        var messages = new[]
        {
            "What's 2+2?",
            "Where is the coldest part of Texas?",
            "What's the capital of France?",
            "What's a one word name for a food item with ground beef patties on a bun?",
            "What are two toppings for a pizza?",
            "What american football play are you calling on a 3rd and 8 from our own 25?",
            "What liquor should I add to egg nog?",
            "I have two sons, Bert and Ernie. What should I name my daughter?",
            "What day comes after Friday?",
            "What color shoes should I wear with dark blue pants?",
        };

        var conversations = new List<ConversationData>();
        foreach (var message in messages)
        {
            // apply the model's prompt template to our question and system prompt
            var template = new LLamaTemplate(model);
            template.Add("system", "I am a helpful bot that returns short and concise answers. I include a ten word description of my reasoning when I finish.");
            template.Add("user", message);
            template.AddAssistant = true;
            var templatedMessage = Encoding.UTF8.GetString(template.Apply());
            
            // create a new conversation and prompt it. include special and bos because we are using the template
            var conversation = executor.Create();
            conversation.Prompt(executor.Context.Tokenize(templatedMessage, addBos: true, special: true));
            
            conversations.Add(new ConversationData {
                Prompt = message,
                Conversation = conversation,
                Sampler = new GreedySamplingPipeline(),
                Decoder = new StreamingTokenDecoder(executor.Context)
            });
        }

        var table = BuildTable(conversations);
        await AnsiConsole.Live(table).StartAsync(async ctx =>
        {
            for (var i = 0; i < TokenCount; i++)
            {
                // Run inference for all conversations in the batch which have pending tokens.
                var decodeResult = await executor.Infer();
                if (decodeResult == DecodeResult.NoKvSlot)
                    throw new Exception("Could not find a KV slot for the batch. Try reducing the size of the batch or increase the context.");
                if (decodeResult == DecodeResult.Error)
                    throw new Exception("Unknown error occurred while inferring.");
                
                foreach (var conversationData in conversations.Where(c => c.IsComplete == false))
                {
                    if (conversationData.Conversation.RequiresSampling == false)
                        continue;
                
                    // sample a single token for the executor, passing the sample index of the conversation
                    var sampleIndex = conversationData.Conversation.GetSampleIndex();
                    var token = conversationData.Sampler.Sample(
                        executor.Context,
                        sampleIndex
                    );
                    
                    if (modelTokens.IsEndOfGeneration(token))
                    {
                        conversationData.MarkComplete();
                    }
                    else
                    {
                        // it isn't the end of generation, so add this token to the decoder and then add that to our tracked data
                        conversationData.Decoder.Add(token);
                        todo: conversationData.AppendAnswer(conversationData.Decoder.Read().ReplaceLineEndings(" "));
                        
                        // add the token to the conversation
                        conversationData.Conversation.Prompt(token);
                    }
                }
                
                // render the current state
                table = BuildTable(conversations);
                ctx.UpdateTarget(table);
 
                if (conversations.All(c => c.IsComplete))
                {
                    break;
                }
            }

            // if we ran out of tokens before completing just mark them as complete for rendering purposes.
            foreach (var data in conversations.Where(i => i.IsComplete == false))
            {
                data.MarkComplete();
            }
            
            table = BuildTable(conversations);
            ctx.UpdateTarget(table);
        });
    }

    /// <summary>
    /// Helper to build a table to display the conversations.
    /// </summary>
    private static Table BuildTable(List<ConversationData> conversations)
    {
        var table = new Table()
            .RoundedBorder()
            .AddColumns("Prompt", "Response");
        
        foreach (var data in conversations)
        {
            table.AddRow(data.Prompt.EscapeMarkup(), data.AnswerMarkdown);
        }

        return table;
    }
}

public class ConversationData
{
    public required string Prompt { get; init; } 
    public required Conversation Conversation { get; init; } 
    public required BaseSamplingPipeline Sampler { get; init; }
    public required StreamingTokenDecoder Decoder { get; init; } 

    public string AnswerMarkdown => IsComplete 
        ? $"[green]{_inProgressAnswer.Message.EscapeMarkup()}{_inProgressAnswer.LatestToken.EscapeMarkup()}[/]" 
        : $"[grey]{_inProgressAnswer.Message.EscapeMarkup()}[/][white]{_inProgressAnswer.LatestToken.EscapeMarkup()}[/]";

    public bool IsComplete { get; private set; }
    
    // we are only keeping track of the answer in two parts to render them differently.
    private (string Message, string LatestToken) _inProgressAnswer = (string.Empty, string.Empty);
    
    public void AppendAnswer(string newText) => _inProgressAnswer = (_inProgressAnswer.Message + _inProgressAnswer.LatestToken, newText);

    public void MarkComplete()
    {
        IsComplete = true;
        if (Conversation.IsDisposed == false)
        {
            // clean up the conversation and sampler to release more memory for inference. 
            // real life usage would protect against these two being referenced after being disposed.
            Conversation.Dispose();
            Sampler.Dispose();
        }
    }
}