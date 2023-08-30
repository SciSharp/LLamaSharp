using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

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
        public uint DecodeUTF8(ref ReadOnlySpan<byte> src)
        {
            int[] lookup = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 3, 4 };

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

        public uint GetSymbolId(ParseState state, ReadOnlySpan<byte> src, int len)
        {
            uint nextId = (uint)state.SymbolIds.Count;
            string key = Encoding.UTF8.GetString(src.Slice(0, len).ToArray());

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

        public bool IsWordChar(byte c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '-' || ('0' <= c && c <= '9');
        }

        public uint ParseHex(ref ReadOnlySpan<byte> src, int size)
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
                throw new InvalidOperationException($"Expecting {size} hex chars at {Encoding.UTF8.GetString(src.ToArray())}");
            }
            src = src.Slice(pos);
            return value;
        }

        public ReadOnlySpan<byte> ParseSpace(ReadOnlySpan<byte> src, bool newlineOk)
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

        public ReadOnlySpan<byte> ParseName(ReadOnlySpan<byte> src)
        {
            int pos = 0;
            while (pos < src.Length && IsWordChar(src[pos]))
            {
                pos++;
            }
            if (pos == 0)
            {
                throw new InvalidOperationException($"Expecting name at {Encoding.UTF8.GetString(src.ToArray())}");
            }
            return src.Slice(pos);
        }

        public uint ParseChar(ref ReadOnlySpan<byte> src)
        {
            if (src[0] == '\\')
            {
                src = src.Slice(2);
                switch ((char)src[1])
                {
                    case 'x':
                        return ParseHex(ref src, 2);
                    case 'u':
                        return ParseHex(ref src, 4);
                    case 'U':
                        return ParseHex(ref src, 8);
                    case 't':
                        return '\t';
                    case 'r':
                        return '\r';
                    case 'n':
                        return '\n';
                    case '\\':
                    case '"':
                    case '[':
                    case ']':
                        return src[1];
                    default:
                        throw new Exception("Unknown escape at " + Encoding.UTF8.GetString(src.ToArray()));
                }
            }
            else if (!src.IsEmpty)
            {
                return DecodeUTF8(ref src);
            }

            throw new Exception("Unexpected end of input");
        }

        public ReadOnlySpan<byte> ParseSequence(
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

                    while (pos[0] != '"')
                    {
                        var charPair = ParseChar(ref pos);
                        outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.CHAR, Value = charPair });
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
                        var charPair = ParseChar(ref pos);
                        var type = lastSymStart < outElements.Count ? LLamaGrammarElementType.CHAR_ALT : startType;

                        outElements.Add(new LLamaGrammarElement { Type = type, Value = charPair });

                        if (pos[0] == '-' && pos[1] != ']')
                        {
                            pos = pos.Slice(1);
                            var endCharPair = ParseChar(ref pos);
                            outElements.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.CHAR_RNG_UPPER, Value = endCharPair });
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
                        throw new Exception($"Expecting ')' at {Encoding.UTF8.GetString(pos.ToArray())}");
                    }
                    pos = ParseSpace(pos.Slice(1), isNested);
                }
                else if (pos[0] == '*' || pos[0] == '+' || pos[0] == '?') // repetition operator
                {
                    if (lastSymStart == outElements.Count)
                    {
                        throw new Exception($"Expecting preceding item to */+/? at {Encoding.UTF8.GetString(pos.ToArray())}");
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

        public ReadOnlySpan<byte> ParseAlternates(
            ParseState state,
            ReadOnlySpan<byte> src,
            string ruleName,
            uint ruleId,
            bool isNested)
        {
            var rule = new List<LLamaGrammarElement>();
            ReadOnlySpan<byte> pos = ParseSequence(state, src, ruleName, rule, isNested);

            while (pos[0] == '|')
            {
                rule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.ALT, Value = 0 });
                pos = ParseSpace(pos.Slice(1), true);
                pos = ParseSequence(state, pos, ruleName, rule, isNested);
            }

            rule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.END, Value = 0 });
            AddRule(state, ruleId, rule);

            return pos;
        }
    }
}
