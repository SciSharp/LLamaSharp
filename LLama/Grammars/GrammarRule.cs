using System;
using System.Collections.Generic;
using LLama.Exceptions;
using LLama.Native;

namespace LLama.Grammars
{
    /// <summary>
    /// A single rule in a <see cref="Grammar"/>
    /// </summary>
    public sealed record GrammarRule
    {
        /// <summary>
        /// Name of this rule
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The elements of this grammar rule
        /// </summary>
        public IReadOnlyList<LLamaGrammarElement> Elements { get; }

        /// <summary>
        /// Create a new GrammarRule containing the given elements
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        /// <exception cref="ArgumentException"></exception>
        public GrammarRule(string name, IReadOnlyList<LLamaGrammarElement> elements)
        {
            Validate(elements, name);

            Name = name;
            Elements = elements;
        }

        private static void Validate(IReadOnlyList<LLamaGrammarElement> elements, string name)
        {
            if (elements.Count == 0)
                throw new ArgumentException("Cannot create a GrammarRule with zero elements", nameof(elements));
            if (elements[elements.Count - 1].Type != LLamaGrammarElementType.END)
                throw new ArgumentException("Last grammar element must be END", nameof(elements));

            for (var i = 0; i < elements.Count; i++)
            {
                switch (elements[i].Type)
                {
                    case LLamaGrammarElementType.END:
                        if (i != elements.Count - 1)
                            throw new GrammarUnexpectedEndElement(name, i);
                        continue;

                    case LLamaGrammarElementType.CHAR_RNG_UPPER:
                        if (i == 0 || !elements[i - 1].IsCharElement())
                            throw new GrammarUnexpectedCharRngElement(name, i);
                        break;
                    case LLamaGrammarElementType.CHAR_ALT:
                        if (i == 0 || !elements[i - 1].IsCharElement())
                            throw new GrammarUnexpectedCharAltElement(name, i);
                        break;

                    case LLamaGrammarElementType.ALT:
                    case LLamaGrammarElementType.RULE_REF:
                    case LLamaGrammarElementType.CHAR:
                    case LLamaGrammarElementType.CHAR_NOT:
                        break;

                    default:
                        throw new ArgumentException($"Unknown grammar element type: '{elements[i].Type}'", nameof(elements));
                }
            }
        }
    }
}
