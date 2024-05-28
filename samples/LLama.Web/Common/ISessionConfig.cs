using System.ComponentModel;

namespace LLama.Web.Common;

public interface ISessionConfig
{
    string AntiPrompt { get; set; }

    [DisplayName("Anti Prompts")]
    List<string> AntiPrompts { get; set; }

    [DisplayName("Executor Type")]
    LLamaExecutorType ExecutorType { get; set; }

    string Model { get; set; }

    [DisplayName("Output Filter")]
    string OutputFilter { get; set; }

    List<string> OutputFilters { get; set; }

    string Prompt { get; set; }
}