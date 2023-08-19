using LLama.Common;
using System.Text;

namespace LLama.Unittest
{
    public class StatelessExecutorTest
        : IDisposable
    {
        private readonly LLamaWeights _weights;
        private readonly ModelParams _params;

        public StatelessExecutorTest()
        {
            _params = new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin")
            {
                ContextSize = 60,
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
            var executor = new StatelessExecutor(_weights.CreateContext(_params, Encoding.UTF8));

            const string question = "Question. what is a cat?\nAnswer: ";
            const string expected = " a domestic or wild animal that is typically small to medium-sized, has fur, four legs, and sharp retractable claws.";
            var @params = new InferenceParams { MaxTokens = 32, AntiPrompts = new[] { "." } };

            var result1 = string.Join("", executor.Infer(question, @params));
            Assert.Equal(expected, result1);

            var result2 = string.Join("", executor.Infer(question, @params));
            Assert.Equal(expected, result2);

            Assert.Equal(result1, result2);
        }

        [Fact]
        public void OutOfContext()
        {
            var executor = new StatelessExecutor(_weights.CreateContext(_params, Encoding.UTF8));

            const string question = " Question. why is a cat the best pet?\nAnswer: ";
            const string answer = " there are many reasons why cats make excellent pets! here are just a few of them:\n" +
                                  "1)Loyalty: Cats are known for their loyalty to their owners, and they will often follow " +
                                  "you around the house if you call them. They will always come running when called, and they’ll " +
                                  "nuzzle and purr with delight when you walk into the room! they adore being close to their human " +
                                  "family members and can form very close bonds.\n";

            // The context size is set to 60. Generate more than that, forcing it to generate a coherent response
            // with a modified context
            var @params = new InferenceParams()
            {
                MaxTokens = 100,
                TokensKeep = question.Length,
            };

            var result = string.Join("", executor.Infer(question, @params));

            Assert.Equal(answer, result);
        }
    }
}