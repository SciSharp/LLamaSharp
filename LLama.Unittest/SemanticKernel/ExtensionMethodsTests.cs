using LLamaSharp.SemanticKernel;

namespace LLama.Unittest.SemanticKernel
{
    public class ExtensionMethodsTests
    {
        [Fact]
        public void ToLLamaSharpChatHistory_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var chatHistory = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory();
            bool ignoreCase = true;

            // Act
            var result = ExtensionMethods.ToLLamaSharpChatHistory(
                chatHistory,
                ignoreCase);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ToLLamaSharpChatHistory_NullChatHistory_ThrowsArgumentNullException()
        {
            // Arrange
            Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory = null;
            bool ignoreCase = true;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ExtensionMethods.ToLLamaSharpChatHistory(chatHistory, ignoreCase));

            Assert.Equal("chatHistory", exception.ParamName);
        }

        [Fact]
        public void ToLLamaSharpInferenceParams_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var requestSettings = new LLamaSharpPromptExecutionSettings();

            // Act
            var result = ExtensionMethods.ToLLamaSharpInferenceParams(
                requestSettings);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ToLLamaSharpInferenceParams_NullRequestSettings_ThrowsArgumentNullException()
        {
            // Arrange
            LLamaSharpPromptExecutionSettings requestSettings = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ExtensionMethods.ToLLamaSharpInferenceParams(requestSettings));

            // Ensure the exception is thrown for the correct parameter
            Assert.Equal("requestSettings", exception.ParamName);
        }
        
    }
}
