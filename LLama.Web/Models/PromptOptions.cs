namespace LLama.Web.Models
{
    public class PromptOptions
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Prompt { get; set; }
        public List<string> AntiPrompt { get; set; }
        public List<string> OutputFilter { get; set; }
    }
}
