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
        /// <summary>
        /// Test changing temperature
        /// </summary>
        [Fact]
        public void SamplingWithTemperature()
        {
            using var context = new LLamaContext(_model, _params);
            var tokens = _model.NativeHandle.Tokenize("The quick brown fox", false, false, Encoding.UTF8);

            _batch.Add(token: tokens[0], pos: 0, sequence: LLamaSeqId.Zero, logits: true);
            DecodeAndClear(context);

            var logits = context.NativeHandle.GetLogits(numTokens: 1);

            // Apply low temperature
            var arrayLow = LLamaTokenDataArray.Create(logits);
            using (var nativeArrayLow = LLamaTokenDataArrayNative.Create(arrayLow, out var cur_p_low))
            {
                using (var chainLow = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default()))
                {
                    chainLow.AddTemperature(0.1f);
                    chainLow.Apply(ref cur_p_low);
                    float lowTempSample = cur_p_low.Data[0].Logit;
                    // Apply high temperature
                    var arrayHigh = LLamaTokenDataArray.Create(logits); // Create a fresh array for high temperature
                    using (var nativeArrayHigh = LLamaTokenDataArrayNative.Create(arrayHigh, out var cur_p_high))
                    {
                        using (var chainHigh = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default()))
                        {
                            chainHigh.AddTemperature(1.5f);
                            chainHigh.Apply(ref cur_p_high);
                            float highTempSample = cur_p_high.Data[0].Logit;
                            Assert.NotEqual(lowTempSample, highTempSample);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Test that TopK works
        /// </summary>
        [Fact]
        public void SamplingWithTopK()
        {
            using var context = new LLamaContext(_model, _params);
            var tokens = _model.NativeHandle.Tokenize("The quick brown fox", false, false, Encoding.UTF8);

            _batch.Add(token: tokens[0], pos: 0, sequence: LLamaSeqId.Zero, logits: true);
            DecodeAndClear(context);

            var logits = context.NativeHandle.GetLogits(numTokens: 1);
            var array = LLamaTokenDataArray.Create(logits);

            // First sampling (TopK=5)
            using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
            using var chain5 = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain5.AddTopK(5);
            chain5.Apply(ref cur_p);
            int sampledToken5 = (int)cur_p.Data[(int)cur_p.Selected].ID;
           
            // Second sampling (TopK=50)
            using var _2 = LLamaTokenDataArrayNative.Create(array, out var cur_p_broader);
            using var chain50 = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

            chain50.AddTopK(50);
            chain50.Apply(ref cur_p_broader);
            int sampledToken50 = (int)cur_p_broader.Data[(int)cur_p_broader.Selected].ID;

            // Convert cur_p_broader to a List of tokens (IDs)
            List<LLama.Native.LLamaToken> tokenList = new List<LLama.Native.LLamaToken>();
            for (int i = 0; i < (int)cur_p_broader.Size; i++)
            {
                tokenList.Add(cur_p_broader.Data[i].ID);
            }

            // Use StreamingTokenDecoder to decode the token list
            var decoder = new StreamingTokenDecoder(context);
            decoder.AddRange(tokenList);
            var text50 = decoder.Read();  // Convert tokens to text


            // Convert cur_p to a List of tokens (IDs)
            tokenList = new List<LLama.Native.LLamaToken>();
            for (int i = 0; i < (int)cur_p.Size; i++)
            {
                tokenList.Add(cur_p.Data[i].ID);
            }

            // Use StreamingTokenDecoder to decode the token list
            decoder = new StreamingTokenDecoder(context);
            decoder.AddRange(tokenList);
            var text5 = decoder.Read();  // Convert tokens to text

            Assert.NotEqual(text5, text50);
        }

        /// <summary>
        /// test frequency pentalty out of range exception when less than -2
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
        /// test frequency pentalty out of range exception when greater than 2
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

        /// <summary>
        /// Test the pipeline seed
        /// </summary>
        [Fact]
        public void Seed_IsInitializedWithRandomValue()
        {
            // Arrange
            var pipeline = new DefaultSamplingPipeline();

            // Act
            uint seed = pipeline.Seed;

            // Assert
            Assert.InRange(seed, 0u, uint.MaxValue);
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
