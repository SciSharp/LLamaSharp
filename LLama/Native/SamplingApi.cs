using System;

#pragma warning disable IDE1006 // Naming Styles

namespace LLama.Native
{
    using llama_token = Int32;

    /// <summary>
    /// Direct translation of the llama.cpp sampling API
    /// </summary>
    public class SamplingApi
    {
        /// <summary>
        /// Apply grammar rules to candidate tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="grammar"></param>
        [Obsolete("use LLamaTokenDataArray ApplyGrammar method")]
        public static void llama_sample_grammar(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, SafeLLamaGrammarHandle grammar)
        {
            candidates.ApplyGrammar(ctx, grammar);
        }

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        [Obsolete("use LLamaTokenDataArray Softmax method")]
        public static void llama_sample_softmax(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            candidates.Softmax(ctx);
        }

        /// <summary>
        /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="k"></param>
        /// <param name="min_keep"></param>
        [Obsolete("use LLamaTokenDataArray TopK method")]
        public static void llama_sample_top_k(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, int k, ulong min_keep)
        {
            candidates.TopK(ctx, k, min_keep);
        }

        /// <summary>
        /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [Obsolete("use LLamaTokenDataArray TopP method")]
        public static void llama_sample_top_p(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
        {
            candidates.TopP(ctx, p, min_keep);
        }

        /// <summary>
        /// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="z"></param>
        /// <param name="min_keep"></param>
        [Obsolete("use LLamaTokenDataArray TailFree method")]
        public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float z, ulong min_keep)
        {
            candidates.TailFree(ctx, z, min_keep);
        }

        /// <summary>
        /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [Obsolete("use LLamaTokenDataArray LocallyTypical method")]
        public static void llama_sample_typical(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float p, ulong min_keep)
        {
            candidates.LocallyTypical(ctx, p, min_keep);
        }

        /// <summary>
        /// Sample with temperature.
        /// As temperature increases, the prediction becomes diverse but also vulnerable to hallucinations -- generating tokens that are sensible but not factual
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="temp"></param>
        [Obsolete("use LLamaTokenDataArray Temperature() method")]
        public static void llama_sample_temperature(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float temp)
        {
            candidates.Temperature(ctx, temp);
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
        [Obsolete("use LLamaTokenDataArray SampleTokenMirostat() method")]
        public static llama_token llama_sample_token_mirostat(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, int m, ref float mu)
        {
            return candidates.SampleTokenMirostat(ctx, tau, eta, m, ref mu);
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
        [Obsolete("use LLamaTokenDataArray SampleTokenMirostat2() method")]
        public static llama_token llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, float tau, float eta, ref float mu)
        {
            return candidates.SampleTokenMirostat2(ctx, tau, eta, ref mu);
        }

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [Obsolete("Use LLamaTokenDataArray SampleTokenGreedy() method")]
        public static llama_token llama_sample_token_greedy(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            return candidates.SampleTokenGreedy(ctx);
        }

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to LLamaTokenDataArray</param>
        /// <returns></returns>
        [Obsolete("use LLamaTokenDataArray SampleToken() method")]
        public static llama_token llama_sample_token(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
        {
            return candidates.SampleToken(ctx);
        }
    }
}
