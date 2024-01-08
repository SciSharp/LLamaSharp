using LLama.Common;
using LLama.Grammars;
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
            _params = new ModelParams(Constants.ModelPath)
            {
                ContextSize = 2048,
                Seed = 92,
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
            var rules = new List<GrammarRule>
            {
                new GrammarRule("alpha", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'a'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 'z'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                }),
            };

            using var handle = SafeLLamaGrammarHandle.Create(rules, 0);
        }

        [Fact]
        public void CreateGrammar_StartIndexOutOfRange()
        {
            var rules = new List<GrammarRule>
            {
                new GrammarRule("alpha", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'a'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 'z'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                }),
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => new Grammar(rules, 3));
        }

        [Fact]
        public async Task SampleWithTrivialGrammar()
        {
            // Create a grammar that constrains the output to be "cat" and nothing else. This is a nonsense answer, so
            // we can be confident it's not what the LLM would say if not constrained by the grammar!
            var rules = new List<GrammarRule>
            {
                new GrammarRule("feline", new []
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'c'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 'a'),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 't'),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                }),
            };

            var grammar = new Grammar(rules, 0);
            using var grammarInstance = grammar.CreateInstance();
            using var grammarInstance2 = grammarInstance.Clone();

            var executor = new StatelessExecutor(_model, _params);
            var inferenceParams = new InferenceParams
            {
                MaxTokens = 3,
                AntiPrompts = new [] { ".", "Input:", "\n" },
                Grammar = grammarInstance2,
            };

            var result = await executor.InferAsync("Q. 7 + 12\nA. ", inferenceParams).ToListAsync();

            Assert.Equal("cat", result[0]);
        }

        //this test is flakey - it reproduces an error which appears to be a bug in llama.cpp
        //[Fact]
        //public async Task SampleTwiceWithGrammar()
//        {
//            var executor = new StatelessExecutor(_model, _params);

//            var grammar = Grammar.Parse("""
//root   ::= (object | array) endline?
//endline ::= "<|im_end|>" ws
//value  ::= object | array | string | number | ("true" | "false" | "null") ws

//object ::=
//"{" ws (
//          string ":" ws value
//  ("," ws string ":" ws value)*
//)? "}" ws

//array  ::=
//"[" ws (
//          value
//  ("," ws value)*
//)? "]" ws

//string ::=
//"\"" (
//  [^"\\] |
//  "\\" (["\\/bfnrt] | "u" [0-9a-fA-F] [0-9a-fA-F] [0-9a-fA-F]) # escapes
//)* "\"" ws

//number ::= ("-"? ([0-9] | [1-9] [0-9]*)) ("." [0-9]+)? ([eE] [-+]? [0-9]+)? ws

//# Optional space: by convention, applied in this grammar after literal chars when allowed
//ws ::= ([ \t\n] ws)?
//""",
//                "root");

//            using (var grammarInstance = grammar.CreateInstance())
//            {
//                var inferenceParams = new InferenceParams
//                {
//                    MaxTokens = 20,
//                    AntiPrompts = new[] { ".", "Input:", "\n", "<|im_end|>" },
//                    Grammar = grammarInstance,
//                };
//                var result = await executor.InferAsync("Write a JSON array with the first 6 positive numbers", inferenceParams).ToListAsync();
//            }

//            using (var grammarInstance2 = grammar.CreateInstance())
//            {
//                var inferenceParams2 = new InferenceParams
//                {
//                    MaxTokens = 20,
//                    AntiPrompts = new[] { ".", "Input:", "\n" },
//                    Grammar = grammarInstance2,
//                };
//                var result2 = await executor.InferAsync("Write a JSON array with the first 6 positive numbers", inferenceParams2).ToListAsync();
//            }
//        }
    }
}
