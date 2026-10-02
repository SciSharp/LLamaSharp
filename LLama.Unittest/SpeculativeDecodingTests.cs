using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace LLama.Unittest
{
    /// <summary>
    /// Validates the end-to-end integration of the native speculative decoding engine.
    /// <para>These tests cover Dual-Model speculation, Multi-Token Prediction (MTP) routing, and the specialized queueing mechanics required for BatchedExecutor multiplexing.</para>
    /// </summary>
    public sealed class SpeculativeDecodingTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly LLamaWeights _targetWeights;
        private readonly LLamaWeights _draftWeights;
        private readonly ModelParams _targetParams;
        private readonly ModelParams _draftParams;

        public SpeculativeDecodingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            // Target Model (e.g., Llama 3.2 1B)
            _targetParams = new ModelParams(Constants.GenerativeModelPath)
            {
                ContextSize = 1024,
                BatchSize = 512,
                GpuLayerCount = -1,
                MainGpu = 0,
                UBatchSize = 512
            };
            _targetWeights = LLamaWeights.LoadFromFile(_targetParams);

            // Draft Model (e.g., SmolLM 360M) - MUST be physically distinct for C++ stability
            _draftParams = new ModelParams(Constants.GenerativeModelPath2)
            {
                ContextSize = 1024,
                BatchSize = 512,
                GpuLayerCount = -1,
                MainGpu = 0,
                UBatchSize = 512
            };
            _draftWeights = LLamaWeights.LoadFromFile(_draftParams);
        }

        public void Dispose()
        {
            _targetWeights.Dispose();
            _draftWeights.Dispose();
        }

        /// <summary>
        /// Verifies that standard Dual-Model speculative decoding (Draft-Simple) successfully generates tokens 
        /// through the StatelessExecutor's IAsyncEnumerable streaming pipeline without cache desynchronization.
        /// </summary>
        [Fact]
        public async Task StatelessExecutor_DualModelSpeculation_ProducesOutput()
        {
            var inferenceParams = new InferenceParams { MaxTokens = 10 };

            // Provide strictly separated Target and Draft models
            var executor = new StatelessExecutor(
                weights: _targetWeights,
                @params: _targetParams,
                draftWeights: _draftWeights,
                draftParams: _draftParams,
                draftTokens: 3,
                useMtp: false
            );

            var tokens = await executor.InferAsync("The quick brown fox", inferenceParams).ToListAsync();

            Assert.NotNull(tokens);
            Assert.True(tokens.Count > 0);

            var generatedText = string.Join("", tokens);
            _testOutputHelper.WriteLine($"Generated text: {generatedText}");
            Assert.False(string.IsNullOrWhiteSpace(generatedText));
        }

        /// <summary>
        /// Verifies the BatchedExecutor's custom multiplexing logic. 
        /// <para>Specifically tests that a native speculative burst is correctly captured in the Conversation's internal queue during the Decode phase, and that C# safely dequeues these tokens bypassing the standard native sampler.</para>
        /// </summary>
        [Fact]
        public async Task BatchedExecutor_DualModelQueue_DequeuesCorrectly()
        {
            using var executor = new BatchedExecutor(
                model: _targetWeights,
                contextParams: _targetParams,
                draftModel: _draftWeights,
                draftParams: _draftParams,
                draftTokens: 3,
                useMtp: false
            );

            using var conversation = executor.Create();
            var promptTokens = executor.Context.Tokenize("Count to three: 1, 2,");
            conversation.Prompt(promptTokens);

            var sampler = new DefaultSamplingPipeline();

            // 1. Evaluate prompt (Prefill phase - produces 0 speculative tokens)
            var result = await executor.Infer();
            Assert.Equal(DecodeResult.Ok, result);
            Assert.True(conversation.RequiresSampling);

            // Sample the first real token manually
            var token = conversation.Sample(sampler);
            Assert.NotEqual((LLamaToken)0, token);

            // 2. Single token evaluation (Decode phase - this triggers speculative drafting!)
            conversation.Prompt(token);
            result = await executor.Infer();
            Assert.Equal(DecodeResult.Ok, result);

            // NOW we should have captured the burst!
            Assert.True(conversation.HasSpeculativeTokens, "The conversation queue should have captured the speculative burst.");

            int tokenCount = 0;
            while (conversation.HasSpeculativeTokens)
            {
                token = conversation.Sample(sampler);
                Assert.NotEqual((LLamaToken)0, token);

                conversation.Prompt(token);
                tokenCount++;
            }

            Assert.True(tokenCount > 0, "Failed to dequeue any speculative tokens.");
            _testOutputHelper.WriteLine($"Successfully speculatively decoded {tokenCount} tokens in a single batch pass.");
        }

        /// <summary>
        /// Verifies that Multi-Token Prediction (MTP) self-speculation correctly initializes and routes 
        /// the target model's hidden states through its own projection heads, requiring no external draft weights.
        /// </summary>
        [Fact]
        public async Task StatelessExecutor_Mtp_ProducesOutput()
        {
            var mtpParams = new ModelParams(Constants.MtpModelPath)
            {
                ContextSize = 1024,
                BatchSize = 512,
                LoadMTP = true // This correctly triggers context_type = MTP in the new aligned struct!
            };

            using var mtpWeights = LLamaWeights.LoadFromFile(mtpParams);

            var executor = new StatelessExecutor(
                weights: mtpWeights,
                @params: mtpParams,
                draftTokens: 2,
                useMtp: true // Tell the SpeculativeDecoder to use MTP routing
            );

            var tokens = await executor.InferAsync("The quick brown fox", new InferenceParams { MaxTokens = 10 }).ToListAsync();
            Assert.True(tokens.Count > 0);

            var generatedText = string.Join("", tokens);
            _testOutputHelper.WriteLine($"Generated MTP text: {generatedText}");
            Assert.False(string.IsNullOrWhiteSpace(generatedText));
        }
    }
}

