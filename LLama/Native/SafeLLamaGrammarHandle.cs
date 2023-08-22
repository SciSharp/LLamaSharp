﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A safe reference to a `llama_grammar`
    /// </summary>
    public class SafeLLamaGrammarHandle
        : SafeLLamaHandleBase
    {
        #region construction/destruction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        internal SafeLLamaGrammarHandle(IntPtr handle)
            : base(handle)
        {
        }

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
        /// <param name="start_rule_index">The index (in the outer list) of the start rule</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLLamaGrammarHandle Create(IReadOnlyList<IReadOnlyList<LLamaGrammarElement>> rules, ulong start_rule_index)
        {
            unsafe
            {
                var totalElements = rules.Sum(a => a.Count);
                var nrules = (ulong)rules.Count;

                // Borrow an array large enough to hold every single element
                // and another array large enough to hold a pointer to each rule
                var allElements = ArrayPool<LLamaGrammarElement>.Shared.Rent(totalElements);
                var pointers = ArrayPool<IntPtr>.Shared.Rent(rules.Count);
                try
                {
                    fixed (LLamaGrammarElement* allElementsPtr = allElements)
                    {
                        var elementIndex = 0;
                        var pointerIndex = 0;
                        foreach (var rule in rules)
                        {
                            // Save a pointer to the start of this rule
                            pointers[pointerIndex++] = (IntPtr)(allElementsPtr + elementIndex);

                            // Copy all of the rule elements into the flat array
                            foreach (var element in rule)
                                allElementsPtr[elementIndex++] = element;
                        }

                        // Sanity check some things that should be true if the copy worked as planned
                        Debug.Assert((ulong)pointerIndex == nrules);
                        Debug.Assert(elementIndex == totalElements);

                        // Make the actual call through to llama.cpp
                        fixed (void* ptr = pointers)
                        {
                            return Create((LLamaGrammarElement**)ptr, nrules, start_rule_index);
                        }
                    }
                }
                finally
                {
                    ArrayPool<LLamaGrammarElement>.Shared.Return(allElements);
                    ArrayPool<IntPtr>.Shared.Return(pointers);
                }
            }
        }

        /// <summary>
        /// Create a new llama_grammar
        /// </summary>
        /// <param name="rules">rules list, each rule is a list of rule elements (terminated by a LLamaGrammarElementType.END element)</param>
        /// <param name="nrules">total number of rules</param>
        /// <param name="start_rule_index">index of the start rule of the grammar</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static unsafe SafeLLamaGrammarHandle Create(LLamaGrammarElement** rules, ulong nrules, ulong start_rule_index)
        {
            var grammar_ptr = NativeApi.llama_grammar_init(rules, nrules, start_rule_index);
            if (grammar_ptr == IntPtr.Zero)
                throw new RuntimeError("Failed to create grammar from rules");

            return new(grammar_ptr);
        }
        #endregion
    }
}
