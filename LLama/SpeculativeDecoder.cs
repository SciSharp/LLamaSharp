using System;
using LLama.Native;
using LLama.Exceptions;

namespace LLama.Speculative
{
    /// <summary>
    /// A managed wrapper for the native speculative decoding engine.
    /// <para>This class orchestrates the synchronization between a target context and a draft context (or MTP projection heads). It handles burst generation, native mathematical verification, and automated KV cache rollbacks to accelerate inference.</para>
    /// </summary>
    public sealed class SpeculativeDecoder : IDisposable
    {
        private readonly SafeLLamaSpeculativeContextHandle _specHandle;
        private readonly SafeLLamaSamplerChainHandle _sampler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeculativeDecoder"/> class.
        /// <para>This wrapper safely orchestrates the native speculative verification loop, managing target evaluations, draft proposals, and state rollbacks.</para>
        /// </summary>
        /// <param name="targetContext">A safe handle to the primary target model's context used for verifying draft tokens.</param>
        /// <param name="draftContext">A safe handle to the secondary context used for generating draft tokens. In MTP mode, this represents the target model's MTP projection heads.</param>
        /// <param name="draftTokens">The maximum number of draft tokens to propose per speculative burst.</param>
        /// <param name="useMtp">Set to <c>true</c> to enable Multi-Token Prediction (Self-Speculation) routing, bypassing standard draft evaluation.</param>
        /// <exception cref="RuntimeError">Thrown if the underlying native speculative context fails to initialize.</exception>
        public SpeculativeDecoder(
             SafeLLamaContextHandle targetContext,
             SafeLLamaContextHandle draftContext,
             int draftTokens = 16,
             bool useMtp = false)
        {
            // Null guard to catch context creation failures gracefully
            if (targetContext.DangerousGetHandle() == IntPtr.Zero || draftContext.DangerousGetHandle() == IntPtr.Zero)
                throw new RuntimeError("Cannot initialize SpeculativeDecoder: Context pointer is null. llama_init_from_model failed.");

            var samplerParams = LLamaSamplerChainParams.Default();
            _sampler = SafeLLamaSamplerChainHandle.Create(samplerParams);
            _sampler.AddGreedySampler();

            var param = new NativeApi.LlamaSpeculativeParams
            {
                n_draft = draftTokens,
                n_predict = -1,
                is_mtp = useMtp,
            };

            IntPtr rawSpec = NativeApi.llama_speculative_init(
                targetContext.DangerousGetHandle(),
                draftContext.DangerousGetHandle(),
                _sampler.DangerousGetHandle(),
                ref param
            );

            if (rawSpec == IntPtr.Zero)
            {
                _sampler.Dispose();
                throw new RuntimeError("Failed to initialize native llama_speculative_context.");
            }

            _specHandle = new SafeLLamaSpeculativeContextHandle(rawSpec);
        }

        /// <summary>
        /// Executes a complete speculative decoding pipeline (Draft, Verify, and Rollback) for the provided batch.
        /// <para>This method natively evaluates the draft tokens against the target model. It automatically purges rejected tokens from the KV cache and handles state rollbacks to preserve mathematical consistency.</para>
        /// </summary>
        /// <param name="batch">The current batch of tokens to evaluate and use as the base for speculative drafting.</param>
        /// <param name="maxSequences">The maximum number of sequence results to allocate memory for and process. Defaults to 128.</param>
        /// <returns>An array of <see cref="NativeApi.LlamaSpeculativeResult"/> structs containing the accepted tokens and verification counts for each active sequence.</returns>
        public NativeApi.LlamaSpeculativeResult[] Decode(LLamaBatch batch, int maxSequences = 128)
        {
            var resultsBuffer = new NativeApi.LlamaSpeculativeResult[maxSequences];

            using (var pin = batch.ToNativeBatch(out var nativeBatch))
            {
                int count = NativeApi.llama_speculative_decode(
                    _specHandle,
                    ref nativeBatch,
                    resultsBuffer,
                    maxSequences);

                // If native API throws a native error
                if (count < 0)
                    throw new LLamaDecodeError((DecodeResult)count);

                // If it evaluated successfully but produced no speculative tokens (e.g., Prompt phase)
                if (count == 0)
                    return Array.Empty<NativeApi.LlamaSpeculativeResult>();

                var finalResults = new NativeApi.LlamaSpeculativeResult[count];
                Array.Copy(resultsBuffer, finalResults, count);
                return finalResults;
            }
        }

        /// <summary>
        /// Disposes the speculative decoder, safely releasing the underlying unmanaged native context and its allocated memory.
        /// </summary>
        public void Dispose()
        {
            _specHandle?.Dispose();
            _sampler?.Dispose();
        }
    }
}
