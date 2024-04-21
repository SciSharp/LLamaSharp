using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Config
{
    /// <summary>
    /// Scheduler configuration.
    /// </summary>
    public class SchedulerConfig
    {
        /// <summary>
        /// Maximum number of tokens to be processed in a single iteration.
        /// </summary>
        public int MaxNumBatchedTokens { get; set; }

        /// <summary>
        /// Maximum number of sequences to be processed in a single iteration.
        /// </summary>
        public int MaxNumSequences { get; set; }

        /// <summary>
        /// Maximum length of a sequence (including prompt and generated text).
        /// </summary>
        public int MaxSequenceLength { get; set; }

        /// <summary>
        /// If True, prefill requests can be chunked based on the remaining max_num_batched_tokens.
        /// </summary>
        public bool EnableChunkedPrefill { get; set; }

        /// <summary>
        /// Apply a delay (of delay factor multiplied by previous prompt latency) before scheduling next prompt.
        /// </summary>
        public float DelayFactor { get; set; }

        public SchedulerConfig(int maxNumBatchedTokens, int maxNumSequences, int maxSequenceLength, bool enableChunkedPrefill = false, float delayFactor = .0f)
        {
            MaxNumBatchedTokens = maxNumBatchedTokens;
            MaxNumSequences = maxNumSequences;
            MaxSequenceLength = maxSequenceLength;
            EnableChunkedPrefill = enableChunkedPrefill;
            DelayFactor = delayFactor;
        }



        /// <summary>
        /// Verify if this configuration is valid and throw an exception if it's invalid.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void ThrowIfInvalid()
        {
            if (MaxNumBatchedTokens < MaxSequenceLength && !EnableChunkedPrefill)
            {
                throw new ArgumentException($"MaxNumBatchedTokens ({MaxNumBatchedTokens}) is smaller than " +
                    $"MaxSequenceLength ({MaxSequenceLength}). This effectively limits the maximum sequence length to " +
                    $"MaxNumBatchedTokens. Please increase MaxNumBatchedTokens, decrease MaxSequenceLength or enable chunked prefill.");
            }

            if (MaxNumBatchedTokens < MaxNumSequences)
            {
                throw new ArgumentException($"MaxNumBatchedTokens ({MaxNumBatchedTokens}) must be greater than or equal to " +
                    $"MaxNumSequences ({MaxNumSequences}).");
            }
        }
    }
}
