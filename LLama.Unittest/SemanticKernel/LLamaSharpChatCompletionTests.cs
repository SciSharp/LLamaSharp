using LLama.Abstractions;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using Xunit;

namespace LLama.Unittest.SemanticKernel
{
    public class LLamaSharpChatCompletionTests
    {
        private Mock<ILLamaExecutor> mockStatelessExecutor;

        public LLamaSharpChatCompletionTests()
        {
            this.mockStatelessExecutor = new Mock<ILLamaExecutor>();
        }

        private LLamaSharpChatCompletion CreateLLamaSharpChatCompletion()
        {
            return new LLamaSharpChatCompletion(
                this.mockStatelessExecutor.Object,
                null,
                null,
                null);
        }

        [Fact]
        public void CreateNewChat_NoInstructions_ReturnsEmptyChatHistory()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpChatCompletion();

            // Act
            var result = unitUnderTest.CreateNewChat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // No system message should be added
        }

        [Fact]
        public void CreateNewChat_WithInstructions_AddsSystemMessage()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpChatCompletion();
            string instructions = "This is a system instruction";

            // Act
            var result = unitUnderTest.CreateNewChat(instructions);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // One message should be present
            Assert.Equal(instructions, result[0].Content); // System message should match the instructions
        }

        [Fact]
        public void CreateNewChat_NullInstructions_ReturnsEmptyChatHistory()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpChatCompletion();

            // Act
            var result = unitUnderTest.CreateNewChat(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Should not add a system message
        }

        [Fact]
        public async Task GetChatMessageContentsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpChatCompletion();
            ChatHistory chatHistory = new ChatHistory();
            
            // Add to Chat History
            chatHistory.AddMessage(new AuthorRole("User"), "Hello");
            chatHistory.AddMessage(new AuthorRole("User"), "World");
            chatHistory.AddMessage(new AuthorRole("User"), "Goodbye");
            chatHistory.AddMessage(new AuthorRole("InvalidRole"), "This should trigger Unknown role");
            
            PromptExecutionSettings? executionSettings = null;
            Kernel? kernel = null;
            CancellationToken cancellationToken = default;
            mockStatelessExecutor.Setup(e => e.InferAsync(It.IsAny<string>(), It.IsAny<IInferenceParams>(), It.IsAny<CancellationToken>()))
                .Returns(new List<string> { "test" }.ToAsyncEnumerable());

            // Act
            var result = await unitUnderTest.GetChatMessageContentsAsync(
                chatHistory,
                executionSettings,
                kernel,
                cancellationToken);
            
            // Assert
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetStreamingChatMessageContentsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpChatCompletion();
            ChatHistory chatHistory = new ChatHistory();
            PromptExecutionSettings? executionSettings = null;
            Kernel? kernel = null;
            CancellationToken cancellationToken = default;

            mockStatelessExecutor.Setup(e => e.InferAsync(It.IsAny<string>(), It.IsAny<IInferenceParams>(), It.IsAny<CancellationToken>()))
               .Returns(new List<string> { "test" }.ToAsyncEnumerable());

            // Act
            await foreach (var result in unitUnderTest.GetStreamingChatMessageContentsAsync(
                chatHistory,
                executionSettings,
                kernel,
                cancellationToken))
            {
                // Assert
                Assert.NotNull(result);
            }
        }
    }
}
