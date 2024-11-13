using LLama.Common;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory.AI;
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
        [InlineData("...___---")]
        [InlineData("15 + 6 = 21 && 68 * 75 = 5100")]
        [InlineData("  \n  \r\n  \t   ")]
        public void GetTokens_ShouldReturnListOfTokensForInputString(string? text)
        {
            var tokens = _generator!.GetTokens(text);
            var tokensCount = _generator.CountTokens(text);

            var expected = text;
            var actual = string.Join("", tokens);

            _testOutputHelper.WriteLine($"Tokens for '{text}':");
            _testOutputHelper.WriteLine(string.Join("", tokens.Select(x => $"({x})")));

            Assert.Equal(expected, actual);
            Assert.Equal(tokensCount, tokens.Count);
        }

        /* This is exactly the same test as the non-unicode cases. However, there are reasons why this
         * should be made a special case and may deviate in the future:
         * 
         * As of now there appears to be no final word as to how characters that consist of more than one 
         * numeric token should correspond to textual tokens, and results vary according to different 
         * models' tokenizers. For example, given a character 'Z' that corresponds to the numeric tokens {1,2,3} 
         * some (llama-2) will pad the length of the total number of tokens by returning spaces as tokens 
         * (i.e. ' ', ' ', 'Z') while others (GPT4Tokenizer) will pad with the character itself (i.e. 'Z','Z','Z').
         * 
         * This is very evident when tokenizing ideograms and emojis, but can arise with various unicode characters 
         * as well. See pull request for more relevant discussion https://github.com/SciSharp/LLamaSharp/pull/862
         *
         * Currently the method will remain consistent with the output of ITextTokenizer.CountTokens, meaning
         * any redundant tokens will not be omitted as long as they are counted by CountTokens.
         * 
         * StreamingTokenDecoder, while sufficiently useful for this task, was not designed with producing
         * output for one numeric token at a time in mind, so ITextTokenizer.GetTokens should not be considered 
         * an example of proper use.
         * 
         * Note: if this message is removed, also remove references to it in LLamaSharpTextEmbeddingGenerator.GetTokens
         * and LLamaSharpTextGenerator.GetTokens
         */
        [Theory]
        [InlineData("And a little bit of unicode για να κρατήσουμε τα πράγματα ενδιαφέροντα")]
        [InlineData("猫坐在垫子上 😀🤨🤐😏")]
        public void GetTokens_Unicode_ShouldReturnListOfTokensForInputString(string? text)
        {
            var tokens = _generator!.GetTokens(text);
            var tokensCount = _generator.CountTokens(text);

            var expected = text;
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
