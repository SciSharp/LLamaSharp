using System;

namespace LLama.Native
{
    /// <summary>
    /// A safe reference to a `llama_grammar`
    /// </summary>
    public class SafeLLamaGrammarHandle
        : SafeLLamaHandleBase
    {
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
    }
}
