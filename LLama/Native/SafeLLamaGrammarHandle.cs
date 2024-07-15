using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LLama.Exceptions;
using LLama.Grammars;

namespace LLama.Native
{
    /// <summary>
    /// A safe reference to a `llama_grammar`
    /// </summary>
    public class SafeLLamaGrammarHandle
        : SafeLLamaHandleBase
    {
        #region construction/destruction
        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llama_grammar_free(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Create a new llama_grammar
        /// </summary>
        /// <param name="rules">A list of list of elements, each inner list makes up one grammar rule</param>
        /// <param name="startRuleIndex">The index (in the outer list) of the start rule</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLLamaGrammarHandle Create(IReadOnlyList<GrammarRule> rules, ulong startRuleIndex)
        {
            unsafe
            {
                var totalElements = rules.Sum(a => a.Elements.Count);
                var nrules = (ulong)rules.Count;

                // Borrow an array large enough to hold every single element
                // and another array large enough to hold a pointer to each rule
                var allElements = ArrayPool<LLamaGrammarElement>.Shared.Rent(totalElements);
                var rulePointers = ArrayPool<IntPtr>.Shared.Rent(rules.Count);
                try
                {
                    // We're taking pointers into `allElements` below, so this pin is required to fix
                    // that memory in place while those pointers are in use!
                    using var pin = allElements.AsMemory().Pin();

                    var elementIndex = 0;
                    var ruleIndex = 0;
                    foreach (var rule in rules)
                    {
                        // Save a pointer to the start of this rule
                        rulePointers[ruleIndex++] = (IntPtr)Unsafe.AsPointer(ref allElements[elementIndex]);

                        // Copy all of the rule elements into the flat array
                        foreach (var element in rule.Elements)
                            allElements[elementIndex++] = element;
                    }

                    // Sanity check some things that should be true if the copy worked as planned
                    Debug.Assert((ulong)ruleIndex == nrules);
                    Debug.Assert(elementIndex == totalElements);

                    // Make the actual call through to llama.cpp
                    fixed (void* ptr = rulePointers)
                    {
                        return Create((LLamaGrammarElement**)ptr, nrules, startRuleIndex);
                    }
                }
                finally
                {
                    ArrayPool<LLamaGrammarElement>.Shared.Return(allElements);
                    ArrayPool<IntPtr>.Shared.Return(rulePointers);
                }
            }
        }

        /// <summary>
        /// Create a new llama_grammar
        /// </summary>
        /// <param name="rules">rules list, each rule is a list of rule elements (terminated by a LLamaGrammarElementType.END element)</param>
        /// <param name="nrules">total number of rules</param>
        /// <param name="startRuleIndex">index of the start rule of the grammar</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static unsafe SafeLLamaGrammarHandle Create(LLamaGrammarElement** rules, ulong nrules, ulong startRuleIndex)
        {
            var grammar = NativeApi.llama_grammar_init(rules, nrules, startRuleIndex);
            if (grammar is null)
                throw new RuntimeError("Failed to create grammar from rules");

            return grammar;
        }
        #endregion

        /// <summary>
        /// Create a copy of this grammar instance
        /// </summary>
        /// <returns></returns>
        public SafeLLamaGrammarHandle Clone()
        {
            return NativeApi.llama_grammar_copy(this);
        }

        /// <summary>
        /// Accepts the sampled token into the grammar
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="token"></param>
        public void AcceptToken(SafeLLamaContextHandle ctx, LLamaToken token)
        {
            NativeApi.llama_grammar_accept_token(ctx, this, token);
        }
    }
}
