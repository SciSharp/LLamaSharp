using Microsoft.SemanticKernel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

[Obsolete("Use LLamaSharpPromptExecutionSettings instead")]
public class ChatRequestSettings : PromptExecutionSettings
{
    /// <summary>
    /// Temperature controls the randomness of the completion.
    /// The higher the temperature, the more random the completion.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    /// <summary>
    /// TopP controls the diversity of the completion.
    /// The higher the TopP, the more diverse the completion.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; }

    /// <summary>
    /// Number between -2.0 and 2.0. Positive values penalize new tokens
    /// based on whether they appear in the text so far, increasing the
    /// model's likelihood to talk about new topics.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; }

    /// <summary>
    /// Number between -2.0 and 2.0. Positive values penalize new tokens
    /// based on their existing frequency in the text so far, decreasing
    /// the model's likelihood to repeat the same line verbatim.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// Sequences where the completion will stop generating further tokens.
    /// </summary>
    [JsonPropertyName("stop_sequences")]
    public IList<string> StopSequences { get; set; } = Array.Empty<string>();

    /// <summary>
    /// How many completions to generate for each prompt. Default is 1.
    /// Note: Because this parameter generates many completions, it can quickly consume your token quota.
    /// Use carefully and ensure that you have reasonable settings for max_tokens and stop.
    /// </summary>
    [JsonPropertyName("results_per_prompt")]
    public int ResultsPerPrompt { get; set; } = 1;

    /// <summary>
    /// The maximum number of tokens to generate in the completion.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Modify the likelihood of specified tokens appearing in the completion.
    /// </summary>
    [JsonPropertyName("token_selection_biases")]
    public IDictionary<int, int> TokenSelectionBiases { get; set; } = new Dictionary<int, int>();

    /// <summary>
    /// Create a new settings object with the values from another settings object.
    /// </summary>
    /// <param name="requestSettings">Template configuration</param>
    /// <param name="defaultMaxTokens">Default max tokens</param>
    /// <returns>An instance of OpenAIRequestSettings</returns>
    public static ChatRequestSettings FromRequestSettings(PromptExecutionSettings? requestSettings, int? defaultMaxTokens = null)
    {
        requestSettings ??= new ChatRequestSettings
        {
            MaxTokens = defaultMaxTokens
        };

        if (requestSettings is ChatRequestSettings requestSettingsChatRequestSettings)
        {
            return requestSettingsChatRequestSettings;
        }

        var json = JsonSerializer.Serialize(requestSettings);
        var chatRequestSettings = JsonSerializer.Deserialize<ChatRequestSettings>(json, SerializerOptions);

        if (chatRequestSettings is not null)
        {
            return chatRequestSettings;
        }

        throw new ArgumentException($"Invalid request settings, cannot convert to {nameof(ChatRequestSettings)}", nameof(requestSettings));
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        MaxDepth = 20,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new ChatRequestSettingsConverter() }
    };
}
