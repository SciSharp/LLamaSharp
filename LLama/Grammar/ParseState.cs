using LLama.Exceptions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;

namespace LLama.Grammar
{
    /// <summary>
    /// Source:
    /// https://github.com/ggerganov/llama.cpp/blob/6381d4e110bd0ec02843a60bbeb8b6fc37a9ace9/common/grammar-parser.h
    /// 
    /// The commit hash from URL is the actual commit hash that reflects current C# code.
    /// </summary>
    public class ParseState
    {
        public SortedDictionary<string, uint> SymbolIds { get; } = new SortedDictionary<string, uint>();
        public List<List<LLamaGrammarElement>> Rules { get; } = new List<List<LLamaGrammarElement>>();

        public IEnumerable<List<LLamaGrammarElement>> CRules()
        {
            foreach (var rule in Rules)
            {
                yield return rule;
            }
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
            catch(Exception err)
            {
                Console.Error.WriteLine($"\nError printing grammar: {err.Message}");
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
                throw new GrammarFormatException(
                    $"Malformed rule, does not end with LLamaGrammarElementType.END: {ruleId}");
            }

            file.Write($"{symbolIdNames[ruleId]} ::= ");

            for (int i = 0, end = rule.Count - 1; i < end; i++)
            {
                var elem = rule[i];
                switch (elem.Type)
                {
                    case LLamaGrammarElementType.END:
                        throw new GrammarFormatException(
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
                            throw new GrammarFormatException(
                                $"LLamaGrammarElementType.CHAR_RNG_UPPER without preceding char: {ruleId},{i}");
                        }
                        file.Write("-");
                        PrintGrammarChar(file, elem.Value);
                        break;
                    case LLamaGrammarElementType.CHAR_ALT:
                        if (i == 0 || !IsCharElement(rule[i - 1]))
                        {
                            throw new GrammarFormatException(
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
    }
}
