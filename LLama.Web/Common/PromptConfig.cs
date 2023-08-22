namespace LLama.Web.Common
{
    public class PromptConfig
    {
        public string Prompt { get; set; }
        public List<string> AntiPrompt { get; set; }
        public List<string> OutputFilter { get; set; }
    }
}
