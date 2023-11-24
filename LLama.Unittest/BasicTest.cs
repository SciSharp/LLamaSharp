using System.Text;
using LLama.Common;
using LLama.Native;
using Xunit.Abstractions;

namespace LLama.Unittest
{
    public sealed class BasicTest
        : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ModelParams _params;
        private readonly LLamaWeights _model;

        public BasicTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _params = new ModelParams(Constants.ModelPath)
            {
                ContextSize = 2048
            };
            _model = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        [Fact]
        public void BasicModelProperties()
        {
            Assert.Equal(32000, _model.VocabCount);
            Assert.Equal(4096, _model.ContextSize);
            Assert.Equal(4096, _model.EmbeddingSize);
        }

        [Fact]
        public void AdvancedModelProperties()
        {
            var expected = new Dictionary<string, string>
            {
                { "general.name", "LLaMA v2" },
                { "general.architecture", "llama" },
                { "general.quantization_version", "2" },
                { "general.file_type", "2" },

                { "llama.context_length", "4096" },
                { "llama.rope.dimension_count", "128" },
                { "llama.embedding_length", "4096" },
                { "llama.block_count", "32" },
                { "llama.feed_forward_length", "11008" },
                { "llama.attention.head_count", "32" },
                { "llama.attention.head_count_kv", "32" },
                { "llama.attention.layer_norm_rms_epsilon", "0.000001" },

                { "tokenizer.ggml.eos_token_id", "2" },
                { "tokenizer.ggml.model", "llama" },
                { "tokenizer.ggml.bos_token_id", "1" },
                { "tokenizer.ggml.unknown_token_id", "0" },
            };

            var metaCount = NativeApi.llama_model_meta_count(_model.NativeHandle);
            Assert.Equal(expected.Count, metaCount);

            Span<byte> buffer = stackalloc byte[128];
            for (var i = 0; i < expected.Count; i++)
            {
                unsafe
                {
                    fixed (byte* ptr = buffer)
                    {
                        var length = NativeApi.llama_model_meta_key_by_index(_model.NativeHandle, i, ptr, 128);
                        Assert.True(length > 0);
                        var key = Encoding.UTF8.GetString(buffer[..length]);

                        length = NativeApi.llama_model_meta_val_str_by_index(_model.NativeHandle, i, ptr, 128);
                        Assert.True(length > 0);
                        var val = Encoding.UTF8.GetString(buffer[..length]);

                        _testOutputHelper.WriteLine($"{key} == {val}");

                        Assert.True(expected.ContainsKey(key));
                        Assert.Equal(expected[key], val);
                    }
                }
            }
        }
    }
}