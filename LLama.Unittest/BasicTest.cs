using LLama.Common;
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
            _params = new ModelParams(Constants.GenerativeModelPath)
            {
                ContextSize = 128,
                GpuLayerCount = Constants.CIGpuLayerCount
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
            // These are the keys in the llama 7B test model. This will need changing if
            // tests are switched to use a new model!
            var expected = new Dictionary<string, string>
            {
                { "general.name", "LLaMA v2" },
                { "general.architecture", "llama" },
                { "general.quantization_version", "2" },
                { "general.file_type", "11" },

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

            // Print all keys
            foreach (var (key, value) in _model.Metadata)
                _testOutputHelper.WriteLine($"{key} = {value}");

            // Check the count is equal
            Assert.Equal(expected.Count, _model.Metadata.Count);

            // Check every key
            foreach (var (key, value) in _model.Metadata)
                Assert.Equal(expected[key], value);
        }
    }
}