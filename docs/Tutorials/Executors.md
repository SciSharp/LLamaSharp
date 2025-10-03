# LLamaSharp executors

LLamaSharp executor defines the behavior of the model when it is called. Currently, there are four kinds of executors, which are `InteractiveExecutor`, `InstructExecutor`, `StatelessExecutor` and `BatchedExecutor`.

In a word, `InteractiveExecutor` is suitable for getting answer of your questions from LLM continuously. `InstructExecutor` let LLM execute your instructions, such as "continue writing". `StatelessExecutor` is best for one-time job because the previous inference has no impact on the current inference. `BatchedExecutor` could accept multiple inputs and generate multiple outputs of different sessions at the same time, significantly improving the throughput of the program.

## Text-to-Text APIs of the executors

All the executors implements the interface `ILLamaExecutor`, which provides two APIs to execute text-to-text tasks.

```cs
public interface ILLamaExecutor
{
    /// <summary>
    /// The loaded context for this executor.
    /// </summary>
    public LLamaContext Context { get; }

    // LLava Section
    //
    /// <summary>
    /// Identify if it's a multi-modal model and there is a image to process.
    /// </summary>
    public bool IsMultiModal { get; }
    /// <summary>
    /// Multi-Modal Projections / Clip Model weights
    /// </summary>
    public LLavaWeights? ClipModel { get;  }        

    /// <summary>
    /// List of images: List of images in byte array format.
    /// </summary>
    public List<byte[]> Images { get; }


    /// <summary>
    /// Asynchronously infers a response from the model.
    /// </summary>
    /// <param name="text">Your prompt</param>
    /// <param name="inferenceParams">Any additional parameters</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns></returns>
    IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken token = default);
}
```

The output of both two APIs are **yield enumerable**. Therefore, when receiving the output, you can directly use `foreach` to take actions on each word you get by order, instead of waiting for the whole process completed.

## InteractiveExecutor & InstructExecutor

Both of them are taking "completing the prompt" as the goal to generate the response. For example, if you input `Long long ago, there was a fox who wanted to make friend with humen. One day`, then the LLM will continue to write the story.

Under interactive mode, you serve a role of user and the LLM serves the role of assistant. Then it will help you with your question or request. 

Under instruct mode, you give LLM some instructions and it follows.

Though the behaviors of them sounds similar, it could introduce many differences depending on your prompt. For example, "chat-with-bob" has good performance under interactive mode and `alpaca` does well with instruct mode.

```
// chat-with-bob

Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.

User: Hello, Bob.
Bob: Hello. How may I help you today?
User: Please tell me the largest city in Europe.
Bob: Sure. The largest city in Europe is Moscow, the capital of Russia.
User:
```

```
// alpaca

Below is an instruction that describes a task. Write a response that appropriately completes the request.
```

Therefore, please modify the prompt correspondingly when switching from one mode to the other.

## StatelessExecutor.

Despite the differences between interactive mode and instruct mode, both of them are stateful mode. That is, your previous question/instruction will impact on the current response from LLM. On the contrary, the stateless executor does not have such a "memory". No matter how many times you talk to it, it will only concentrate on what you say in this time. It is very useful when you want a clean context, without being affected by previous inputs.

Since the stateless executor has no memory of conversations before, you need to input your question with the whole prompt into it to get the better answer.

For example, if you feed `Q: Who is Trump? A: ` to the stateless executor, it may give the following answer with the antiprompt `Q: `.

```
Donald J. Trump, born June 14, 1946, is an American businessman, television personality, politician and the 45th President of the United States (2017-2021). # Anexo:Torneo de Hamburgo 2022 (individual masculino)

## Presentación previa

* Defensor del título:  Daniil Medvédev
```

It seems that things went well at first. However, after answering the question itself, LLM began to talk about some other things until the answer reached the token count limit. The reason of this strange behavior is the anti-prompt cannot be match. With the input, LLM cannot decide whether to append a string "A: " at the end of the response.

As an improvement, let's take the following text as the input:

```
Q: What is the capital of the USA? A: Washingtong. Q: What is the sum of 1 and 2? A: 3. Q: Who is Trump? A: 
```

Then, I got the following answer with the anti-prompt `Q: `.

```
45th president of the United States.
```

At this time, by repeating the same mode of `Q: xxx? A: xxx.`, LLM outputs the anti-prompt we want to help to decide where to stop the generation.

## BatchedExecutor

Different from other executors, `BatchedExecutor` could accept multiple inputs from different sessions and generate outputs for them at the same time. Here is an example to use it.

```cs
using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates using a batch to generate two sequences and then using one
/// sequence as the negative guidance ("classifier free guidance") for the other.
/// </summary>
public class BatchedExecutorGuidance
{
    private const int n_len = 32;

    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath);
        using var model = LLamaWeights.LoadFromFile(parameters);

        var positivePrompt = AnsiConsole.Ask("Positive Prompt (or ENTER for default):", "My favourite colour is").Trim();
        var negativePrompt = AnsiConsole.Ask("Negative Prompt (or ENTER for default):", "I hate the colour red. My favourite colour is").Trim();
        var weight = AnsiConsole.Ask("Guidance Weight (or ENTER for default):", 2.0f);

        // Create an executor that can evaluate a batch of conversations together
        using var executor = new BatchedExecutor(model, parameters);

        // Print some info
        var name = executor.Model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // Load the two prompts into two conversations
        using var guided = executor.Create();
        guided.Prompt(positivePrompt);
        using var guidance = executor.Create();
        guidance.Prompt(negativePrompt);

        // Run inference to evaluate prompts
        await AnsiConsole
             .Status()
             .Spinner(Spinner.Known.Line)
             .StartAsync("Evaluating Prompts...", _ => executor.Infer());

        // Fork the "guided" conversation. We'll run this one without guidance for comparison
        using var unguided = guided.Fork();

        // Run inference loop
        var unguidedSampler = new GuidedSampler(null, weight);
        var unguidedDecoder = new StreamingTokenDecoder(executor.Context);
        var guidedSampler = new GuidedSampler(guidance, weight);
        var guidedDecoder = new StreamingTokenDecoder(executor.Context);
        await AnsiConsole
           .Progress()
           .StartAsync(async progress =>
            {
                var reporter = progress.AddTask("Running Inference", maxValue: n_len);

                for (var i = 0; i < n_len; i++)
                {
                    if (i != 0)
                        await executor.Infer();

                    // Sample from the "unguided" conversation. This is just a conversation using the same prompt, without any
                    // guidance. This serves as a comparison to show the effect of guidance.
                    var u = unguidedSampler.Sample(executor.Context.NativeHandle, unguided.Sample(), Array.Empty<LLamaToken>());
                    unguidedDecoder.Add(u);
                    unguided.Prompt(u);

                    // Sample from the "guided" conversation. This sampler will internally use the "guidance" conversation
                    // to steer the conversation. See how this is done in GuidedSampler.ProcessLogits (bottom of this file).
                    var g = guidedSampler.Sample(executor.Context.NativeHandle, guided.Sample(), Array.Empty<LLamaToken>());
                    guidedDecoder.Add(g);

                    // Use this token to advance both guided _and_ guidance. Keeping them in sync (except for the initial prompt).
                    guided.Prompt(g);
                    guidance.Prompt(g);

                    // Early exit if we reach the natural end of the guided sentence
                    if (g == model.EndOfSentenceToken)
                        break;

                    // Update progress bar
                    reporter.Increment(1);
                }
            });

        AnsiConsole.MarkupLine($"[green]Unguided:[/][white]{unguidedDecoder.Read().ReplaceLineEndings(" ")}[/]");
        AnsiConsole.MarkupLine($"[green]Guided:[/][white]{guidedDecoder.Read().ReplaceLineEndings(" ")}[/]");
    }

    private class GuidedSampler(Conversation? guidance, float weight)
        : BaseSamplingPipeline
    {
        public override void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
        {
        }

        public override ISamplingPipeline Clone()
        {
            throw new NotSupportedException();
        }

        protected override void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
        {
            if (guidance == null)
                return;

            // Get the logits generated by the guidance sequences
            var guidanceLogits = guidance.Sample();

            // Use those logits to guide this sequence
            NativeApi.llama_sample_apply_guidance(ctx, logits, guidanceLogits, weight);
        }

        protected override LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
        {
            candidates.Temperature(ctx, 0.8f);
            candidates.TopK(ctx, 25);

            return candidates.SampleToken(ctx);
        }
    }
}
```

## Inference parameters

Different from context parameters, which is indicated in [understand-llama-context](./UnderstandLLamaContext.md), executors accept parameters when you call its API to execute the inference. That means you could change the parameters every time you ask the model to generate the outputs.

Here is the parameters for LLamaSharp executors.

```cs
/// <summary>
/// The parameters used for inference.
/// </summary>
public record InferenceParams
    : IInferenceParams
{
    /// <summary>
    /// number of tokens to keep from initial prompt
    /// </summary>
    public int TokensKeep { get; set; } = 0;

    /// <summary>
    /// how many new tokens to predict (n_predict), set to -1 to infinitely generate response
    /// until it complete.
    /// </summary>
    public int MaxTokens { get; set; } = -1;

    /// <summary>
    /// logit bias for specific tokens
    /// </summary>
    public Dictionary<LLamaToken, float>? LogitBias { get; set; } = null;

    /// <summary>
    /// Sequences where the model will stop generating further tokens.
    /// </summary>
    public IReadOnlyList<string> AntiPrompts { get; set; } = Array.Empty<string>();

    /// <inheritdoc />
    public int TopK { get; set; } = 40;

    /// <inheritdoc />
    public float TopP { get; set; } = 0.95f;

    /// <inheritdoc />
    public float MinP { get; set; } = 0.05f;

    /// <inheritdoc />
    public float TfsZ { get; set; } = 1.0f;

    /// <inheritdoc />
    public float TypicalP { get; set; } = 1.0f;

    /// <inheritdoc />
    public float Temperature { get; set; } = 0.8f;

    /// <inheritdoc />
    public float RepeatPenalty { get; set; } = 1.1f;

    /// <inheritdoc />
    public int RepeatLastTokensCount { get; set; } = 64;

    /// <inheritdoc />
    public float FrequencyPenalty { get; set; } = .0f;

    /// <inheritdoc />
    public float PresencePenalty { get; set; } = .0f;

    /// <inheritdoc />
    public MirostatType Mirostat { get; set; } = MirostatType.Disable;

    /// <inheritdoc />
    public float MirostatTau { get; set; } = 5.0f;

    /// <inheritdoc />
    public float MirostatEta { get; set; } = 0.1f;

    /// <inheritdoc />
    public bool PenalizeNL { get; set; } = true;

    /// <inheritdoc />
    public SafeLLamaGrammarHandle? Grammar { get; set; }

    /// <inheritdoc />
    public ISamplingPipeline? SamplingPipeline { get; set; }
}
```



## Save and load executor state

An executor also has its state, which can be saved and loaded. That means a lot when you want to support restore a previous session for the user in your application.

The following code shows how to use save and load executor state.

```cs
InteractiveExecutor executor = new InteractiveExecutor(model);
// do some things...
executor.SaveState("executor.st");
var stateData = executor.GetStateData();

InteractiveExecutor executor2 = new InteractiveExecutor(model);
executor2.LoadState(stateData);
// do some things...

InteractiveExecutor executor3 = new InteractiveExecutor(model);
executor3.LoadState("executor.st");
// do some things...
```