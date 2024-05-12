using LLamaSharp.SemanticKernel;
using Microsoft.SemanticKernel;

namespace LLama.Unittest.SemanticKernel
{
    public class ChatRequestSettingsTests
    {
        [Fact]
        public void ChatRequestSettings_FromRequestSettingsNull()
        {
            // Arrange
            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(null, null);

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
        public void ChatRequestSettings_FromRequestSettingsNullWithMaxTokens()
        {
            // Arrange
            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(null, 200);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(0, requestSettings.FrequencyPenalty);
            Assert.Equal(200, requestSettings.MaxTokens);
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
        public void ChatRequestSettings_FromExistingRequestSettings()
        {
            // Arrange
            var originalRequestSettings = new LLamaSharpPromptExecutionSettings()
            {
                FrequencyPenalty = 0.5,
                MaxTokens = 100,
                PresencePenalty = 0.5,
                ResultsPerPrompt = -1,
                StopSequences = new[] { "foo", "bar" },
                Temperature = 0.5,
                TokenSelectionBiases = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } },
                TopP = 0.5,
            };

            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(originalRequestSettings);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(originalRequestSettings, requestSettings);
        }

        [Fact]
        public void ChatRequestSettings_FromAIRequestSettings()
        {
            // Arrange
            var originalRequestSettings = new PromptExecutionSettings()
            {
                ModelId = "test",
            };

            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(originalRequestSettings);

            // Assert
            Assert.NotNull(requestSettings);
            Assert.Equal(originalRequestSettings.ModelId, requestSettings.ModelId);
        }

        [Fact]
        public void ChatRequestSettings_FromAIRequestSettingsWithExtraPropertiesInSnakeCase()
        {
            // Arrange
            var originalRequestSettings = new PromptExecutionSettings()
            {
                ModelId = "test",
                ExtensionData = new Dictionary<string, object>
                {
                    { "frequency_penalty", 0.5 },
                    { "max_tokens", 250 },
                    { "presence_penalty", 0.5 },
                    { "results_per_prompt", -1 },
                    { "stop_sequences", new [] { "foo", "bar" } },
                    { "temperature", 0.5 },
                    { "token_selection_biases", new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } } },
                    { "top_p", 0.5 },
                }
            };

            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(originalRequestSettings);

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
        public void ChatRequestSettings_FromAIRequestSettingsWithExtraPropertiesInPascalCase()
        {
            // Arrange
            var originalRequestSettings = new PromptExecutionSettings()
            {
                ModelId = "test",
                ExtensionData = new Dictionary<string, object>
                {
                    { "FrequencyPenalty", 0.5 },
                    { "MaxTokens", 250 },
                    { "PresencePenalty", 0.5 },
                    { "ResultsPerPrompt", -1 },
                    { "StopSequences", new [] { "foo", "bar" } },
                    { "Temperature", 0.5 },
                    { "TokenSelectionBiases", new Dictionary<int, int>() { { 1, 2 }, { 3, 4 } } },
                    { "TopP", 0.5 },
                }
            };

            // Act
            var requestSettings = LLamaSharpPromptExecutionSettings.FromRequestSettings(originalRequestSettings);

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
