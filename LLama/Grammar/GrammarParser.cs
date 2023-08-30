using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LLama.Grammar
{
    /// <summary>
    /// Source:
    /// https://github.com/ggerganov/llama.cpp/blob/6381d4e110bd0ec02843a60bbeb8b6fc37a9ace9/common/grammar-parser.cpp
    /// 
    /// The commit hash from URL is the actual commit hash that reflects current C# code.
    /// </summary>
    public class GrammarParser
    {
        // NOTE: assumes valid utf8 (but checks for overrun)
        // copied from llama.cpp
        private uint DecodeUTF8(ref ReadOnlySpan<byte> src)
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

        private uint GetSymbolId(ParseState state, ReadOnlySpan<byte> src, int len)
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

        private uint GenerateSymbolId(ParseState state, string baseName)
        {
            uint nextId = (uint)state.SymbolIds.Count;
            string key = $"{baseName}_{nextId}";
            state.SymbolIds[key] = nextId;
            return nextId;
        }

        private void AddRule(ParseState state, uint ruleId, List<LLamaGrammarElement> rule)
        {
            while (state.Rules.Count <= ruleId)
            {
                state.Rules.Add(new List<LLamaGrammarElement>());
            }

            state.Rules[(int)ruleId] = rule;
        }

        private bool IsWordChar(byte c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '-' || ('0' <= c && c <= '9');
        }

        private uint ParseHex(ref ReadOnlySpan<byte> src, int size)
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

        private ReadOnlySpan<byte> ParseSpace(ReadOnlySpan<byte> src, bool newlineOk)
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

        private ReadOnlySpan<byte> ParseName(ReadOnlySpan<byte> src)
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

        private uint ParseChar(ref ReadOnlySpan<byte> src)
        {
            if (src[0] == '\\')
            {
                var chr = src[1];
                src = src.Slice(2);
                switch (chr)
                {
                    case (byte)'x':
                        return ParseHex(ref src, 2);
                    case (byte)'u':
                        return ParseHex(ref src, 4);
                    case (byte)'U':
                        return ParseHex(ref src, 8);
                    case (byte)'t':
                        return '\t';
                    case (byte)'r':
                        return '\r';
                    case (byte)'n':
                        return '\n';
                    case (byte)'\\':
                    case (byte)'"':
                    case (byte)'[':
                    case (byte)']':
                        return chr;
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

                    while (!pos.IsEmpty && pos[0] != ']')
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
                rule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.ALT, Value = 0 });
                pos = ParseSpace(pos.Slice(1), true);
                pos = ParseSequence(state, pos, ruleName, rule, isNested);
            }

            rule.Add(new LLamaGrammarElement { Type = LLamaGrammarElementType.END, Value = 0 });
            AddRule(state, ruleId, rule);

            return pos;
        }

        private ReadOnlySpan<byte> ParseRule(ParseState state, ReadOnlySpan<byte> src)
        {
            ReadOnlySpan<byte> nameEnd = ParseName(src);
            ReadOnlySpan<byte> pos = ParseSpace(nameEnd, false);
            int nameLen = src.Length - nameEnd.Length;
            uint ruleId = GetSymbolId(state, src.Slice(0, nameLen), nameLen);
            string name = Encoding.UTF8.GetString(src.Slice(0, nameLen).ToArray());

            if (!(pos[0] == ':' && pos[1] == ':' && pos[2] == '='))
            {
                throw new Exception($"Expecting ::= at {Encoding.UTF8.GetString(pos.ToArray())}");
            }
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
                throw new Exception($"Expecting newline or end at {Encoding.UTF8.GetString(pos.ToArray())}");
            }
            return ParseSpace(pos, true);
        }

        public ParseState Parse(string input)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(input);
                ReadOnlySpan<byte> src = new ReadOnlySpan<byte>(byteArray);
                ParseState state = new ParseState();
                ReadOnlySpan<byte> pos = ParseSpace(src, true);

                while (!pos.IsEmpty)
                {
                    pos = ParseRule(state, pos);
                }

                return state;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine($"{nameof(Parse)}: error parsing grammar: {err.Message}");
                return new ParseState();
            }
        }

        private void PrintGrammarChar(StreamWriter file, uint c)
        {
            if (c >= 0x20 && c <= 0x7F)
            {
                file.Write((char)c);
            }
            else
            {
                // cop out of encoding UTF-8
                file.Write($"<U+{c:X4}>");
            }
        }

        private bool IsCharElement(LLamaGrammarElement elem)
        {
            switch (elem.Type)
            {
                case LLamaGrammarElementType.CHAR:
                case LLamaGrammarElementType.CHAR_NOT:
                case LLamaGrammarElementType.CHAR_ALT:
                case LLamaGrammarElementType.CHAR_RNG_UPPER:
                    return true;
                default:
                    return false;
            }
        }

        public void PrintRuleBinary(StreamWriter file, List<LLamaGrammarElement> rule)
        {
            foreach (var elem in rule)
            {
                switch (elem.Type)
                {
                    case LLamaGrammarElementType.END: file.Write("END"); break;
                    case LLamaGrammarElementType.ALT: file.Write("ALT"); break;
                    case LLamaGrammarElementType.RULE_REF: file.Write("RULE_REF"); break;
                    case LLamaGrammarElementType.CHAR: file.Write("CHAR"); break;
                    case LLamaGrammarElementType.CHAR_NOT: file.Write("CHAR_NOT"); break;
                    case LLamaGrammarElementType.CHAR_RNG_UPPER: file.Write("CHAR_RNG_UPPER"); break;
                    case LLamaGrammarElementType.CHAR_ALT: file.Write("CHAR_ALT"); break;
                }
                switch (elem.Type)
                {
                    case LLamaGrammarElementType.END:
                    case LLamaGrammarElementType.ALT:
                    case LLamaGrammarElementType.RULE_REF:
                        file.Write($"({elem.Value}) ");
                        break;
                    case LLamaGrammarElementType.CHAR:
                    case LLamaGrammarElementType.CHAR_NOT:
                    case LLamaGrammarElementType.CHAR_RNG_UPPER:
                    case LLamaGrammarElementType.CHAR_ALT:
                        file.Write("(\"");
                        PrintGrammarChar(file, elem.Value);
                        file.Write("\") ");
                        break;
                }
            }
            file.WriteLine();
        }

        private void PrintRule(
            StreamWriter file,
            uint ruleId,
            List<LLamaGrammarElement> rule,
            Dictionary<uint, string> symbolIdNames)
        {
            if (rule.Count == 0 || rule[rule.Count - 1].Type != LLamaGrammarElementType.END)
            {
                throw new InvalidOperationException(
                    $"Malformed rule, does not end with LLamaGrammarElementType.END: {ruleId}");
            }

            file.Write($"{symbolIdNames[ruleId]} ::= ");

            for (int i = 0, end = rule.Count - 1; i < end; i++)
            {
                var elem = rule[i];
                switch (elem.Type)
                {
                    case LLamaGrammarElementType.END:
                        throw new InvalidOperationException(
                            $"Unexpected end of rule: {ruleId}, {i}");
                    case LLamaGrammarElementType.ALT:
                        file.Write("| ");
                        break;
                    case LLamaGrammarElementType.RULE_REF:
                        file.Write($"{symbolIdNames[elem.Value]} ");
                        break;
                    case LLamaGrammarElementType.CHAR:
                        file.Write("[");
                        PrintGrammarChar(file, elem.Value);
                        break;
                    case LLamaGrammarElementType.CHAR_NOT:
                        file.Write("[^");
                        PrintGrammarChar(file, elem.Value);
                        break;
                    case LLamaGrammarElementType.CHAR_RNG_UPPER:
                        if (i == 0 || !IsCharElement(rule[i - 1]))
                        {
                            throw new InvalidOperationException(
                                $"LLamaGrammarElementType.CHAR_RNG_UPPER without preceding char: {ruleId},{i}");
                        }
                        file.Write("-");
                        PrintGrammarChar(file, elem.Value);
                        break;
                    case LLamaGrammarElementType.CHAR_ALT:
                        if (i == 0 || !IsCharElement(rule[i - 1]))
                        {
                            throw new InvalidOperationException(
                                $"LLamaGrammarElementType.CHAR_ALT without preceding char: {ruleId},{i}");
                        }
                        PrintGrammarChar(file, elem.Value);
                        break;

                }

                if (IsCharElement(elem))
                {
                    switch (rule[i + 1].Type)
                    {
                        case LLamaGrammarElementType.CHAR_ALT:
                        case LLamaGrammarElementType.CHAR_RNG_UPPER:
                            break;
                        default:
                            file.Write("] ");
                            break;
                    }
                }
            }
            file.WriteLine();
        }

        public void PrintGrammar(StreamWriter file, ParseState state)
        {
            try
            {
                Dictionary<uint, string> symbolIdNames = new Dictionary<uint, string>();
                foreach (var kv in state.SymbolIds)
                {
                    symbolIdNames[kv.Value] = kv.Key;
                }
                for (int i = 0, end = state.Rules.Count; i < end; i++)
                {
                    PrintRule(file, (uint)i, state.Rules[i], symbolIdNames);
                }
            }
            catch (Exception err)
            {
                Console.Error.WriteLine($"\nError printing grammar: {err.Message}");
            }
        }
    }
}
