using LLama.Abstractions;
using LLama.Common;
using LLama.Web.Common;

namespace LLama.Web.Models
{
    public class CreateSessionModel : IInferenceParams
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public LLamaExecutorType ExecutorType { get; set; } = LLamaExecutorType.Interactive;
        public string AntiPrompt { get; set; } = string.Empty;
        public string OutputFilter { get; set; } = string.Empty;

        public int TokensKeep { get; set; } = 0;
        public int MaxTokens { get; set; } = -1;
        public IEnumerable<string> AntiPrompts { get; set; } = Array.Empty<string>();
        public string InputSuffix { get; set; } = string.Empty;
        public string InputPrefix { get; set; } = string.Empty;
        public int TopK { get; set; } = 40;
        public float TopP { get; set; } = 0.95f;
        public float TfsZ { get; set; } = 1.0f;
        public float TypicalP { get; set; } = 1.0f;
        public float Temperature { get; set; } = 0.8f;
        public float RepeatPenalty { get; set; } = 1.1f;
        public int RepeatLastTokensCount { get; set; } = 64;
        public float FrequencyPenalty { get; set; } = .0f;
        public float PresencePenalty { get; set; } = .0f;
        public MirostatType Mirostat { get; set; } = MirostatType.Disable;
        public float MirostatTau { get; set; } = 5.0f;
        public float MirostatEta { get; set; } = 0.1f;
        public bool PenalizeNL { get; set; } = true;


        // TODO: Ensure overpost protected
        public Dictionary<int, float> LogitBias { get; set; }
        public string PathSession { get; set; } = string.Empty;
    }
}
