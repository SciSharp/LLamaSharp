using LLama.Common;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LLama.Unittest.KernelMemory
{
    public abstract class ITextTokenizerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

#pragma warning disable KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        protected ITextTokenizer? _generator;
#pragma warning restore KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        protected InferenceParams _infParams;
        protected LLamaSharpConfig _lsConfig;

        public ITextTokenizerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _infParams = new() { AntiPrompts = ["\n\n"] };
            _lsConfig = new(Constants.GenerativeModelPath) { DefaultInferenceParams = _infParams };

            testOutputHelper.WriteLine($"Using model {Path.GetFileName(_lsConfig.ModelPath)}");
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog")]
        [InlineData("Well, here're some special characters!!!")]
        [InlineData("And a little bit of unicode για να κρατήσουμε τα πράγματα ενδιαφέροντα")]
        [InlineData("  \n  \r\n  \t   ")]
        public void GetTokens_ShouldReturnListOfTokensForInputString(string? text)
        {
            var tokens = _generator!.GetTokens(text);
            var tokensCount = _generator.CountTokens(text);

            var expected = " " + text; // the placement of the space corresponding to BOS will vary by model
            var actual = string.Join("", tokens);

            _testOutputHelper.WriteLine($"Tokens for '{text}':");
            _testOutputHelper.WriteLine(string.Join("", tokens.Select(x => $"({x})")));

            Assert.Equal(expected, actual);
            Assert.Equal(tokensCount, tokens.Count);
        }

        [Fact]
        public void GetToken_ShouldThrowForNull()
        {
            string? text = null;

            Assert.Throws<ArgumentNullException>(() => { _generator!.GetTokens(text!); });
        }

        [Fact]
        public void GetToken_EmptyStringYieldsOneEmptyToken()
        {
            var text = "";
            var expected = "";

            var tokens = _generator!.GetTokens(text);
            var tokensCount = _generator.CountTokens(text);
            var actual = tokens.Single();

            _testOutputHelper.WriteLine($"Tokens for '{text}':");
            _testOutputHelper.WriteLine(string.Join("", tokens.Select(x => $"({x})")));

            Assert.Equal(expected, actual);
            Assert.Equal(tokensCount, tokens.Count);
        }
    }
}
