using System.Diagnostics;
using LLama.Common;
using LLama.Sampling;
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
            _params = new ModelParams(Constants.GenerativeModelPath2)
            {
                ContextSize = 60,
                BatchSize = 2,
                GpuLayerCount = Constants.CIGpuLayerCount,                
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
            var pipeline = new DefaultSamplingPipeline();

            var executor = new StatelessExecutor(_weights, _params);

            const string question = "Question. what is a cat?\nAnswer:";
            var @params = new InferenceParams { MaxTokens = 32, AntiPrompts = new[] { "." }, SamplingPipeline = pipeline };

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
        public async Task OutOfContext_WithTruncateStrategy_SuccessfullyGenerates()
        {
            var executor = new StatelessExecutor(_weights, _params);

            const string question = " Question. cats or dogs?\nAnswer:";

            // The context size is set to 60. Generate more than that, forcing it to generate a coherent response
            // with a modified context.
            // We explicitly set the strategy to TruncateAndReprefill to test the new fallback logic.
            var @params = new InferenceParams()
            {
                MaxTokens = 65,
                TokensKeep = question.Length,
                OverflowStrategy = ContextOverflowStrategy.TruncateAndReprefill,
                ContextTruncationPercentage = 0.2f // Drop 20% of tokens when full
            };

            var result1 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());
            var result2 = string.Join("", await executor.InferAsync(question, @params).ToListAsync());

            _testOutputHelper.WriteLine(result1);

            // Check that it produced the exact same result both times
            Assert.Equal(result1, result2);
        }

        [Fact]
        public async Task OutOfContext_WithDefaultStrategy_ThrowsException()
        {
            var executor = new StatelessExecutor(_weights, _params);
            using var context = _weights.CreateContext(_params);

            // Read the ACTUAL context size allocated by the native engine
            uint actualContextSize = context.ContextSize;

            string question = "Cats and dogs are great pets. ";

            // Fast pad for the bulk of it
            while (context.Tokenize(question, special: true).Length < actualContextSize - 20)
            {
                question += "Cats and dogs are great pets. ";
            }

            // Slow pad by single words to precisely hit actualContextSize - 2
            while (context.Tokenize(question, special: true).Length < actualContextSize - 2)
            {
                question += "pet ";
            }

            var finalLength = context.Tokenize(question, special: true).Length;
            _testOutputHelper.WriteLine($"[DEBUG] Actual ContextSize: {actualContextSize}, Prompt length: {finalLength}");

            // Sanity check to ensure we didn't overshoot
            Assert.True(finalLength < actualContextSize, "Prompt exceeded context size during prefill!");

            var @params = new InferenceParams()
            {
                MaxTokens = 10,
                TokensKeep = 5,
            };

            var exception = await Assert.ThrowsAsync<Exceptions.ContextOverflowException>(async () =>
            {
                await executor.InferAsync(question, @params).ToListAsync();
            });

            _testOutputHelper.WriteLine($"Successfully caught expected exception: {exception.Message}");
        }

        [Fact]
        public async Task OutOfContext_WithDefaultStrategy_2_ThrowsException()
        {
            using var context = _weights.CreateContext(_params);
            var executor = new InstructExecutor(context);

            uint actualContextSize = context.ContextSize;
            string instruction = "Cats or dogs? ";

            // Fast pad safely below limit (InstructExecutor adds hidden prefix/suffix)
            while (context.Tokenize(instruction, special: true).Length < actualContextSize - 30)
            {
                instruction += "Cats or dogs? ";
            }

            // Slow pad
            while (context.Tokenize(instruction, special: true).Length < actualContextSize - 15)
            {
                instruction += "pet ";
            }

            var @params = new InferenceParams()
            {
                MaxTokens = 20,
                TokensKeep = 5,
            };

            var exception = await Assert.ThrowsAsync<Exceptions.ContextOverflowException>(async () =>
            {
                await executor.InferAsync(instruction, @params).ToListAsync();
            });

            _testOutputHelper.WriteLine($"Successfully caught expected exception in InstructExecutor: {exception.Message}");
        }
    }
}