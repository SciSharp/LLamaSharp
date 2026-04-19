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
        var vocab = executor.Context.NativeHandle.ModelHandle.Vocab;
        
        // Print some info
        var name = model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // A set of questions to evaluate all at once
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
            "Wy ae cts btr tn dgs?"
        };

        // Create a "Conversation" for each question
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
            // - BOS is the "Beginning of Sequence" token and should be included at the start of any prompt
            // - Special tokens are special non-text tokens which an LLM is trained to understand (e.g. BOS). The templated text may contains special tokens.
            var conversation = executor.Create();
            conversation.Prompt(executor.Context.Tokenize(templatedMessage, addBos: true, special: true));
            
            // Store everything we need to process this conversation
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
            // Enter a loop generating tokens
            for (var i = 0; i < TokenCount; i++)
            {
                // Run inference for all conversations in the batch which have pending tokens.
                var decodeResult = await executor.Infer();

                // Inference can fail, always check the return value!
                // NoKvSlot is not a fatal error, it just means that there's not enough memory available in the KV cache to process everything. You can force
                // this to happen by setting a small value for ContextSize in the ModelParams at the top of this file (e.g. 512).
                // In this case it's handled by ending a conversation (which will free up some space) and trying again. You could also handle this by
                // saving the conversation to disk and loading it up again later once some other conversations have finished.
                if (decodeResult == DecodeResult.NoKvSlot)
                {
                    conversations.FirstOrDefault(a => !a.IsComplete)?.MarkComplete(failed:true);
                    continue;
                }

                // A generic error, this is fatal and the batch can no longer be used. This should never occur and generally indicates
                // a bug in LLamaSharp, llama.cpp or a hardware error.
                if (decodeResult != DecodeResult.Ok)
                    throw new Exception($"Error occurred while inferring: {decodeResult}");
                
                // After inference all of the conversations must be sampled before running inference again.
                foreach (var conversationData in conversations)
                {
                    // Completed conversations don't need sampling.
                    if (conversationData.IsComplete)
                        continue;

                    // If the conversation wasn't prompted before the last call to Infer then it won't need sampling.
                    if (!conversationData.Conversation.RequiresSampling)
                        continue;

                    // Use the sampling pipeline to choose a single token for this conversation.
                    var token = conversationData.Conversation.Sample(conversationData.Sampler);

                    // Some special tokens indicate that this sequence has ended. Check if that's what has been chosen by the sampling pipeline.
                    if (token.IsEndOfGeneration(vocab))
                    {
                        conversationData.MarkComplete();
                    }
                    else
                    {
                        // It isn't the end of generation, so add this token to the decoder and then add that to our tracked data
                        conversationData.Decoder.Add(token);
                        conversationData.AppendAnswer(conversationData.Decoder.Read().ReplaceLineEndings(" "));
                        
                        // Prompt the conversation with this token, ready for the next round of inference to generate another token
                        conversationData.Conversation.Prompt(token);
                    }
                }
                
                // Render the current state
                table = BuildTable(conversations);
                ctx.UpdateTarget(table);
 
                if (conversations.All(c => c.IsComplete))
                    break;
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

    public string AnswerMarkdown =>
        IsComplete
            ? $"[{(IsFailed ? "red" : "green")}]{_inProgressAnswer.Message.EscapeMarkup()}{_inProgressAnswer.LatestToken.EscapeMarkup()}[/]"
            : $"[grey]{_inProgressAnswer.Message.EscapeMarkup()}[/][white]{_inProgressAnswer.LatestToken.EscapeMarkup()}[/]";

    public bool IsComplete { get; private set; }
    public bool IsFailed { get; private set; }
    
    // we are only keeping track of the answer in two parts to render them differently.
    private (string Message, string LatestToken) _inProgressAnswer = (string.Empty, string.Empty);
    
    public void AppendAnswer(string newText) => _inProgressAnswer = (_inProgressAnswer.Message + _inProgressAnswer.LatestToken, newText);

    public void MarkComplete(bool failed = false)
    {
        IsComplete = true;
        IsFailed = failed;
        if (Conversation.IsDisposed == false)
        {
            // clean up the conversation and sampler to release more memory for inference. 
            // real life usage would protect against these two being referenced after being disposed.
            Conversation.Dispose();
            Sampler.Dispose();
        }
    }
}