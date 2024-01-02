using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
	public static partial class NativeApi
    {
		/// <summary>
		/// Create a new grammar from the given set of grammar rules
		/// </summary>
		/// <param name="rules"></param>
		/// <param name="n_rules"></param>
		/// <param name="start_rule_index"></param>
		/// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr llama_grammar_init(LLamaGrammarElement** rules, ulong n_rules, ulong start_rule_index);

        /// <summary>
        /// Free all memory from the given SafeLLamaGrammarHandle
        /// </summary>
        /// <param name="grammar"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void llama_grammar_free(IntPtr grammar);

		/// <summary>
		/// Create a copy of an existing grammar instance
		/// </summary>
		/// <param name="grammar"></param>
		/// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_grammar_copy(SafeLLamaGrammarHandle grammar);

		/// <summary>
		/// Apply constraints from grammar
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="candidates"></param>
		/// <param name="grammar"></param>
		[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_grammar(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, SafeLLamaGrammarHandle grammar);

		/// <summary>
		/// Accepts the sampled token into the grammar
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="grammar"></param>
		/// <param name="token"></param>
		[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_grammar_accept_token(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, LLamaToken token);
	}
}
