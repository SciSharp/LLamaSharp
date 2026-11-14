using LLama.Batched;
using LLama.Common;
using LLama.Native;
using System.Numerics.Tensors;
using System.Text;

namespace LLama.Unittest
{
    public class SamplingTests
        : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly LLamaWeights _model;
        private readonly ModelParams _params;

        private readonly LLamaBatch _batch;
        private readonly StreamingTokenDecoder _decoder;

        public void Dispose() => _model.Dispose();

        public SamplingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _params = new ModelParams(Constants.GenerativeModelPath2) {
                ContextSize = 200,
                BatchSize = 200,
                GpuLayerCount = Constants.CIGpuLayerCount,
            };
            _model = LLamaWeights.LoadFromFile(_params);
            _batch = new LLamaBatch();
            _decoder = new(Encoding.UTF8, _model);
        }


        [Fact]
        public void Sampling()
        {
            using var context = new LLamaContext(_model, _params);
            var tokens = _model.NativeHandle.Tokenize("I will repeat this phrase forever.\n", false, false, Encoding.UTF8);
            var logitBias = tokens.Select(x => new LLamaLogitBias() { Token = x, Bias = -1000 }).ToArray();

            // Add "I will repeat this phrase forever.\nI will", without requesting any logits.
            for (int i = 0; i < tokens.Length; i++) { _batch.Add(token: tokens[i], pos: i, sequence: LLamaSeqId.Zero, logits: false); }
            for (int i = 0; i < 2; i++) { _batch.Add(token: tokens[i], pos: tokens.Length + i, sequence: LLamaSeqId.Zero, logits: false); }

            // Add " repeat" and test whether next tokens will be "this phrase forever.".
            for (int i = 0; i < 4; i++)
            {
                _batch.Add(token: tokens[i + 2], pos: tokens.Length + i + 2, sequence: LLamaSeqId.Zero, logits: true);
                DecodeAndClear(context);

                var expected = tokens[i + 3];
                var logits = context.NativeHandle.GetLogits(numTokens: 1);

                // Test raw sampling
                Assert.Equal(expected, TensorPrimitives.IndexOfMax(logits));

                // Test native sampling with `LLamaTokenDataArrayNative`.
                var array = LLamaTokenDataArray.Create(logits);
                {
                    using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                    var rawLogits = new float[_model.Vocab.Count];
                    for (int j = 0; j < cur_p.Data.Length; j++)
                    {
                        rawLogits[(int) cur_p.Data[j].ID] = cur_p.Data[j].Logit;
                    }
                    Assert.Equal(expected, TensorPrimitives.IndexOfMax(rawLogits));
                }

                // Test sampling chain
                {
                    using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                    using var chain = CreateChain(context.NativeHandle);
                    chain.Apply(ref cur_p);
                    Assert.Equal(expected, cur_p.Data[(int) cur_p.Selected].ID);
                }

                // Test logit bias
                {
                    using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                    using var chain = CreateChain(context.NativeHandle, logitBias);
                    chain.Apply(ref cur_p);
                    Assert.NotEqual(expected, cur_p.Data[(int) cur_p.Selected].ID);
                }
            }
        }


        [Fact]
        public void BatchedSampling()
        {
            const int batch_count = 4;
            using var context = new LLamaContext(_model, _params with { SeqMax = batch_count });
            var tokens = _model.NativeHandle.Tokenize("I will repeat this phrase forever.\n", false, false, Encoding.UTF8);
            var logitBias = tokens.Select(x => new LLamaLogitBias() { Token = x, Bias = -1000 }).ToArray();

            // Add "I will repeat this phrase forever.\nI will", without requesting any logits.
            for (int i = 0; i < tokens.Length + 2; i++)
            {
                for (int b = 0; b < batch_count; b++)
                {
                    _batch.Add(token: tokens[i % tokens.Length], pos: i, sequence: (LLamaSeqId) b, logits: false);
                }
            }

            // Add " repeat" and test whether next tokens will be "this phrase forever."
            for (int i = 0; i < 4; i++)
            {
                for (int b = 0; b < batch_count; b++)
                {
                    _batch.Add(token: tokens[i + 2], pos: tokens.Length + i + 2, sequence: (LLamaSeqId) b, logits: true);
                }
                DecodeAndClear(context);

                var expected = tokens[i + 3];
                var all_logits = context.NativeHandle.GetLogits(numTokens: batch_count);

                for (int b = 0; b < batch_count; b++)
                {
                    var logits = all_logits.Slice(b * _model.Vocab.Count, _model.Vocab.Count);

                    // Test raw sampling
                    Assert.Equal(expected, TensorPrimitives.IndexOfMax(logits));

                    // Test native sampling with `LLamaTokenDataArrayNative`.
                    var array = LLamaTokenDataArray.Create(logits);
                    {
                        using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                        var rawLogits = new float[_model.Vocab.Count];
                        for (int j = 0; j < cur_p.Data.Length; j++)
                        {
                            rawLogits[(int) cur_p.Data[j].ID] = cur_p.Data[j].Logit;
                        }
                        Assert.Equal(expected, TensorPrimitives.IndexOfMax(rawLogits));
                    }

                    // Test sampling chain
                    {
                        using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                        using var chain = CreateChain(context.NativeHandle);
                        chain.Apply(ref cur_p);
                        Assert.Equal(expected, cur_p.Data[(int) cur_p.Selected].ID);
                    }

                    // Test logit bias
                    {
                        using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                        using var chain = CreateChain(context.NativeHandle, logitBias);
                        chain.Apply(ref cur_p);
                        Assert.NotEqual(expected, cur_p.Data[(int) cur_p.Selected].ID);
                    }
                }
            }
        }


        /// <summary>
        /// Reject excess conversations before submitting invalid sequence IDs and reuse released IDs.
        /// </summary>
        [Fact]
        public async Task BatchedExecutorRespectsSequenceCapacity()
        {
            using var executor = new BatchedExecutor(_model, _params with { SeqMax = 2 });
            using var first = executor.Create();
            using var second = executor.Create();
            Assert.Throws<InvalidOperationException>(() => executor.Create());
            var releasedId = first.ConversationId;
            first.Dispose();
            using var replacement = executor.Create();
            Assert.Equal(releasedId, replacement.ConversationId);
            replacement.Prompt(executor.Context.Tokenize("Hello"));
            second.Prompt(executor.Context.Tokenize("Goodbye"));
            Assert.Equal(DecodeResult.Ok, await executor.Infer(TestContext.Current.CancellationToken));
        }

        /// <summary>
        /// A missing state file must not retain the executor's only sequence slot.
        /// </summary>
        [Fact]
        public void FailedFileLoadReleasesSequenceCapacity()
        {
            using var executor = new BatchedExecutor(_model, _params with { SeqMax = 1 });
            var missingFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.state");
            Assert.Throws<FileNotFoundException>(() => executor.Load(missingFile));
            using var replacement = executor.Create();
            Assert.Equal(LLamaSeqId.Zero, replacement.ConversationId);
        }

        /// <summary>
        /// A rejected in-memory state must not retain the executor's only sequence slot.
        /// </summary>
        [Fact]
        public void FailedMemoryLoadReleasesSequenceCapacity()
        {
            using var executor = new BatchedExecutor(_model, _params with { SeqMax = 1 });
            Assert.ThrowsAny<Exception>(() => executor.Load((Conversation.State)null!));
            using var replacement = executor.Create();
            Assert.Equal(LLamaSeqId.Zero, replacement.ConversationId);
        }

        /// <summary>
        /// Successful loads keep their sequence slot until the returned conversation is disposed.
        /// </summary>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SuccessfulLoadRetainsSequenceCapacity(bool fromFile)
        {
            using var executor = new BatchedExecutor(_model, _params with { SeqMax = 1 });
            var filename = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.state");
            Conversation.State? state = null;
            try
            {
                using (var source = executor.Create())
                {
                    if (fromFile)
                    {
                        source.Save(filename);
                    }
                    else
                    {
                        state = source.Save();
                    }
                }

                using var loaded = fromFile ? executor.Load(filename) : executor.Load(state!);
                Assert.False(loaded.IsDisposed);
                Assert.Throws<InvalidOperationException>(() => executor.Create());
                loaded.Dispose();
                using var replacement = executor.Create();
                Assert.Equal(LLamaSeqId.Zero, replacement.ConversationId);
            }
            finally
            {
                state?.Dispose();
                File.Delete(filename);
            }
        }

        /// <summary>
        /// Preserve full-context DRY history while keeping zero history disabled.
        /// </summary>
        [Theory]
        [InlineData(-1, -0.8f)]
        [InlineData(32, -0.8f)]
        [InlineData(0, 0f)]
        public void DryHistoryLengthAppliesExpectedPenalty(int historyLength, float expected)
        {
            using var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());
            chain.AddDry(_model.NativeHandle, [], penaltyLastN: historyLength);
            foreach (LLamaToken token in new[] { 1000, 1001, 1002, 1000, 1001 })
            {
                chain.Accept(token);
            }

            var candidates = LLamaTokenDataArray.Create(new float[_model.Vocab.Count]);
            using var pin = LLamaTokenDataArrayNative.Create(candidates, out var native);
            chain.Apply(ref native);
            Assert.Equal(expected, native.Data[1002].Logit);
            Assert.Equal(0f, native.Data[1003].Logit);
        }

        private void DecodeAndClear(LLamaContext context)
        {
            Assert.Equal(DecodeResult.Ok, context.Decode(_batch));
            _batch.Clear();
        }

        private static SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context, LLamaLogitBias[]? logit_bias = null)
        {
            var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain.AddPenalties(
                vocabSize: context.Vocab.Count, penaltyCount: 60, repeat: 1, freq: 0, presence: 0
            );

            if (logit_bias != null) { chain.AddLogitBias(context.Vocab.Count, logit_bias); }

            chain.AddTopK(10);
            chain.AddTemperature(0.1f);
            chain.AddDistributionSampler(seed: 42);

            return chain;
        }
    }
}
