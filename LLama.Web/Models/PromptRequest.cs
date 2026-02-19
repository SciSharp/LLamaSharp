namespace LLama.Web.Models;

public class PromptRequest
{
    public string Prompt { get; set; }
    public List<string> AttachmentIds { get; set; } = new();
}
