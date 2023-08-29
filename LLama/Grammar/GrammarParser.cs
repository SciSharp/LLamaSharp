using LLama.Native;
using System;
using System.Collections.Generic;

namespace LLama.Grammar
{
    /// <summary>
    /// Source:
    /// https://github.com/ggerganov/llama.cpp/blob/6381d4e110bd0ec02843a60bbeb8b6fc37a9ace9/common/grammar-parser.cpp
    /// 
    /// The commit hash from URL is the actual commit hash that reflects current C# code.
    /// </summary>
    internal class GrammarParser
    {
        // NOTE: assumes valid utf8 (but checks for overrun)
        // copied from llama.cpp
        public Tuple<uint, ReadOnlyMemory<char>> DecodeUTF8(ReadOnlyMemory<char> src)
        {
            ReadOnlySpan<char> span = src.Span;
            int[] lookup = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 3, 4 };

            byte firstByte = (byte)span[0];
            byte highbits = (byte)(firstByte >> 4);
            int len = lookup[highbits];
            byte mask = (byte)((1 << (8 - len)) - 1);
            uint value = (uint)(firstByte & mask);

            int end = len;
            int pos = 1;

            for (; pos < end && pos < src.Length; pos++)
            {
                value = (uint)((value << 6) + ((byte)span[pos] & 0x3F));
            }

            ReadOnlyMemory<char> nextSpan = src.Slice(pos);

            return new Tuple<uint, ReadOnlyMemory<char>>(value, nextSpan);
        }

        public uint GetSymbolId(ParseState state, ReadOnlySpan<char> src, int len)
        {
            uint nextId = (uint)state.SymbolIds.Count;
            string key = src.Slice(0, len).ToString();

            if (state.SymbolIds.TryGetValue(key, out uint existingId))
            {
                return existingId;
            }
            else
            {
                state.SymbolIds[key] = nextId;
                return nextId;
            }
        }

        public uint GenerateSymbolId(ParseState state, string baseName)
        {
            uint nextId = (uint)state.SymbolIds.Count;
            string key = $"{baseName}_{nextId}";
            state.SymbolIds[key] = nextId;
            return nextId;
        }

        public void AddRule(ParseState state, uint ruleId, List<LLamaGrammarElement> rule)
        {
            while (state.Rules.Count <= ruleId)
            {
                state.Rules.Add(new List<LLamaGrammarElement>());
            }

            state.Rules[(int)ruleId] = rule;
        }

        public bool IsWordChar(char c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '-' || ('0' <= c && c <= '9');
        }

        public Tuple<uint, ReadOnlyMemory<char>> ParseHex(ReadOnlyMemory<char> src, int size)
        {
            int pos = 0;
            int end = size;
            uint value = 0;

            ReadOnlySpan<char> srcSpan = src.Span;

            for (; pos < end && pos < src.Length; pos++)
            {
                value <<= 4;
                char c = srcSpan[pos];
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
                throw new InvalidOperationException($"Expecting {size} hex chars at {src.ToString()}");
            }

            return new Tuple<uint, ReadOnlyMemory<char>>(value, src.Slice(pos));
        }

        public ReadOnlySpan<char> ParseSpace(ReadOnlySpan<char> src, bool newlineOk)
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

        public ReadOnlySpan<char> ParseName(ReadOnlySpan<char> src)
        {
            int pos = 0;
            while (pos < src.Length && IsWordChar(src[pos]))
            {
                pos++;
            }
            if (pos == 0)
            {
                throw new InvalidOperationException($"Expecting name at {src.ToString()}");
            }
            return src.Slice(pos);
        }

        public Tuple<uint, ReadOnlyMemory<char>> ParseChar(ReadOnlyMemory<char> src)
        {
            ReadOnlySpan<char> span = src.Span;

            if (span[0] == '\\')
            {
                switch (span[1])
                {
                    case 'x':
                        return ParseHex(src.Slice(2), 2);
                    case 'u':
                        return ParseHex(src.Slice(2), 4);
                    case 'U':
                        return ParseHex(src.Slice(2), 8);
                    case 't':
                        return new Tuple<uint, ReadOnlyMemory<char>>('\t', src.Slice(2));
                    case 'r':
                        return new Tuple<uint, ReadOnlyMemory<char>>('\r', src.Slice(2));
                    case 'n':
                        return new Tuple<uint, ReadOnlyMemory<char>>('\n', src.Slice(2));
                    case '\\':
                    case '"':
                    case '[':
                    case ']':
                        return new Tuple<uint, ReadOnlyMemory<char>>(span[1], src.Slice(2));
                    default:
                        throw new Exception("Unknown escape at " + src.ToString());
                }
            }
            else if (!span.IsEmpty)
            {
                return DecodeUTF8(src);
            }

            throw new Exception("Unexpected end of input");
        }

        public ReadOnlySpan<char> ParseSequence(
            ref ParseState state,
            ReadOnlyMemory<char> src,
            string ruleName,
            List<LLamaGrammarElement> outElements,
            bool isNested)
        {
            int lastSymStart = outElements.Count;
            var pos = src.Span;

            while (!pos.IsEmpty)
            {
                if (pos[0] == '"')  // literal string
                {
                    pos = pos.Slice(1);
                    lastSymStart = outElements.Count;

                    while (pos[0] != '"')
                    {
                        var charPair = ParseChar(src);
                        pos = charPair.Item2.Span;
                        outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.CHAR, Value = charPair.Item1 });
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

                    while (pos[0] != ']')
                    {
                        var charPair = ParseChar(src);
                        pos = charPair.Item2.Span;
                        var type = lastSymStart < outElements.Count ? LLamaGrammarElementType.CHAR_ALT : startType;

                        outElements.Add(new LLamaGrammarElement { Type = type, Value = charPair.Item1 });

                        if (pos[0] == '-' && pos[1] != ']')
                        {
                            var endCharPair = ParseChar(src.Slice(1));
                            pos = endCharPair.Item2.Span;
                            outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.CHAR_RNG_UPPER, Value = endCharPair.Item1 });
                        }
                    }
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (IsWordChar(pos[0]))  // rule reference
                {
                    var nameEnd = ParseName(pos);
                    uint refRuleId = GetSymbolId(state, pos, nameEnd.Length);
                    pos = ParseSpace(nameEnd, isNested);
                    lastSymStart = outElements.Count;
                    outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.RULE_REF, Value = refRuleId });
                }
                else if (pos[0] == '(')  // grouping
                {
                    // parse nested alternates into synthesized rule
                    pos = ParseSpace(pos.Slice(1), true);
                    uint subRuleId = GenerateSymbolId(state, ruleName);
                    pos = ParseAlternates(state, pos, ruleName, subRuleId, true);
                    lastSymStart = outElements.Count;
                    // output reference to synthesized rule
                    outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.RULE_REF, Value = subRuleId });
                    if (pos[0] != ')')
                    {
                        throw new Exception($"Expecting ')' at {new string(pos.ToArray())}");
                    }
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (pos[0] == '*' || pos[0] == '+' || pos[0] == '?') // repetition operator
                {
                    if (lastSymStart == outElements.Count)
                    {
                        throw new Exception($"Expecting preceding item to */+/? at {new string(pos.ToArray())}");
                    }

                    // apply transformation to previous symbol (lastSymStart to end) according to
                    // rewrite rules:
                    // S* --> S' ::= S S' |
                    // S+ --> S' ::= S S' | S
                    // S? --> S' ::= S |
                    uint subRuleId = GenerateSymbolId(state, ruleName);

                    List<LLamaGrammarElement> subRule = new List<LLamaGrammarElement>();

                    // add preceding symbol to generated rule
                    subRule.AddRange(outElements.GetRange(lastSymStart, outElements.Count - lastSymStart));

                    if (pos[0] == '*' || pos[0] == '+')
                    {
                        // cause generated rule to recurse
                        subRule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.RULE_REF, Value = subRuleId });
                    }

                    // mark start of alternate def
                    subRule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.ALT, Value = 0 });

                    if (pos[0] == '+')
                    {
                        // add preceding symbol as alternate only for '+' (otherwise empty)
                        subRule.AddRange(outElements.GetRange(lastSymStart, outElements.Count - lastSymStart));
                    }

                    subRule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.END, Value = 0 });

                    AddRule(state, subRuleId, subRule);

                    // in original rule, replace previous symbol with reference to generated rule
                    outElements.RemoveRange(lastSymStart, outElements.Count - lastSymStart);
                    outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.RULE_REF, Value = subRuleId });

                    pos = ParseSpace(pos.Slice(1), isNested);

                }
                else
                {
                    break;
                }
            }

            return pos;
        }

        public ReadOnlySpan<char> ParseAlternates(ParseState state, ReadOnlySpan<char> pos, string ruleName, uint subRuleId, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
