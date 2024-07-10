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
    }
}
