using System.Collections.Generic;
using LLama.Common;

namespace LLama.Abstractions
{
    public interface IInferenceParams
    {
        IEnumerable<string> AntiPrompts { get; set; }
        float FrequencyPenalty { get; set; }
        string InputPrefix { get; set; }
        string InputSuffix { get; set; }
        Dictionary<int, float>? LogitBias { get; set; }
        int MaxTokens { get; set; }
        MirostatType Mirostat { get; set; }
        float MirostatEta { get; set; }
        float MirostatTau { get; set; }
        string PathSession { get; set; }
        bool PenalizeNL { get; set; }
        float PresencePenalty { get; set; }
        int RepeatLastTokensCount { get; set; }
        float RepeatPenalty { get; set; }
        float Temperature { get; set; }
        float TfsZ { get; set; }
        int TokensKeep { get; set; }
        int TopK { get; set; }
        float TopP { get; set; }
        float TypicalP { get; set; }
    }
}