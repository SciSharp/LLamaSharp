namespace LLama.Web.Common
{
    public class SessionConfig
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public LLamaExecutorType ExecutorType { get; set; } = LLamaExecutorType.Instruct;
        public string AntiPrompt { get; set; } = string.Empty;
        public string OutputFilter { get; set; } = string.Empty;
        public string InputSuffix { get; set; } = string.Empty;
        public string InputPrefix { get; set; } = string.Empty;
    }
}
