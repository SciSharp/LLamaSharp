using System;
using System.Collections.Generic;
using System.Text;
using LLama.Exceptions;
using LLama.Native;

namespace LLama.Grammars
{
    /// <summary>
    /// A grammar is a set of <see cref="GrammarRule"/>s for deciding which characters are valid next. Can be used to constrain
    /// output to certain formats - e.g. force the model to output JSON
    /// </summary>
    public sealed class Grammar
    {
        /// <summary>
        /// Index of the initial rule to start from
        /// </summary>
        public ulong StartRuleIndex { get; }

        /// <summary>
        /// The rules which make up this grammar
        /// </summary>
        public IReadOnlyList<GrammarRule> Rules { get; }

        /// <summary>
        /// Create a new grammar from a set of rules
        /// </summary>
        /// <param name="rules">The rules which make up this grammar</param>
        /// <param name="startRuleIndex">Index of the initial rule to start from</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Grammar(IReadOnlyList<GrammarRule> rules, ulong startRuleIndex)
        {
            if (startRuleIndex >= (uint)rules.Count)
                throw new ArgumentOutOfRangeException(nameof(startRuleIndex), "startRule must be less than the number of rules");

            StartRuleIndex = startRuleIndex;
            Rules = rules;
        }

        /// <summary>
        /// Create a `SafeLLamaGrammarHandle` instance to use for parsing
        /// </summary>
        /// <returns></returns>
        public SafeLLamaGrammarHandle CreateInstance()
        {
            return SafeLLamaGrammarHandle.Create(Rules, StartRuleIndex);
        }

        /// <summary>
        /// Parse a string of <a href="https://github.com/ggerganov/llama.cpp/tree/master/grammars">GGML BNF</a> into a Grammar
        /// </summary>
        /// <param name="gbnf">The string to parse</param>
        /// <param name="startRule">Name of the start rule of this grammar</param>
        /// <exception cref="GrammarFormatException">Thrown if input is malformed</exception>
        /// <returns>A Grammar which can be converted into a SafeLLamaGrammarHandle for sampling</returns>
        public static Grammar Parse(string gbnf, string startRule)
        {
            var parser = new GBNFGrammarParser();
            return parser.Parse(gbnf, startRule);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            PrintGrammar(builder);
            return builder.ToString();
        }

        private void PrintGrammar(StringBuilder output)
        {
            for (var i = 0; i < Rules.Count; i++)
                PrintRule(output, Rules[i]);
        }

        private void PrintRule(StringBuilder output, GrammarRule rule)
        {
            output.Append($"{rule.Name} ::= ");

            for (int i = 0, end = rule.Elements.Count - 1; i < end; i++)
            {
                var elem = rule.Elements[i];
                switch (elem.Type)
                {
                    // GrammarRule has already verified that END is not being misused, no need to check again
                    case LLamaGrammarElementType.END:
                        break;

                    case LLamaGrammarElementType.ALT:
                        output.Append("| ");
                        break;

                    case LLamaGrammarElementType.RULE_REF:
                        output.Append($"{Rules[(int)elem.Value].Name} ");
                        break;

                    case LLamaGrammarElementType.CHAR:
                        output.Append('[');
                        PrintGrammarChar(output, elem.Value);
                        break;

                    case LLamaGrammarElementType.CHAR_NOT:
                        output.Append("[^");
                        PrintGrammarChar(output, elem.Value);
                        break;

                    case LLamaGrammarElementType.CHAR_RNG_UPPER:
                        output.Append('-');
                        PrintGrammarChar(output, elem.Value);
                        break;

                    case LLamaGrammarElementType.CHAR_ALT:
                        PrintGrammarChar(output, elem.Value);
                        break;
                }

                if (elem.IsCharElement())
                {
                    switch (rule.Elements[i + 1].Type)
                    {
                        case LLamaGrammarElementType.CHAR_ALT:
                        case LLamaGrammarElementType.CHAR_RNG_UPPER:
                            break;

                        case LLamaGrammarElementType.END:
                        case LLamaGrammarElementType.ALT:
                        case LLamaGrammarElementType.RULE_REF:
                        case LLamaGrammarElementType.CHAR:
                        case LLamaGrammarElementType.CHAR_NOT:
                        default:
                            output.Append("] ");
                            break;
                    }
                }
            }

            output.AppendLine();
        }

        private static void PrintGrammarChar(StringBuilder output, uint c)
        {
            if (c >= 0x20 && c <= 0x7F)
            {
                output.Append((char)c);
            }
            else
            {
                // cop out of encoding UTF-8
                output.Append($"<U+{c:X4}>");
            }
        }
    }
}
