﻿using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    using llama_token = Int32;

    public unsafe partial class NativeApi
    {
        /// <summary>
        /// Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, the logits must be directly extracted from the original generation context without being sorted.</param>
        /// <param name="guidanceCtx">A separate context from the same model. Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.</param>
        /// <param name="scale">Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_classifier_free_guidance(SafeLLamaContextHandle ctx, LLamaTokenDataArrayNative candidates, SafeLLamaContextHandle guidanceCtx, float scale);

        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="penalty"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, llama_token* last_tokens, ulong last_tokens_size, float penalty);

        /// <summary>
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="alpha_frequency"></param>
        /// <param name="alpha_presence"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, llama_token* last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence);

        /// <summary>
        /// Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, the logits must be directly extracted from the original generation context without being sorted.</param>
        /// <param name="guidance_ctx">A separate context from the same model. Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.</param>
        /// <param name="scale">Guidance strength. 1.0f means no guidance. Higher values mean stronger guidance.</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_classifier_free_guidance(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, SafeLLamaContextHandle guidance_ctx, float scale);

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
        /// Modify logits by temperature
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="temp"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_sample_temperature(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float temp);

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
        public static extern llama_token llama_sample_token_mirostat(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, int m, ref float mu);

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
        public static extern llama_token llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, ref float mu);

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern llama_token llama_sample_token_greedy(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern llama_token llama_sample_token(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    }
}
