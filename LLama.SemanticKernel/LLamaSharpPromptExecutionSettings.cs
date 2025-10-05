using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaSharp.SemanticKernel;

public class LLamaSharpPromptExecutionSettings : PromptExecutionSettings
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
    /// Indicates the format of the response which can be used downstream to post-process the messages. Handlebars: handlebars_object. JSON: json_object, etc.
    /// </summary>
    [JsonPropertyName("response_format")]
    public string ResponseFormat { get; set; } = string.Empty;

    /// <summary>
    /// Create a new settings object with the values from another settings object.
    /// </summary>
    /// <param name="requestSettings">Template configuration</param>
    /// <param name="defaultMaxTokens">Default max tokens</param>
    /// <returns>An instance of OpenAIRequestSettings</returns>
    public static LLamaSharpPromptExecutionSettings FromRequestSettings(PromptExecutionSettings? requestSettings, int? defaultMaxTokens = null)
    {
        requestSettings ??= new LLamaSharpPromptExecutionSettings
        {
            MaxTokens = defaultMaxTokens
        };

        if (requestSettings is LLamaSharpPromptExecutionSettings requestSettingsChatRequestSettings)
        {
            return requestSettingsChatRequestSettings;
        }

        var json = JsonSerializer.Serialize(requestSettings);
        var chatRequestSettings = JsonSerializer.Deserialize<LLamaSharpPromptExecutionSettings>(json, SerializerOptions);

        if (chatRequestSettings is not null)
        {
            return chatRequestSettings;
        }

        throw new ArgumentException($"Invalid request settings, cannot convert to {nameof(LLamaSharpPromptExecutionSettings)}", nameof(requestSettings));
    }

    internal static LLamaSharpPromptExecutionSettings FromRequestSettings(ChatOptions? options, int? defaultMaxTokens = null)
    {
        if (options == null)
        {
            return new LLamaSharpPromptExecutionSettings
            {
                MaxTokens = defaultMaxTokens
            };
        }

        // Handle nullable float? to double conversion and nullability
        double GetDoubleOrDefault(float? value, double defaultValue = 0.0) => value.HasValue ? (double)value.Value : defaultValue;

        // Handle StopSequences: ensure always IList<string>
        IList<string> stopSequences = options.StopSequences != null
            ? new List<string>(options.StopSequences)
            : new List<string>();

        // ResultsPerPrompt, MaxTokens, TokenSelectionBiases, ResponseFormat: check for property existence
        // Since these properties do not exist on ChatOptions, use defaults
        int resultsPerPrompt = 1;
        int? maxTokens = defaultMaxTokens;
        IDictionary<int, int> tokenSelectionBiases = new Dictionary<int, int>();
        string responseFormat = options.ResponseFormat?.ToString() ?? string.Empty;

        var settings = new LLamaSharpPromptExecutionSettings
        {
            Temperature = GetDoubleOrDefault(options.Temperature),
            TopP = GetDoubleOrDefault(options.TopP),
            PresencePenalty = GetDoubleOrDefault(options.PresencePenalty),
            FrequencyPenalty = GetDoubleOrDefault(options.FrequencyPenalty),
            StopSequences = stopSequences,
            ResultsPerPrompt = resultsPerPrompt,
            MaxTokens = maxTokens ?? options.MaxOutputTokens,
            TokenSelectionBiases = tokenSelectionBiases,
            ResponseFormat = responseFormat
        };

        return settings;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        MaxDepth = 20,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new LLamaSharpPromptExecutionSettingsConverter() }
    };
}
