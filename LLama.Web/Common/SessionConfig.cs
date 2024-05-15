namespace LLama.Web.Common;

public class SessionConfig : ISessionConfig
{
    public string Model { get; set; }
    public string Prompt { get; set; }

    public string AntiPrompt { get; set; }
    public List<string> AntiPrompts { get; set; }
    public string OutputFilter { get; set; }
    public List<string> OutputFilters { get; set; }
    public LLamaExecutorType ExecutorType { get; set; }
}
