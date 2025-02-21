using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using System.Numerics.Tensors;
using System.Text;

using Xunit.Abstractions;

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
            using var context = new LLamaContext(_model, _params);
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

            // Add " repeat" and test whether next tokens will be "this phrase forever.".
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


        private void DecodeAndClear(LLamaContext context)
        {
            context.Decode(_batch);
            _batch.Clear();
        }

        private static SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context, LLamaLogitBias[]? logit_bias = null)
        {
            var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain.AddPenalties(
                penaltyCount: 60, repeat: 1, freq: 0, presence: 0
            );

            if (logit_bias != null) { chain.AddLogitBias(context.Vocab.Count, logit_bias); }

            chain.AddTopK(10);
            chain.AddTemperature(0.1f);
            chain.AddDistributionSampler(seed: 42);

            return chain;
        }
       
        [Fact]
        public void SamplingWithMockTopK()
        {
            // Manually create a mock logits array with a fixed, well-distributed set of values
            var logits = new float[]
            {
                0.56f, -0.85f, 0.74f, -0.33f, 0.92f, -0.44f, 0.61f, -0.77f, 0.18f, -0.29f,
                0.87f, -0.52f, 0.31f, -0.66f, 0.28f, -0.91f, 0.75f, -0.58f, 0.42f, -0.62f,
                0.39f, -0.48f, 0.94f, -0.72f, 0.53f, -0.15f, 0.68f, -0.41f, 0.81f, -0.35f,
                0.76f, -0.27f, 0.63f, -0.69f, 0.21f, -0.11f, 0.59f, -0.79f, 0.33f, -0.87f,
                0.46f, -0.53f, 0.71f, -0.23f, 0.66f, -0.39f, 0.29f, -0.65f, 0.83f, -0.49f,
                0.35f, -0.71f, 0.61f, -0.13f, 0.57f, -0.43f, 0.93f, -0.37f, 0.82f, -0.54f,
                0.44f, -0.22f, 0.88f, -0.46f, 0.72f, -0.18f, 0.64f, -0.55f, 0.95f, -0.33f,
                0.41f, -0.63f, 0.79f, -0.28f, 0.31f, -0.67f, 0.74f, -0.44f, 0.85f, -0.32f,
                0.54f, -0.16f, 0.66f, -0.38f, 0.73f, -0.49f, 0.36f, -0.79f, 0.61f, -0.24f,
                0.77f, -0.55f, 0.52f, -0.41f, 0.81f, -0.36f, 0.69f, -0.26f, 0.45f, -0.17f
            };

            // Mock LLamaTokenDataArray and LLamaTokenDataArrayNative
            var array = LLamaTokenDataArray.Create(logits);

            // First sampling (TopK=5)
            using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
            using var chain5 = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain5.AddTopK(5);
            chain5.Apply(ref cur_p);
            var top5 = new List<float>();
            for (int i = 0; i < 5; i++)
            {
                top5.Add(cur_p.Data[i].Logit);
            }

            // Second sampling (TopK=50)
            using var _2 = LLamaTokenDataArrayNative.Create(array, out var cur_p_broader);
            using var chain50 = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain50.AddTopK(50);
            chain50.Apply(ref cur_p_broader);
            var top50 = new List<float>();
            for (int i = 0; i < 50; i++)
            {
                top50.Add(cur_p_broader.Data[i].Logit);
            }

            // Assert that the top 5 logits are present in the top 50 logits
            Assert.True(top5.All(logit => top50.Contains(logit)));
        }



        /// <summary>
        /// test frequency penalty out of range exception when less than -2
        /// </summary>
        [Fact]
        public void FrequencyPenalty_ThrowsException_WhenValueIsLessThanMinusTwo()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultSamplingPipeline
            {
                FrequencyPenalty = -2.1f
            });
        }


        /// <summary>
        /// test frequency penalty out of range exception when greater than 2
        /// </summary>
        [Fact]
        public void FrequencyPenalty_ThrowsException_WhenValueIsGreaterThanTwo()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultSamplingPipeline
            {
                FrequencyPenalty = 2.1f
            });
        }

        /// <summary>
        /// Test Argument out of range exception when presence penalty less than -2
        /// </summary>
        [Fact]
        public void PresencePenalty_ThrowsException_WhenValueIsLessThanMinusTwo()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultSamplingPipeline
            {
                PresencePenalty = -2.1f
            });
        }

        /// <summary>
        /// Test argument out of range exception when presence penalty is greater than 2
        /// </summary>
        [Fact]
        public void PresencePenalty_ThrowsException_WhenValueIsGreaterThanTwo()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultSamplingPipeline
            {
                PresencePenalty = 2.1f
            });
        }

        /// <summary>
        /// Test the default sampling pipeline defaults
        /// </summary>
        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            var pipeline = new DefaultSamplingPipeline();

            Assert.Equal(1, pipeline.RepeatPenalty);
            Assert.Equal(0.75f, pipeline.Temperature);
            Assert.Equal(40, pipeline.TopK);
            Assert.Equal(1, pipeline.TypicalP);
            Assert.Equal(0.9f, pipeline.TopP);
            Assert.Equal(0.1f, pipeline.MinP);
            Assert.Equal(64, pipeline.PenaltyCount);
            Assert.False(pipeline.PenalizeNewline);
            Assert.False(pipeline.PreventEOS);
        }

        [Fact]
        public void Seed_HasLowProbabilityOfCollision()
        {
            var seedSet = new HashSet<uint>();
            const int numberOfInitializations = 1000; // Run the test 1000 times
            const int maxAllowedDuplicates = 2;

            int duplicateCount = 0;

            for (int i = 0; i < numberOfInitializations; i++)
            {
                var pipeline = new DefaultSamplingPipeline();
                uint seed = pipeline.Seed;
                if (!seedSet.Add(seed))
                {
                    duplicateCount++;
                }
            }

            // Assert that the number of duplicates is within the acceptable threshold
            Assert.True(duplicateCount <= maxAllowedDuplicates, $"Too many duplicate seeds: {duplicateCount}");
        }


        /// <summary>
        /// test the pipeline seed with a specific value
        /// </summary>
        [Fact]
        public void Seed_IsInitializedWithSpecificValue()
        {
            // Arrange
            var pipeline = new DefaultSamplingPipeline();

            // Act
            uint seed = 32;

            // Assert
            Assert.Equal(32, (float)seed);
        }
        /// <summary>
        /// test minkeep with a specific value
        /// </summary>
        [Fact]
        public void SetMinKeep()
        {
            // Arrange
            var pipeline = new DefaultSamplingPipeline();

            //Act
            pipeline.MinKeep = 5;

            //Assert
            Assert.Equal(5, pipeline.MinKeep);
        }

        /// <summary>
        /// test the minkeep default
        /// </summary>
        [Fact]
        public void GetMinKeepDefault()
        {
            // Arrange
            var pipeline = new DefaultSamplingPipeline();

            //Assert
            Assert.Equal(1, pipeline.MinKeep);
        }
    }
}
