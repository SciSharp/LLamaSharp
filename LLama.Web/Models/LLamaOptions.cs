namespace LLama.Web.Models
{
    public class LLamaOptions
    {
        public List<ModelOptions> Models { get; set; }
        public List<PromptOptions> Prompts { get; set; } = new List<PromptOptions>();
        public List<ParameterOptions> Parameters { get; set; } = new List<ParameterOptions>();

        public void Initialize()
        {
            foreach (var prompt in Prompts)
            {
                if (File.Exists(prompt.Path))
                {
                    prompt.Prompt = File.ReadAllText(prompt.Path).Trim();
                }
            }
        }
    }
}
