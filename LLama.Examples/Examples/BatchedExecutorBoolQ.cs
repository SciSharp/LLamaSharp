using System.Text;
using LLama.Batched;
using LLama.Common;
using LLama.Native;
using Spectre.Console;
using LLama.Sampling;

namespace LLama.Examples.Examples;

public class BatchedExecutorBoolQ
{
    // Answers may start with a space, and then must produce one of the listed strings followed by a newline character and nothing else.
    private static readonly Grammar AnswerGrammar = new("root ::= (\" \")? (\"true\" | \"false\" | \"yes\" | \"no\") \"\\n\"", "root");

    public static async Task Run()
    {
        // Load model weights
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);

        const int tokensGenerate = 8;
        var batchSize = AnsiConsole.Ask("How many parallel conversations to evaluate in a batch", 64);
        var sys = AnsiConsole.Ask("System prompt", "Answer the question with a single word answer.");
        var hint = AnsiConsole.Ask("Provide hints to model (test reading comprehension instead of knowledge)", true);

        // Create an executor that can evaluate a batch of conversations together
        using var executor = new BatchedExecutor(model, parameters);

        // Print some info
        var name = model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // Load dataset
        var data = new List<(string, bool, string)>();
        if (AnsiConsole.Ask("Load training dataset?", false))
            data.AddRange(LoadData("Assets/BoolQ/train.csv"));
        if (AnsiConsole.Ask("Load validation dataset?", true))
            data.AddRange(LoadData("Assets/BoolQ/validation.csv"));
        AnsiConsole.MarkupLineInterpolated($"Loaded Dataset: {data.Count} questions");
        var limit = AnsiConsole.Ask("Limit dataset size", 1000);
        if (data.Count > limit)
            data = data.Take(limit).ToList();

        // Process data in batches
        var chunks = data.Chunk(batchSize).ToArray();
        var results = new List<BatchResult>();
        await AnsiConsole.Progress()
                         .Columns(new SpinnerColumn(Spinner.Known.Dots8Bit), new PercentageColumn(), new ProgressBarColumn(), new RemainingTimeColumn())
                         .StartAsync(async ctx =>
        {
            var reporter = ctx.AddTask("Processing Chunks", maxValue: chunks.Length);

            foreach (var chunk in chunks)
            {
                var result = await RunBatch(executor, tokensGenerate, sys, hint, chunk);
                results.Add(result);

                reporter.Increment(1);

                AnsiConsole.MarkupLineInterpolated($"[green]{result.TruePositive + result.TrueNegative}[/] / [red]{chunk.Length}[/] ({result.Accuracy:P})");
            }
        });

        // Print final results
        var correct = (from result in results select result.Correct).Sum();
        var total = data.Count;
        var accuracy = (float)correct / total;
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupInterpolated($"Final Result: [green]{correct}[/] / [red]{total}[/] ({accuracy:P})");

    }

    private static IEnumerable<(string, bool, string)> LoadData(string path)
    {
        foreach (var line in File.ReadLines(path))
        {
            var splits = line.Split(",");

            if (!bool.TryParse(splits[1], out var boolean))
                continue;

            var hint = string.Join(",", splits[2..]);
            hint = hint.Trim('\"');

            yield return (splits[0], boolean, hint);
        }
    }

    private static async Task<BatchResult> RunBatch(BatchedExecutor executor, int maxTokens, string sys, bool hint, IEnumerable<(string, bool, string)> batch)
    {
        var conversations = (from item in batch
                             select new ConversationRunner(executor, sys, item.Item1, item.Item2, hint ? item.Item3 : null)).ToArray();

        for (var i = 0; i < maxTokens; i++)
        {
            if (executor.BatchQueueCount > 1)
                AnsiConsole.MarkupLineInterpolated($"Batch Queue: {executor.BatchQueueCount} ({i})");

            // Process the entire queue of batching waiting to be processed
            while (executor.BatchQueueCount > 0)
            {
                var result = await executor.Infer();
                if (result != DecodeResult.Ok)
                    throw new NotImplementedException($"Decode failed: {result}");

                foreach (var item in conversations)
                    item.Sample();
            }

            // Prompt each conversation that just sampled a token
            foreach (var item in conversations)
                item.Prompt();
        }

        int tp = 0, tn = 0, fp = 0, fn = 0;
        foreach (var item in conversations)
        {
            item.Result(ref tp, ref tn, ref fp, ref fn);
            item.Dispose();
        }

        return new BatchResult(tp, tn, fp, fn);
    }

    private record BatchResult(int TruePositive, int TrueNegative, int FalsePositive, int FalseNegative)
    {
        public int Correct => TruePositive + TrueNegative;
        public int Incorrect => FalsePositive + FalseNegative;

        public int TotalPositives = TruePositive + FalseNegative;
        public int TotalNegatives = TrueNegative + FalsePositive;
        public int Total => Correct + Incorrect;

        public float Accuracy => (float)Correct / Total;
    }

    /// <summary>
    /// All of the mechanics necessary to run a conversation to answer a single question
    /// </summary>
    private class ConversationRunner
        : IDisposable
    {
        private readonly BatchedExecutor _executor;
        private readonly StreamingTokenDecoder _decoder;
        private readonly ISamplingPipeline _sampler;

        private readonly Conversation _conversation;
        private bool _finished;
        private LLamaToken? _sampledToken;

        public string Question { get; }
        public bool Answer { get; }

        public ConversationRunner(BatchedExecutor executor, string sys, string question, bool answer, string? hint)
        {
            _executor = executor;
            _decoder = new StreamingTokenDecoder(executor.Context);
            _sampler = new GreedySamplingPipeline { Grammar = AnswerGrammar };

            // Make sure question ends with question mark
            if (!question.EndsWith('?'))
                question += '?';

            // Prepend hint if necessary
            if (hint != null)
            {
                if (!hint.EndsWith('.'))
                    hint += '.';
                question = $"{hint}\n{question}";
            }

            // Template the question
            var template = new LLamaTemplate(executor.Model);
            template.Add("system", sys);
            template.Add("user", question);
            template.AddAssistant = true;
            var templatedQuestion = Encoding.UTF8.GetString(template.Apply());

            // Prompt
            _conversation = executor.Create();
            _conversation.Prompt(_executor.Context.Tokenize(templatedQuestion));

            Question = question;
            Answer = answer;
        }

        public void Sample()
        {
            if (_finished)
                return;
            if (!_conversation.RequiresSampling)
                return;

            var token = _sampler.Sample(_executor.Context, _conversation.GetSampleIndex());

            var tokens = _executor.Context.NativeHandle.ModelHandle.Tokens;
            if (tokens.IsEndOfGeneration(token) || tokens.Newline == token)
            {
                _sampledToken = default;
                _finished = true;
            }
            else
            {
                _sampledToken = token;
            }
        }

        public void Prompt()
        {
            if (_finished)
                return;
            if (!_sampledToken.HasValue)
                return;

            var token = _sampledToken.Value;
            _sampledToken = default;

            _sampler.Accept(token);
            _decoder.Add(token);
            _conversation.Prompt(token);
        }

        public void Result(ref int tp, ref int tn, ref int fp, ref int fn)
        {
            var str = _decoder.Read().Trim();
            var result = str switch
            {
                "true" or "yes" => true,
                _ => false,
            };

            switch (Answer, result)
            {
                case (true, true): tp++; break;
                case (true, false): fn++; break;
                case (false, true): fp++; break;
                case (false, false): tn++; break;
            }
        }

        public void Dispose()
        {
            _conversation.Dispose();
            _sampler.Dispose();
        }
    }
}