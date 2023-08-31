﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// grammar element type
    /// </summary>
    public enum LLamaGrammarElementType
    {
        /// <summary>
        /// end of rule definition
        /// </summary>
        END = 0,

        /// <summary>
        /// start of alternate definition for rule
        /// </summary>
        ALT = 1,

        /// <summary>
        /// non-terminal element: reference to rule
        /// </summary>
        RULE_REF = 2,

        /// <summary>
        /// terminal element: character (code point)
        /// </summary>
        CHAR = 3,

        /// <summary>
        /// inverse char(s) ([^a], [^a-b] [^abc])
        /// </summary>
        CHAR_NOT = 4,

        /// <summary>
        /// modifies a preceding CHAR or CHAR_ALT to
        /// be an inclusive range ([a-z])
        /// </summary>
        CHAR_RNG_UPPER = 5,

        /// <summary>
        /// modifies a preceding CHAR or
        /// CHAR_RNG_UPPER to add an alternate char to match ([ab], [a-zA])
        /// </summary>
        CHAR_ALT = 6,
    };

    /// <summary>
    /// An element of a grammar
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{Type} {Value}")]
    public readonly struct LLamaGrammarElement
        : IEquatable<LLamaGrammarElement>
    {
        /// <summary>
        /// The type of this element
        /// </summary>
        public readonly LLamaGrammarElementType Type;

        /// <summary>
        /// Unicode code point or rule ID
        /// </summary>
        public readonly uint Value;

        /// <summary>
        /// Construct a new LLamaGrammarElement
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public LLamaGrammarElement(LLamaGrammarElementType type, uint value)
        {
            Type = type;
            Value = value;
        }

        /// <inheritdoc />
        public bool Equals(LLamaGrammarElement other)
        {
            if (Type != other.Type)
                return false;

            // No need to compare values for the END rule
            if (Type == LLamaGrammarElementType.END)
                return true;

            return Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is LLamaGrammarElement other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 2999;
                hash = hash * 7723 + (int)Type;
                hash = hash * 7723 + (int)Value;
                return hash;
            }
        }

        internal bool IsCharElement()
        {
            switch (Type)
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
