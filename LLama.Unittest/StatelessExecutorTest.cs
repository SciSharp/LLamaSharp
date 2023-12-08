using System.Diagnostics;
using LLama.Common;
using LLama.Sampling;
using LLama.Sampling.Logits;
using LLama.Sampling.Selection;
using LLama.Sampling.Tokens;
using Xunit.Abstractions;

namespace LLama.Unittest
{
    public class StatelessExecutorTest
        : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly LLamaWeights _weights;
        private readonly ModelParams _params;

        public StatelessExecutorTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _params = new ModelParams(Constants.ModelPath)
            {
                ContextSize = 60,
                Seed = 1754,
            };
            _weights = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _weights.Dispose();
        }

        [Fact]
        public async Task Stateless()
        {
            // Create a custom pipeline that mimics the default pipeline
            var pipeline = new ConfigurableSamplingPipeline()
            {
                ProtectedLogits =
                {
                    _weights.NewlineToken,
                    _weights.BeginningOfSentenceToken,
                    _weights.EndOfSentenceToken
                },
                LogitProcessors =
                {
                    new LogitBias
                    {
                        Biases =
                        {
                            { _weights.NewlineToken, 1000 }, // This is an insane bias, but because newline is a protected logit it will do nothing!
                            { 42, 0f },
                        }
                    }
                },
                TokenDataProcessors =
                {
                    new TailFreeSampling { Z = 1 },
                    new LocallyTypicalSampling { P = 1 },
                    new TopPSampling { P = 0.95f },
                    new MinPSampling { P = 0.05f },
                    new TemperatureSampling { Temperature = 0.8f },
                },
                Selector = new StandardSelection(),
            };

            var executor = new StatelessExecutor(_weights, _params);

            const string question = "Question. what is a cat?\nAnswer: ";
            var @params = new InferenceParams { MaxTokens = 32, AntiPrompts = new[] { "." }, SamplingPipeline = pipeline};

            var timer = new Stopwatch();
            timer.Start();

            var result1 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());
            var result2 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());

            timer.Stop();
            _testOutputHelper.WriteLine($"{timer.ElapsedMilliseconds}ms");

            _testOutputHelper.WriteLine(result1);
            _testOutputHelper.WriteLine(result2);

            // Check that it produced the exact same result both times
            Assert.Equal(result1, result2);
        }

        [Fact(Skip = "Very very slow in CI")]
        public async Task OutOfContext()
        {
            var executor = new StatelessExecutor(_weights, _params);

            const string question = " Question. cats or dogs?\nAnswer: ";

            // The context size is set to 60. Generate more than that, forcing it to generate a coherent response
            // with a modified context
            var @params = new InferenceParams()
            {
                MaxTokens = 65,
                TokensKeep = question.Length,
            };

            var result1 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());
            var result2 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());

            _testOutputHelper.WriteLine(result1);

            // Check that it produced the exact same result both times
            Assert.Equal(result1, result2);
        }
    }
}