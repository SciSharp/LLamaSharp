using Xunit;
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
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace LLamaSharp.SemanticKernel.Tests
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
