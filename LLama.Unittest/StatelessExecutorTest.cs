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
            _params = new ModelParams(Constants.GenerativeModelPath)
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
        public async Task OutOfContext()
        {
            var executor = new StatelessExecutor(_weights, _params);

            const string question = " Question. cats or dogs?\nAnswer:";

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