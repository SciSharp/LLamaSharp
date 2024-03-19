using Xunit;
using Moq;
using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static LLama.LLamaTransforms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace LLamaSharp.SemanticKernel.ChatCompletion.Tests
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
        public async Task GetChatMessageContentsAsync_StateUnderTest_ExpectedBehavior()
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
