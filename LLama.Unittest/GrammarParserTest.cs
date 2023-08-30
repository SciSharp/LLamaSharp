using LLama.Common;
using LLama.Native;
using System.Diagnostics;
using LLama.Grammar;
using Newtonsoft.Json.Linq;

namespace LLama.Unittest
{
    public sealed class GrammarParserTest
    {
        [Fact]
        public void ParseComplexGrammar()
        {
            GrammarParser parsedGrammar = new GrammarParser();
            string grammarBytes = @"root  ::= (expr ""="" term ""\n"")+
                expr  ::= term ([-+*/] term)*
                term  ::= [0-9]+";

            ParseState state = parsedGrammar.Parse(grammarBytes);

            List<KeyValuePair<string, uint>> expected = new List<KeyValuePair<string, uint>>
            {
                new KeyValuePair<string, uint>("expr", 2),
                new KeyValuePair<string, uint>("expr_5", 5),
                new KeyValuePair<string, uint>("expr_6", 6),
                new KeyValuePair<string, uint>("root", 0),
                new KeyValuePair<string, uint>("root_1", 1),
                new KeyValuePair<string, uint>("root_4", 4),
                new KeyValuePair<string, uint>("term", 3),
                new KeyValuePair<string, uint>("term_7", 7),
            };

            uint index = 0;
            foreach (var it in state.SymbolIds)
            {
                string key = it.Key;
                uint value = it.Value;
                var expectedPair = expected[(int)index];

                // pretty print error message before asserting
                if (expectedPair.Key != key || expectedPair.Value != value)
                {
                    Console.Error.WriteLine($"expectedPair: {expectedPair.Key}, {expectedPair.Value}");
                    Console.Error.WriteLine($"actualPair: {key}, {value}");
                    Console.Error.WriteLine("expectedPair != actualPair");
                }
                Assert.Equal(expectedPair.Key, key);
                Assert.Equal(expectedPair.Value, value);

                index++;
            }
            Assert.NotEmpty(state.SymbolIds);


            var expectedRules = new List<LLamaGrammarElement>
            {
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 4),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 2),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 61),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 10),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 6),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 7),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 4),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 45),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 43),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 42),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 47),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 5),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 6),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 48),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 57),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 7),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 48),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 57),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
            };

            index = 0;
            foreach (var rule in state.Rules)
            {
                // compare rule to expected rule
                for (uint i = 0; i < rule.Count; i++)
                {
                    var element = rule[(int)i];
                    var expectedElement = expectedRules[(int)index];

                    // Pretty print error message before asserting
                    if (expectedElement.Type != element.Type || expectedElement.Value != element.Value)
                    {
                        Console.Error.WriteLine($"index: {index}");
                        Console.Error.WriteLine($"expected_element: {expectedElement.Type}, {expectedElement.Value}");
                        Console.Error.WriteLine($"actual_element: {element.Type}, {element.Value}");
                        Console.Error.WriteLine("expected_element != actual_element");
                    }
                    Assert.Equal(expectedElement.Type, element.Type);
                    Assert.Equal(expectedElement.Value, element.Value);
                    index++;
                }
            }
            Assert.NotEmpty(state.Rules);
        }
    }
}
