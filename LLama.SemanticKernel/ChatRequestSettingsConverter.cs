using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLamaSharp.SemanticKernel;

/// <summary>
/// JSON converter for <see cref="OpenAIRequestSettings"/>
/// </summary>
public class LLamaSharpPromptExecutionSettingsConverter : JsonConverter<LLamaSharpPromptExecutionSettings>
{
    /// <inheritdoc/>
    public override LLamaSharpPromptExecutionSettings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var requestSettings = new LLamaSharpPromptExecutionSettings();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string? propertyName = reader.GetString();

                if (propertyName is not null)
                {
                    // normalise property name to uppercase
                    propertyName = propertyName.ToUpperInvariant();
                }

                reader.Read();

                switch (propertyName)
                {
                    case "MODELID":
                    case "MODEL_ID":
                        requestSettings.ModelId = reader.GetString();
                        break;
                    case "TEMPERATURE":
                        requestSettings.Temperature = reader.GetDouble();
                        break;
                    case "TOPP":
                    case "TOP_P":
                        requestSettings.TopP = reader.GetDouble();
                        break;
                    case "FREQUENCYPENALTY":
                    case "FREQUENCY_PENALTY":
                        requestSettings.FrequencyPenalty = reader.GetDouble();
                        break;
                    case "PRESENCEPENALTY":
                    case "PRESENCE_PENALTY":
                        requestSettings.PresencePenalty = reader.GetDouble();
                        break;
                    case "MAXTOKENS":
                    case "MAX_TOKENS":
                        requestSettings.MaxTokens = reader.GetInt32();
                        break;
                    case "STOPSEQUENCES":
                    case "STOP_SEQUENCES":
                        requestSettings.StopSequences = JsonSerializer.Deserialize<IList<string>>(ref reader, options) ?? Array.Empty<string>();
                        break;
                    case "RESULTSPERPROMPT":
                    case "RESULTS_PER_PROMPT":
                        requestSettings.ResultsPerPrompt = reader.GetInt32();
                        break;
                    case "TOKENSELECTIONBIASES":
                    case "TOKEN_SELECTION_BIASES":
                        requestSettings.TokenSelectionBiases = JsonSerializer.Deserialize<IDictionary<int, int>>(ref reader, options) ?? new Dictionary<int, int>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return requestSettings;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, LLamaSharpPromptExecutionSettings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("temperature", value.Temperature);
        writer.WriteNumber("top_p", value.TopP);
        writer.WriteNumber("frequency_penalty", value.FrequencyPenalty);
        writer.WriteNumber("presence_penalty", value.PresencePenalty);
        if (value.MaxTokens is null)
        {
            writer.WriteNull("max_tokens");
        }
        else
        {
            writer.WriteNumber("max_tokens", (decimal)value.MaxTokens);
        }
        writer.WritePropertyName("stop_sequences");
        JsonSerializer.Serialize(writer, value.StopSequences, options);
        writer.WriteNumber("results_per_prompt", value.ResultsPerPrompt);
        writer.WritePropertyName("token_selection_biases");
        JsonSerializer.Serialize(writer, value.TokenSelectionBiases, options);

        writer.WriteEndObject();
    }
}