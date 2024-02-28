using System.Text.Json.Serialization;
using LLama.Common;

namespace LLava;

/// <summary>
/// TODO: Temporary prompt wrapper to keep interface ILLamaExecutor without changes. 
/// </summary>
public class Prompt
{
    /// <summary>
    /// Text prompt
    /// </summary>
    [JsonPropertyName("TextPrompt")]
    public string TextPrompt { get; set; }

    [JsonPropertyName("Image")]
    public byte[] Image { get; set; }
    
    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="textPrompt">Textual prompt</param>
    /// <param name="content">Message content</param>
    public Prompt(string textPrompt, byte[] image)
    {
        this.TextPrompt = textPrompt;
        this.Image = image;
    }
}