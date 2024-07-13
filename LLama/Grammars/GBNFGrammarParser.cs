using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LLama.Exceptions;
using LLama.Native;

namespace LLama.Grammars
{
    /// <summary>
    /// Source:
    /// https://github.com/ggerganov/llama.cpp/blob/6381d4e110bd0ec02843a60bbeb8b6fc37a9ace9/common/grammar-parser.cpp
    /// 
    /// The commit hash from URL is the actual commit hash that reflects current C# code.
    /// </summary>
    internal sealed class GBNFGrammarParser
    {
        // NOTE: assumes valid utf8 (but checks for overrun)
        // copied from llama.cpp
        private static uint DecodeUTF8(ref ReadOnlySpan<byte> src)
        {
            Span<int> lookup = [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 3, 4 ];

            byte firstByte = src[0];
            byte highbits = (byte)(firstByte >> 4);
            int len = lookup[highbits];
            byte mask = (byte)((1 << (8 - len)) - 1);
            uint value = (uint)(firstByte & mask);

            int end = len;
            int pos = 1;

            for (; pos < end && pos < src.Length; pos++)
            {
                value = (uint)((value << 6) + (src[pos] & 0x3F));
            }

            src = src.Slice(pos);

            return value;
        }

        private static bool IsWordChar(byte c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '-' || ('0' <= c && c <= '9');
        }

        private static uint ParseHex(ref ReadOnlySpan<byte> src, int size)
        {
            int pos = 0;
            int end = size;
            uint value = 0;

            for (; pos < end && pos < src.Length; pos++)
            {
                value <<= 4;
                byte c = src[pos];
                if ('a' <= c && c <= 'f')
                {
                    value += (uint)(c - 'a' + 10);
                }
                else if ('A' <= c && c <= 'F')
                {
                    value += (uint)(c - 'A' + 10);
                }
                else if ('0' <= c && c <= '9')
                {
                    value += (uint)(c - '0');
                }
                else
                {
                    break;
                }
            }

            if (pos != end)
            {
                throw new GrammarUnexpectedHexCharsCount(size, Encoding.UTF8.GetString(src.ToArray()));
            }
            src = src.Slice(pos);
            return value;
        }

        private static ReadOnlySpan<byte> ParseSpace(ReadOnlySpan<byte> src, bool newlineOk)
        {
            int pos = 0;
            while (pos < src.Length &&
                   (src[pos] == ' ' || src[pos] == '\t' || src[pos] == '#' ||
                    (newlineOk && (src[pos] == '\r' || src[pos] == '\n'))))
            {
                if (src[pos] == '#')
                {
                    while (pos < src.Length && src[pos] != '\r' && src[pos] != '\n')
                    {
                        pos++;
                    }
                }
                else
                {
                    pos++;
                }
            }
            return src.Slice(pos);
        }

        private static ReadOnlySpan<byte> ParseName(ReadOnlySpan<byte> src)
        {
            int pos = 0;
            while (pos < src.Length && IsWordChar(src[pos]))
            {
                pos++;
            }
            if (pos == 0)
            {
                throw new GrammarExpectedName(Encoding.UTF8.GetString(src.ToArray()));
            }
            return src.Slice(pos);
        }

        private static uint ParseChar(ref ReadOnlySpan<byte> src)
        {
            if (src[0] == '\\')
            {
                if (src.Length < 2)
                    throw new GrammarUnexpectedEndOfInput();

                var chr = src[1];
                src = src.Slice(2);

                return (char)chr switch
                {
                    'x' => ParseHex(ref src, 2),
                    'u' => ParseHex(ref src, 4),
                    'U' => ParseHex(ref src, 8),
                    't' => '\t',
                    'r' => '\r',
                    'n' => '\n',
                    '\\' or '"' or '[' or ']' => chr,
                    _ => throw new GrammarUnknownEscapeCharacter(Encoding.UTF8.GetString(src.ToArray())),
                };
            }

            if (!src.IsEmpty)
                return DecodeUTF8(ref src);

            throw new GrammarUnexpectedEndOfInput();
        }

        private ReadOnlySpan<byte> ParseSequence(
            ParseState state,
            ReadOnlySpan<byte> pos,
            string ruleName,
            List<LLamaGrammarElement> outElements,
            bool isNested)
        {
            int lastSymStart = outElements.Count;

            while (!pos.IsEmpty)
            {
                if (pos[0] == '"')  // literal string
                {
                    pos = pos.Slice(1);
                    lastSymStart = outElements.Count;

                    while (!pos.IsEmpty && pos[0] != '"')
                    {
                        var charPair = ParseChar(ref pos);
                        outElements.Add(new LLamaGrammarElement(LLamaGrammarElementType.CHAR, charPair));
                    }
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (pos[0] == '[')  // char range(s)
                {
                    pos = pos.Slice(1);
                    var startType = LLamaGrammarElementType.CHAR;

                    if (pos[0] == '^')
                    {
                        pos = pos.Slice(1);
                        startType = LLamaGrammarElementType.CHAR_NOT;
                    }

                    lastSymStart = outElements.Count;

                    while (!pos.IsEmpty && pos[0] != ']')
                    {
                        var charPair = ParseChar(ref pos);
                        var type = lastSymStart < outElements.Count ? LLamaGrammarElementType.CHAR_ALT : startType;

                        outElements.Add(new LLamaGrammarElement(type, charPair));

                        if (pos[0] == '-' && pos[1] != ']')
                        {
                            pos = pos.Slice(1);
                            var endCharPair = ParseChar(ref pos);
                            outElements.Add(new LLamaGrammarElement(LLamaGrammarElementType.CHAR_RNG_UPPER, endCharPair));
                        }
                    }
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (IsWordChar(pos[0]))  // rule reference
                {
                    var nameEnd = ParseName(pos);
                    uint refRuleId = state.GetSymbolId(pos, nameEnd.Length);
                    pos = ParseSpace(nameEnd, isNested);
                    lastSymStart = outElements.Count;
                    outElements.Add(new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, refRuleId));
                }
                else if (pos[0] == '(')  // grouping
                {
                    // parse nested alternates into synthesized rule
                    pos = ParseSpace(pos.Slice(1), true);
                    uint subRuleId = state.GenerateSymbolId(ruleName);
                    pos = ParseAlternates(state, pos, ruleName, subRuleId, true);
                    lastSymStart = outElements.Count;
                    // output reference to synthesized rule
                    outElements.Add(new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, subRuleId));
                    if (pos[0] != ')')
                        throw new GrammarExpectedNext(")", Encoding.UTF8.GetString(pos.ToArray()));
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (pos[0] == '*' || pos[0] == '+' || pos[0] == '?') // repetition operator
                {
                    if (lastSymStart == outElements.Count)
                        throw new GrammarExpectedPrevious("*/+/?", Encoding.UTF8.GetString(pos.ToArray()));

                    // apply transformation to previous symbol (lastSymStart to end) according to
                    // rewrite rules:
                    // S* --> S' ::= S S' |
                    // S+ --> S' ::= S S' | S
                    // S? --> S' ::= S |
                    uint subRuleId = state.GenerateSymbolId(ruleName);

                    List<LLamaGrammarElement> subRule = new List<LLamaGrammarElement>();

                    // add preceding symbol to generated rule
                    subRule.AddRange(outElements.GetRange(lastSymStart, outElements.Count - lastSymStart));

                    if (pos[0] == '*' || pos[0] == '+')
                    {
                        // cause generated rule to recurse
                        subRule.Add(new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, subRuleId));
                    }

                    // mark start of alternate def
                    subRule.Add(new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0));

                    if (pos[0] == '+')
                    {
                        // add preceding symbol as alternate only for '+' (otherwise empty)
                        subRule.AddRange(outElements.GetRange(lastSymStart, outElements.Count - lastSymStart));
                    }

                    subRule.Add(new LLamaGrammarElement(LLamaGrammarElementType.END, 0));

                    state.AddRule(subRuleId, subRule);

                    // in original rule, replace previous symbol with reference to generated rule
                    outElements.RemoveRange(lastSymStart, outElements.Count - lastSymStart);
                    outElements.Add(new LLamaGrammarElement(LLamaGrammarElementType.RULE_REF, subRuleId));

                    pos = ParseSpace(pos.Slice(1), isNested);

                }
                else
                {
                    break;
                }
            }

            return pos;
        }

        private ReadOnlySpan<byte> ParseAlternates(
            ParseState state,
            ReadOnlySpan<byte> src,
            string ruleName,
            uint ruleId,
            bool isNested)
        {
            var rule = new List<LLamaGrammarElement>();
            ReadOnlySpan<byte> pos = ParseSequence(state, src, ruleName, rule, isNested);

            while (!pos.IsEmpty && pos[0] == '|')
            {
                rule.Add(new LLamaGrammarElement(LLamaGrammarElementType.ALT, 0));
                pos = ParseSpace(pos.Slice(1), true);
                pos = ParseSequence(state, pos, ruleName, rule, isNested);
            }

            rule.Add(new LLamaGrammarElement(LLamaGrammarElementType.END, 0));
            state.AddRule(ruleId, rule);

            return pos;
        }

        private ReadOnlySpan<byte> ParseRule(ParseState state, ReadOnlySpan<byte> src)
        {
            ReadOnlySpan<byte> nameEnd = ParseName(src);
            ReadOnlySpan<byte> pos = ParseSpace(nameEnd, false);
            int nameLen = src.Length - nameEnd.Length;
            uint ruleId = state.GetSymbolId(src.Slice(0, nameLen), 0);
            string name = Encoding.UTF8.GetString(src.Slice(0, nameLen).ToArray());

            if (!(pos[0] == ':' && pos[1] == ':' && pos[2] == '='))
                throw new GrammarExpectedNext("::=", Encoding.UTF8.GetString(pos.ToArray()));

            pos = ParseSpace(pos.Slice(3), true);

            pos = ParseAlternates(state, pos, name, ruleId, false);

            if (!pos.IsEmpty && pos[0] == '\r')
            {
                pos = pos.Slice(pos[1] == '\n' ? 2 : 1);
            }
            else if (!pos.IsEmpty && pos[0] == '\n')
            {
                pos = pos.Slice(1);
            }
            else if (!pos.IsEmpty)
            {
                throw new GrammarExpectedNext("newline or EOF", Encoding.UTF8.GetString(pos.ToArray()));
            }
            return ParseSpace(pos, true);
        }

        /// <summary>
        /// Parse a string of <a href="https://github.com/ggerganov/llama.cpp/tree/master/grammars">GGML BNF</a>
        /// </summary>
        /// <param name="input">The string to parse</param>
        /// <param name="startRule">The name of the root rule of this grammar</param>
        /// <exception cref="GrammarFormatException">Thrown if input is malformed</exception>
        /// <returns>A ParseState that can be converted into a grammar for sampling</returns>
        public Grammar Parse(string input, string startRule)
        {
            var byteArray = Encoding.UTF8.GetBytes(input);
            var state = new ParseState();
            var pos = ParseSpace(byteArray, true);

            while (!pos.IsEmpty)
            {
                pos = ParseRule(state, pos);
            }

            var names = state.SymbolIds.ToDictionary(a => a.Value, a => a.Key);
            var rules = new List<GrammarRule>();
            for (var i = 0; i < state.Rules.Count; i++)
            {
                var elements = state.Rules[i];
                var name = names[(uint)i];
                rules.Add(new GrammarRule(name, elements));
            }

            var startRuleIndex = state.SymbolIds[startRule];
            return new Grammar(rules, startRuleIndex);
        }

        private record ParseState
        {
            public SortedDictionary<string, uint> SymbolIds { get; } = new();
            public List<List<LLamaGrammarElement>> Rules { get; } = new();

            public uint GetSymbolId(ReadOnlySpan<byte> src, int len)
            {
                var nextId = (uint)SymbolIds.Count;
                var key = Encoding.UTF8.GetString(src.Slice(0, src.Length - len).ToArray());

                if (SymbolIds.TryGetValue(key, out uint existingId))
                {
                    return existingId;
                }
                else
                {
                    SymbolIds[key] = nextId;
                    return nextId;
                }
            }

            public uint GenerateSymbolId(string baseName)
            {
                var nextId = (uint)SymbolIds.Count;
                var key = $"{baseName}_{nextId}";
                SymbolIds[key] = nextId;
                return nextId;
            }

            public void AddRule(uint ruleId, List<LLamaGrammarElement> rule)
            {
                while (Rules.Count <= ruleId)
                {
                    Rules.Add(new List<LLamaGrammarElement>());
                }

                Rules[(int)ruleId] = rule;
            }
        }
    }
}
