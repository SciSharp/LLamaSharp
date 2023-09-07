using LLama.Common;
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
                ContextSize = 50,
                Seed = 1754
            };
            _weights = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _weights.Dispose();
        }

        [Fact]
        public void Stateless()
        {
            var executor = new StatelessExecutor(_weights, _params);

            const string question = "Q. what is a cat?\nA. ";
            var @params = new InferenceParams { MaxTokens = 10, AntiPrompts = new[] { "." } };

            var result1 = string.Join("", executor.Infer(question, @params));
            var result2 = string.Join("", executor.Infer(question, @params));

            _testOutputHelper.WriteLine(result1);

            // Check that it produced the exact same result both times
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void OutOfContext()
        {
            var executor = new StatelessExecutor(_weights, _params);

            const string question = "Q. why is a cat the best pet?\nA. ";

            // The context size is set to 50. Generate more than that, forcing it to generate a coherent response
            // with a modified context
            var @params = new InferenceParams()
            {
                MaxTokens = 75,
                TokensKeep = question.Length,
            };

            var result1 = string.Join("", executor.Infer(question, @params));
            var result2 = string.Join("", executor.Infer(question, @params));

            _testOutputHelper.WriteLine(result1);

            // Check that it produced the exact same result both times
            Assert.Equal(result1, result2);
        }
    }
}