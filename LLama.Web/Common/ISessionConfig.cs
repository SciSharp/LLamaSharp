namespace LLama.Web.Common
{
    public interface ISessionConfig
    {
        string AntiPrompt { get; set; }
        List<string> AntiPrompts { get; set; }
        LLamaExecutorType ExecutorType { get; set; }
        string Model { get; set; }
        string OutputFilter { get; set; }
        List<string> OutputFilters { get; set; }
        string Prompt { get; set; }
    }
}