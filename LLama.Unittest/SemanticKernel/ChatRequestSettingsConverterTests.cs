using LLamaSharp.SemanticKernel;
using System.Text.Json;

namespace LLama.Unittest.SemanticKernel
{
    public class ChatRequestSettingsConverterTests
    {
        [Fact]
        public void ChatRequestSettingsConverter_DeserializeWithDefaults()
        {
            // Arrange
            var options = new JsonSerializerOptions();
            options.Converters.Add(new LLamaSharpPromptExecutionSettingsConverter());
            var json = "{}";

            // Act
            var requestSettings = JsonSerializer.Deserialize<LLamaSharpPromptExecutionSettings>(json, options);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(0, requestSettings.FrequencyPenalty);
            Assert.Null(requestSettings.MaxTokens);
            Assert.Equal(0, requestSettings.PresencePenalty);
            Assert.Equal(1, requestSettings.ResultsPerPrompt);
            Assert.NotNull(requestSettings.StopSequences);
            Assert.Empty(requestSettings.StopSequences);
            Assert.Equal(0, requestSettings.Temperature);
            Assert.NotNull(requestSettings.TokenSelectionBiases);
            Assert.Empty(requestSettings.TokenSelectionBiases);
            Assert.Equal(0, requestSettings.TopP);
        }

        [Fact]
        public void ChatRequestSettingsConverter_DeserializeWithSnakeCase()
        {
            // Arrange
            var options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;
            options.Converters.Add(new LLamaSharpPromptExecutionSettingsConverter());
            var json = @"{
    ""frequency_penalty"": 0.5,
    ""max_tokens"": 250,
    ""presence_penalty"": 0.5,
    ""results_per_prompt"": -1,
    ""stop_sequences"": [ ""foo"", ""bar"" ],
    ""temperature"": 0.5,
    ""token_selection_biases"": { ""1"": 2, ""3"": 4 },
    ""top_p"": 0.5,
}";

            // Act
            var requestSettings = JsonSerializer.Deserialize<LLamaSharpPromptExecutionSettings>(json, options);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(0.5, requestSettings.FrequencyPenalty);
            Assert.Equal(250, requestSettings.MaxTokens);
            Assert.Equal(0.5, requestSettings.PresencePenalty);
            Assert.Equal(-1, requestSettings.ResultsPerPrompt);
            Assert.NotNull(requestSettings.StopSequences);
            Assert.Contains("foo", requestSettings.StopSequences);
            Assert.Contains("bar", requestSettings.StopSequences);
            Assert.Equal(0.5, requestSettings.Temperature);
            Assert.NotNull(requestSettings.TokenSelectionBiases);
            Assert.Equal(2, requestSettings.TokenSelectionBiases[1]);
            Assert.Equal(4, requestSettings.TokenSelectionBiases[3]);
            Assert.Equal(0.5, requestSettings.TopP);
        }

        [Fact]
        public void ChatRequestSettingsConverter_DeserializeWithPascalCase()
        {
            // Arrange
            var options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;
            options.Converters.Add(new LLamaSharpPromptExecutionSettingsConverter());
            var json = @"{
    ""FrequencyPenalty"": 0.5,
    ""MaxTokens"": 250,
    ""PresencePenalty"": 0.5,
    ""ResultsPerPrompt"": -1,
    ""StopSequences"": [ ""foo"", ""bar"" ],
    ""Temperature"": 0.5,
    ""TokenSelectionBiases"": { ""1"": 2, ""3"": 4 },
    ""TopP"": 0.5,
}";

            // Act
            var requestSettings = JsonSerializer.Deserialize<LLamaSharpPromptExecutionSettings>(json, options);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(0.5, requestSettings.FrequencyPenalty);
            Assert.Equal(250, requestSettings.MaxTokens);
            Assert.Equal(0.5, requestSettings.PresencePenalty);
            Assert.Equal(-1, requestSettings.ResultsPerPrompt);
            Assert.NotNull(requestSettings.StopSequences);
            Assert.Contains("foo", requestSettings.StopSequences);
            Assert.Contains("bar", requestSettings.StopSequences);
            Assert.Equal(0.5, requestSettings.Temperature);
            Assert.NotNull(requestSettings.TokenSelectionBiases);
            Assert.Equal(2, requestSettings.TokenSelectionBiases[1]);
            Assert.Equal(4, requestSettings.TokenSelectionBiases[3]);
            Assert.Equal(0.5, requestSettings.TopP);
        }
    }
}
