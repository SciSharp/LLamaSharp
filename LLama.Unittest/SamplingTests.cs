using LLama.Common;
using LLama.Native;

using System.Numerics.Tensors;
using System.Runtime.InteropServices;
using System.Text;

using Xunit.Abstractions;

namespace LLama.Unittest
{
    public class SamplingTests : IDisposable
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
            _params = new ModelParams(Constants.GenerativeModelPath) {
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
                    var rawLogits = new float[_model.VocabCount];
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
                    var logits = all_logits.Slice(b * _model.VocabCount, _model.VocabCount);

                    // Test raw sampling
                    Assert.Equal(expected, TensorPrimitives.IndexOfMax(logits));

                    // Test native sampling with `LLamaTokenDataArrayNative`.
                    var array = LLamaTokenDataArray.Create(logits);
                    {
                        using var _ = LLamaTokenDataArrayNative.Create(array, out var cur_p);
                        var rawLogits = new float[_model.VocabCount];
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
                vocabSize: context.VocabCount,
                eos: context.ModelHandle.Tokens.EOS,
                newline: context.ModelHandle.Tokens.Newline ?? 0,
                penaltyCount: 60, repeat: 1, freq: 0, presence: 0,
                penalizeNewline: false, ignoreEOS: false
            );

            if (logit_bias != null) { chain.AddLogitBias(context.VocabCount, logit_bias); }

            chain.AddTopK(10);
            chain.AddTemperature(0.1f);
            chain.AddDistributionSampler(seed: 42);

            return chain;
        }
    }
}
