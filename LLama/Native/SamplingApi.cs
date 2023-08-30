﻿using System;

namespace LLama.Native
{
    using llama_token = Int32;
    public unsafe class SamplingApi
    {
        /// <summary>
        /// Apply grammar rules to candidate tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="grammar"></param>
        public static void llama_sample_grammar(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, SafeLLamaGrammarHandle grammar)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_grammar(ctx, ref st, grammar);
        }

        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="penalty"></param>
        [Obsolete("last_tokens_size parameter is no longer needed")]
        public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<llama_token> last_tokens, ulong last_tokens_size, float penalty)
        {
            llama_sample_repetition_penalty(ctx, candidates, last_tokens, penalty);
        }

        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="penalty"></param>
        public static void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<llama_token> last_tokens, float penalty)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            using var last_tokens_handle = last_tokens.Pin();

            NativeApi.llama_sample_repetition_penalty(ctx, ref st, (int*)last_tokens_handle.Pointer, (ulong)last_tokens.Length, penalty);
        }

        /// <summary>
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="alpha_frequency"></param>
        /// <param name="alpha_presence"></param>
        [Obsolete("last_tokens_size parameter is no longer needed")]
        public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<llama_token> last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence)
        {
            llama_sample_frequency_and_presence_penalties(ctx, candidates, last_tokens, alpha_frequency, alpha_presence);
        }

        /// <summary>
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="alpha_frequency"></param>
        /// <param name="alpha_presence"></param>
        public static void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, Memory<llama_token> last_tokens, float alpha_frequency, float alpha_presence)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            using var last_tokens_handle = last_tokens.Pin();

            NativeApi.llama_sample_frequency_and_presence_penalties(ctx, ref st, (int*)last_tokens_handle.Pointer, (ulong)last_tokens.Length, alpha_frequency, alpha_presence);
        }

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        public static void llama_sample_softmax(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_softmax(ctx, ref st);
        }

        /// <summary>
        /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="k"></param>
        /// <param name="min_keep"></param>
        public static void llama_sample_top_k(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, int k, ulong min_keep)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_top_k(ctx, ref st, k, min_keep);
        }

        /// <summary>
        /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        public static void llama_sample_top_p(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_top_p(ctx, ref st, p, min_keep);
        }

        /// <summary>
        /// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="z"></param>
        /// <param name="min_keep"></param>
        public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float z, ulong min_keep)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_tail_free(ctx, ref st, z, min_keep);
        }

        /// <summary>
        /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        public static void llama_sample_typical(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_typical(ctx, ref st, p, min_keep);
        }

        public static void llama_sample_temperature(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float temp)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            NativeApi.llama_sample_temperature(ctx, ref st, temp);
        }

        /// <summary>
        /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `LLamaTokenData` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        public static llama_token llama_sample_token_mirostat(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, int m, ref float mu)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            return NativeApi.llama_sample_token_mirostat(ctx, ref st, tau, eta, m, ref mu);
        }

        /// <summary>
        /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `LLamaTokenData` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        public static llama_token llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, ref float mu)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            return NativeApi.llama_sample_token_mirostat_v2(ctx, ref st, tau, eta, ref mu);
        }

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        public static llama_token llama_sample_token_greedy(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            return NativeApi.llama_sample_token_greedy(ctx, ref st);
        }

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        public static llama_token llama_sample_token(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            using var handle = LLamaTokenDataArrayNative.Create(candidates, out var st);
            return NativeApi.llama_sample_token(ctx, ref st);
        }
    }
}
