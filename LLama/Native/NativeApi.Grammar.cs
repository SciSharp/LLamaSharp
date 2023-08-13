using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    using llama_token = Int32;

	public unsafe partial class NativeApi
    {
		//todo: LLAMA_API struct llama_grammar * llama_grammar_init(const llama_grammar_element** rules, size_t n_rules,size_t start_rule_index);

		/// <summary>
		/// Free all memory from the given SafeLLamaGrammarHandle
		/// </summary>
		/// <param name="grammar"></param>
		[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void llama_grammar_free(IntPtr grammar);

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
        public static extern void llama_grammar_accept_token(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, llama_token token);
	}
}
