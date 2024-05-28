using Xunit;
using Moq;
using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static LLama.LLamaTransforms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace LLamaSharp.SemanticKernel.TextCompletion.Tests
{
    public class LLamaSharpTextCompletionTests : IDisposable
    {
        private MockRepository mockRepository;
        private Mock<ILLamaExecutor> mockExecutor;

        public LLamaSharpTextCompletionTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockExecutor = this.mockRepository.Create<ILLamaExecutor>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private LLamaSharpTextCompletion CreateLLamaSharpTextCompletion()
        {
            return new LLamaSharpTextCompletion(
                this.mockExecutor.Object);
        }

        [Fact]
        public async Task GetTextContentsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateLLamaSharpTextCompletion();
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
            var unitUnderTest = this.CreateLLamaSharpTextCompletion();
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
