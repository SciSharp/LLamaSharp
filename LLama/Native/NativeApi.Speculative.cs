using LLama.Speculative;
using System;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        /// <summary>
        /// Configuration parameters for initializing the native speculative decoding engine.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LlamaSpeculativeParams
        {
            /// <summary>
            /// The maximum number of draft tokens to propose per speculative burst.
            /// <para>For Draft-Simple models, a modest budget (e.g., 2-5) is recommended. For MTP models, this must match the available projection heads in the model's metadata.</para>
            /// </summary>
            public int n_draft;

            /// <summary>
            /// The maximum number of total context predictions to allocate. 
            /// </summary>
            public int n_predict;

            /// <summary>
            /// Determines the architectural routing for the speculative engine.
            /// <para>If <c>false</c> (Draft-Simple), standard autoregressive drafting is used. If <c>true</c> (MTP), the engine bypasses standard evaluation and explicitly routes target hidden states (<c>h_row</c>) into the draft context's projection heads.</para>
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool is_mtp;
        }

        /// <summary>
        /// Contains the verification results for a single sequence after a speculative decode pass.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LlamaSpeculativeResult
        {
            /// <summary>
            /// The native sequence identifier (<c>llama_seq_id</c>) these results map to.
            /// </summary>
            public int seq_id;

            /// <summary>
            /// The total number of tokens successfully evaluated and accepted during this burst.
            /// <para><b>Important:</b> This count includes BOTH the accepted draft tokens AND the 1 base target token sampled at the point of divergence. For example, if 3 drafts are accepted, this returns 4.</para>
            /// </summary>
            public int count;

            /// <summary>
            /// A fixed-size array holding the raw accepted token IDs. Only the first <see cref="count"/> elements are valid.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] tokens;
        }

        /// <summary>
        /// Initializes a new native speculative execution context that manages draft verification, M-RoPE safe cache rollbacks, and sequence alignment.
        /// </summary>
        /// <param name="ctx_tgt">A pointer to the primary target model's <c>llama_context</c>.</param>
        /// <param name="ctx_dft">A pointer to the draft model's <c>llama_context</c> (or the self-drafting MTP context).</param>
        /// <param name="sampler">A pointer to the native sampler chain to use for target verification. To guarantee mathematical correctness during evaluation, this should be a greedy sampler.</param>
        /// <param name="parameters">The configuration struct defining draft budgets and MTP modes.</param>
        /// <returns>An opaque native pointer to the <c>llama_speculative_context</c> instance.</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_speculative_init(
            IntPtr ctx_tgt,
            IntPtr ctx_dft,
            IntPtr sampler,
            ref LlamaSpeculativeParams parameters);

        /// <summary>
        /// Safely destroys the speculative context and frees associated native memory allocations.
        /// </summary>
        /// <param name="spec_ctx">The opaque native pointer to the <c>llama_speculative_context</c> instance to destroy.</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_speculative_free(IntPtr spec_ctx);

        /// <summary>
        /// Executes a complete multi-sequence speculative pipeline (Sync, Draft, Verify, and Rollback) in a single native API call.
        /// <para>For rejected draft sequences, this automatically purges invalid KV cache entries and executes an unconditional byte-level state rollback to protect hybrid/RNN architectures from <c>X &lt; Y</c> positional collision crashes.</para>
        /// </summary>
        /// <param name="spec_ctx">A safe handle wrapping the initialized speculative context.</param>
        /// <param name="batch">A reference to the current batch containing the prompt or recent inputs.</param>
        /// <param name="results">A pre-allocated buffer array to receive the evaluation outputs. The array length must be equal to or greater than <paramref name="max_results"/>.</param>
        /// <param name="max_results">The maximum number of sequence results to write into the <paramref name="results"/> array (usually matches the batch's active sequence count).</param>
        /// <returns>The number of populated <see cref="LlamaSpeculativeResult"/> structs written to the array.</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_speculative_decode(
            SafeLLamaSpeculativeContextHandle spec_ctx,
            ref LLamaNativeBatch batch,
            [Out, MarshalAs(UnmanagedType.LPArray)] LlamaSpeculativeResult[] results,
            int max_results);
    }
}