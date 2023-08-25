﻿using LLama.Common;
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
            _params = new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin")
            {
                ContextSize = 2048,
            };
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
            // Create a grammar that constrains the output to be "cat" and nothing else. This is a nonsense answer, so
            // we can be confident it's not what the LLM would say if not constrained by the grammar!
            var rules = new List<List<LLamaGrammarElement>>
            {
                new()
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'c'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'a'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 't'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                },
            };

            using var grammar = SafeLLamaGrammarHandle.Create(rules, 0);

            var executor = new StatelessExecutor(_model, _params);
            var inferenceParams = new InferenceParams
            {
                MaxTokens = 3,
                AntiPrompts = new [] { ".", "Input:", "\n" },
                Grammar = grammar,
            };

            var result = executor.Infer("Question: What is your favourite number?\nAnswer: ", inferenceParams).ToList();

            Assert.Equal("cat", result[0]);
        }
    }
}
