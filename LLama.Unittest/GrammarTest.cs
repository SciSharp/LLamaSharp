using System.Text;
using LLama.Common;
using LLama.Native;

namespace LLama.Unittest
{
    public sealed class GrammarTest
        : IDisposable
    {
        private readonly ModelParams _params;
        private readonly LLamaWeights _model;

        public GrammarTest()
        {
            _params = new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin", contextSize: 2048);
            _model = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        [Fact]
        public void CreateBasicGrammar()
        {
            var rules = new List<List<LLamaGrammarElement>>
            {
                new()
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'a'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 'z'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                },
            };

            using var handle = SafeLLamaGrammarHandle.Create(rules, 0);
        }

        [Fact]
        public void SampleWithTrivialGrammar()
        {
            // Create a grammar that constrains the output to be "one" and nothing else
            var rules = new List<List<LLamaGrammarElement>>
            {
                new()
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'o'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'n'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'e'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                },
            };

            using var grammar = SafeLLamaGrammarHandle.Create(rules, 0);

            using var ctx = _model.CreateContext(_params, Encoding.UTF8);
            var executor = new StatelessExecutor(ctx);
            var inferenceParams = new InferenceParams
            {
                MaxTokens = 3,
                AntiPrompts = new [] { ".", "Input:", "\n" },
                Grammar = grammar,
            };

            var result = executor.Infer("Question: What is your favourite number?\nAnswer: ", inferenceParams).ToList();

            Assert.Equal("one", result[0]);
        }
    }
}
