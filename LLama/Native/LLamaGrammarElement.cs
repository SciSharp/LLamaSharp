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
        /// modifies a preceding LLAMA_GRETYPE_CHAR or LLAMA_GRETYPE_CHAR_ALT to
        /// be an inclusive range ([a-z])
        /// </summary>
        CHAR_RNG_UPPER = 5,

        /// <summary>
        /// modifies a preceding LLAMA_GRETYPE_CHAR or
        /// LLAMA_GRETYPE_CHAR_RNG_UPPER to add an alternate char to match ([ab], [a-zA])
        /// </summary>
        CHAR_ALT = 6,
    };

    /// <summary>
    /// An element of a grammar
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaGrammarElement
    {
        /// <summary>
        /// The type of this element
        /// </summary>
        public LLamaGrammarElementType Type;

        /// <summary>
        /// Unicode code point or rule ID
        /// </summary>
        public uint Value;
    }
}
