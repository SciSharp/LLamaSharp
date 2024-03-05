using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="penalty_repeat">Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.</param>
        /// <param name="penalty_freq">Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.</param>
        /// <param name="penalty_present">Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void llama_sample_repetition_penalties(SafeLLamaContextHandle ctx,
                                                                    ref LLamaTokenDataArrayNative candidates,
                                                                    LLamaToken* last_tokens, ulong last_tokens_size,
                                                                    float penalty_repeat,
                                                                    float penalty_freq,
                                                                    float penalty_present);

        /// <summary>
        /// Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="logits">Logits extracted from the original generation context.</param>
        /// <param name="logits_guidance">Logits extracted from a separate context from the same model.
        /// Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.</param>
        /// <param name="scale">Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.</param>
        public static void llama_sample_apply_guidance(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<float> logits_guidance, float scale)
        {
            if (logits == null)
                throw new ArgumentNullException(nameof(logits));
            if (logits_guidance == null)
                throw new ArgumentNullException(nameof(logits_guidance));
            if (logits.Length != ctx.VocabCount)
                throw new ArgumentException("Logits count must have equal context vocab size", nameof(logits));
            if (logits_guidance.Length != ctx.VocabCount)
                throw new ArgumentException("Guidance logits count must have equal context vocab size", nameof(logits_guidance));

            unsafe
            {
                fixed (float* logitsPtr = logits)
                fixed (float* logitsGuidancePtr = logits_guidance)
                    llama_sample_apply_guidance(ctx, logitsPtr, logitsGuidancePtr, scale);
            }
        }

        /// <summary>
        /// Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="logits">Logits extracted from the original generation context.</param>
        /// <param name="logits_guidance">Logits extracted from a separate context from the same model.
        /// Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.</param>
        /// <param name="scale">Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void llama_sample_apply_guidance(SafeLLamaContextHandle ctx, float* logits, float* logits_guidance, float scale);

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_softmax(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);

        /// <summary>
        /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="k"></param>
        /// <param name="min_keep"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_top_k(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, int k, ulong min_keep);

        /// <summary>
        /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_top_p(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);

        /// <summary>
        /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_min_p(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);


        /// <summary>
        /// @details Dynamic temperature implementation described in the paper https://arxiv.org/abs/2309.02772.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="min_temp"></param>
        /// <param name="max_temp"></param>
        /// <param name="exponent_val"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_entropy(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float min_temp, float max_temp, float exponent_val);


        /// <summary>
        /// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="z"></param>
        /// <param name="min_keep"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_tail_free(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float z, ulong min_keep);

        /// <summary>
        /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_typical(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);

        /// <summary>
        /// Dynamic temperature implementation described in the paper https://arxiv.org/abs/2309.02772.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="min_temp"></param>
        /// <param name="max_temp"></param>
        /// <param name="exponent_val"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_typical(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float min_temp, float max_temp, float exponent_val);

        /// <summary>
        /// Modify logits by temperature
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="temp"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_temp(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float temp);

        /// <summary>
        /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_sample_token_mirostat(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, int m, ref float mu);

        /// <summary>
        /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, ref float mu);

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_sample_token_greedy(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_sample_token(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    }
}
