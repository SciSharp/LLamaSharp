using LLama.Native;
using LLama.Grammars;

namespace LLama.Unittest
{
    /// <summary>
    /// Source:
    /// https://github.com/ggerganov/llama.cpp/blob/6381d4e110bd0ec02843a60bbeb8b6fc37a9ace9/tests/test-grammar-parser.cpp
    /// 
    /// The commit hash from URL is the actual commit hash that reflects current C# code.
    /// </summary>
    public sealed class GrammarParserTest
    {
        [Fact]
        public void ParseComplexGrammar()
        {
            GBNFGrammarParser parsedGrammar = new GBNFGrammarParser();
            string grammarBytes = @"root  ::= (expr ""="" term ""\n"")+
                expr  ::= term ([-+*/] term)*
                term  ::= [0-9]+";

            var state = parsedGrammar.Parse(grammarBytes, "root");
            Assert.Equal(0ul, state.StartRuleIndex);

            var expected = new List<KeyValuePair<string, uint>>
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

            foreach (var symbol in expected)
            {
                var rule = state.Rules[(int)symbol.Value];
                Assert.Equal(symbol.Key, rule.Name);
            }

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
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
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

            uint index = 0;
            foreach (var rule in state.Rules)
            {
                // compare rule to expected rule
                for (uint i = 0; i < rule.Elements.Count; i++)
                {
                    var element = rule.Elements[(int)i];
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

        [Fact]
        public void ParseExtraComplexGrammar()
        {
            GBNFGrammarParser parsedGrammar = new GBNFGrammarParser();
            string grammarBytes = @"
                root  ::= (expr ""="" ws term ""\n"")+
                expr  ::= term ([-+*/] term)*
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-9]+ ws
                ws    ::= [ \t\n]*
            ";

            var state = parsedGrammar.Parse(grammarBytes, "root");
            Assert.Equal(0ul, state.StartRuleIndex);

            var expected = new List<KeyValuePair<string, uint>>
            {
                new KeyValuePair<string, uint>("expr", 2),
                new KeyValuePair<string, uint>("expr_6", 6),
                new KeyValuePair<string, uint>("expr_7", 7),
                new KeyValuePair<string, uint>("ident", 8),
                new KeyValuePair<string, uint>("ident_10", 10),
                new KeyValuePair<string, uint>("num", 9),
                new KeyValuePair<string, uint>("num_11", 11),
                new KeyValuePair<string, uint>("root", 0),
                new KeyValuePair<string, uint>("root_1", 1),
                new KeyValuePair<string, uint>("root_5", 5),
                new KeyValuePair<string, uint>("term", 4),
                new KeyValuePair<string, uint>("ws", 3),
                new KeyValuePair<string, uint>("ws_12", 12),
            };

            foreach (var symbol in expected)
            {
                var rule = state.Rules[(int)symbol.Value];
                Assert.Equal(symbol.Key, rule.Name);
            }

            var expectedRules = new List<LLamaGrammarElement>
            {
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 5),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 2),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 61),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 4),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 10),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 4),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 7),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 12),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 8),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 9),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 40),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 2),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 41),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 5),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 45),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 43),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 42),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 47),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 4),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 6),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 7),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 97),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 122),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 10),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 11),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 3),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 97),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 122),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 48),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 57),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 95),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 10),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 48),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 57),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 11),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 48),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 57),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 32),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 9),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 10),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 12),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0)
            };

            uint index = 0;
            foreach (var rule in state.Rules)
            {
                // compare rule to expected rule
                for (uint i = 0; i < rule.Elements.Count; i++)
                {
                    var element = rule.Elements[(int)i];
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
