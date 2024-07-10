using LLama.Abstractions;
using LLamaSharp.SemanticKernel.TextCompletion;
using Microsoft.SemanticKernel;
using Moq;

namespace LLama.Unittest.SemanticKernel
{
    public sealed class LLamaSharpTextCompletionTests
        : IDisposable
    {
        private MockRepository mockRepository;
        private Mock<ILLamaExecutor> mockExecutor;

        public LLamaSharpTextCompletionTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            mockExecutor = mockRepository.Create<ILLamaExecutor>();
        }

        public void Dispose()
        {
            mockRepository.VerifyAll();
        }

        private LLamaSharpTextCompletion CreateLLamaSharpTextCompletion()
        {
            return new LLamaSharpTextCompletion(
                mockExecutor.Object);
        }

        [Fact]
        public async Task GetTextContentsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateLLamaSharpTextCompletion();
            string prompt = "Test";
            PromptExecutionSettings? executionSettings = null;
            Kernel? kernel = null;
            CancellationToken cancellationToken = default;
            mockExecutor.Setup(e => e.InferAsync(It.IsAny<string>(), It.IsAny<IInferenceParams>(), It.IsAny<CancellationToken>()))
                .Returns(new List<string> { "test" }.ToAsyncEnumerable());

            // Act
            var result = await unitUnderTest.GetTextContentsAsync(
                prompt,
                executionSettings,
                kernel,
                cancellationToken);

            // Assert
            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetStreamingTextContentsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateLLamaSharpTextCompletion();
            string prompt = "Test";
            PromptExecutionSettings? executionSettings = null;
            Kernel? kernel = null;
            CancellationToken cancellationToken = default;
            mockExecutor.Setup(e => e.InferAsync(It.IsAny<string>(), It.IsAny<IInferenceParams>(), It.IsAny<CancellationToken>()))
                .Returns(new List<string> { "test" }.ToAsyncEnumerable());

            // Act
            await foreach (var result in unitUnderTest.GetStreamingTextContentsAsync(
                prompt,
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
