using System;
using System.Text.Json.Serialization;

namespace LLama.Executors
{
    using llama_token = Int32;

    /// <summary>
    /// The descriptor of the state of the interactive executor.
    /// </summary>
    public class InteractiveExecutorState : ExecutorBaseState
    {
        /// <summary>
        /// Whether the executor is running for the first time (running the prompt).
        /// </summary>
        [JsonPropertyName("is_prompt_run")]
        public bool IsPromptRun { get; set; }
        /// <summary>
        /// Tokens that represent a new line in with the current model.
        /// </summary>
        [JsonPropertyName("llama_token_newline")]
        public llama_token[] LLamaNewlineTokens { get; set; }
    }
}
