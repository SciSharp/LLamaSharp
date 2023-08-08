using System;
using System.Text.Json.Serialization;

namespace LLama.Executors
{
    using llama_token = Int32;

    /// <summary>
    /// The desciptor of the state of the instruct executor.
    /// </summary>
    public class InstructExecutorState : ExecutorBaseState
    {
        /// <summary>
        /// Whether the executor is running for the first time (running the prompt).
        /// </summary>
        [JsonPropertyName("is_prompt_run")]
        public bool IsPromptRun { get; set; }
        /// <summary>
        /// Instruction prefix tokens.
        /// </summary>
        [JsonPropertyName("inp_pfx")]
        public llama_token[] InputPrefixTokens { get; set; }
        /// <summary>
        /// Instruction suffix tokens.
        /// </summary>
        [JsonPropertyName("inp_sfx")]
        public llama_token[] InputSuffixTokens { get; set; }
    }
}
