using LLama.Exceptions;
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
        private static void CheckGrammar(string grammar, string rootRule, List<KeyValuePair<string, uint>> expected, List<LLamaGrammarElement> expectedRules)
        {
            var state = Grammar.Parse(grammar, rootRule);
            Assert.Equal(0ul, state.StartRuleIndex);

            foreach (var symbol in expected)
            {
                var rule = state.Rules[(int)symbol.Value];
                Assert.Equal(symbol.Key, rule.Name);
            }

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
        public void ParseComplexGrammar()
        {
            var grammarBytes = @"root  ::= (expr ""="" term ""\n"")+
                expr  ::= term ([-\x2b\x2A/] term)*
                term  ::= [\x30-\x39]+";

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

            CheckGrammar(grammarBytes, "root", expected, expectedRules);
        }

        [Fact]
        public void ParseExtraComplexGrammar()
        {
            string grammarBytes = @"
                root  ::= (expr ""="" ws term ""\n"")+
                expr  ::= term ([-+*/] term)*
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-9]+ ws
                ws    ::= [ \t\n]*
            ";

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

            CheckGrammar(grammarBytes, "root", expected, expectedRules);
        }

        [Fact]
        public void ParseGrammarNotSequence()
        {
            var grammarBytes = @"root  ::= [^a]";

            var expected = new List<KeyValuePair<string, uint>>
            {
                new KeyValuePair<string, uint>("root", 0),
            };

            var expectedRules = new List<LLamaGrammarElement>
            {
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR_NOT, 97),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
            };

            CheckGrammar(grammarBytes, "root", expected, expectedRules);
        }

        [Fact]
        public void ParseGrammarWithMultibyteCharacter()
        {
            var grammarBytes = @"root  ::= [罗]*";

            var expected = new List<KeyValuePair<string, uint>>
            {
                new KeyValuePair<string, uint>("root", 0),
                new KeyValuePair<string, uint>("root_1", 1),
            };

            var expectedRules = new List<LLamaGrammarElement>
            {
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.CHAR, 32599),
                new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 1),
                new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
            };

            CheckGrammar(grammarBytes, "root", expected, expectedRules);
        }


        [Fact]
        public void InvalidGrammarMissingRuleDefinition()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"root  := [^a]";

            Assert.Throws<GrammarExpectedNext>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }

        [Fact]
        public void InvalidGrammarNoClosingBracket()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"
                root  ::= (expr ""="" ws term ""\n""+           ## <--- Mismatched brackets on this line
                expr  ::= term ([-+*/] term)*
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-9]+ ws
                ws    ::= [ \t\n]*
            ";

            Assert.Throws<GrammarExpectedNext>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }

        [Fact]
        public void InvalidGrammarNoName()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"
                root  ::= (expr ""="" ws term ""\n"")+
                  ::= term ([-+*/] term)*                       ## <--- Missing a name for this rule!
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-9]+ ws
                ws    ::= [ \t\n]*
            ";

            Assert.Throws<GrammarExpectedName>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }

        [Fact]
        public void InvalidGrammarBadHex()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"
                root  ::= (expr ""="" ws term ""\n"")+
                expr  ::= term ([-+*/] term)*
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-\xQQ]+ ws                             ## <--- `\xQQ` is not valid hex!
                ws    ::= [ \t\n]*
            ";

            Assert.Throws<GrammarUnexpectedHexCharsCount>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }

        [Fact]
        public void InvalidGrammarBadEscapeCharacter()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"
                root  ::= (expr ""="" ws term ""\z"")+          ## <--- `\z` is not a valid escape character
                expr  ::= term ([-+*/] term)*
                term  ::= ident | num | ""("" ws expr "")"" ws
                ident ::= [a-z] [a-z0-9_]* ws
                num   ::= [0-9]+ ws
                ws    ::= [ \t\n]*
            ";

            Assert.Throws<GrammarUnknownEscapeCharacter>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }

        [Fact]
        public void InvalidGrammarUnexpectedEndOfInput()
        {
            var parsedGrammar = new GBNFGrammarParser();
            var grammarBytes = @"root  ::= (expr ""="" ws term ""\";

            Assert.Throws<GrammarUnexpectedEndOfInput>(() =>
            {
                parsedGrammar.Parse(grammarBytes, "root");
            });
        }


        [Fact]
        public void InvalidRuleNoElements()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", Array.Empty<LLamaGrammarElement>());
            });
        }

        [Fact]
        public void InvalidRuleNoEndElement()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0)
                });
            });
        }

        [Fact]
        public void InvalidRuleExtraEndElement()
        {
            Assert.Throws<GrammarUnexpectedEndElement>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0)
                });
            });
        }

        [Fact]
        public void InvalidRuleMalformedRange()
        {
            Assert.Throws<GrammarUnexpectedCharRngElement>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0)
                });
            });

            
        }

        [Fact]
        public void InvalidRuleMalformedCharAlt()
        {
            Assert.Throws<GrammarUnexpectedCharAltElement>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", new[]
                {
                    new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.CHAR_ALT, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0)
                });
            });
        }

        [Fact]
        public void InvalidRuleElement()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new GrammarRule("name", new[]
                {
                    new LLamaGrammarElement((LLamaGrammarElementType)99999, 0),
                    new LLamaGrammarElement(LLamaGrammarElementType.END, 0)
                });
            });
        }
    }
}
