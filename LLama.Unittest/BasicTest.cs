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
            Assert.Equal(128256, _model.VocabCount);
            Assert.Equal(131072, _model.ContextSize);
            Assert.Equal(2048, _model.EmbeddingSize);
        }

        [Fact]
        public void AdvancedModelProperties()
        {
            // These are the keys in the llama 7B test model. This will need changing if
            // tests are switched to use a new model!
            var expected = new Dictionary<string, string>
            {
                { "general.name", "Llama 3.2 1B Instruct" },
                { "general.architecture", "llama" },
                { "general.quantization_version", "2" },
                { "general.file_type", "2" },

                { "llama.context_length", "131072" },
                { "llama.rope.dimension_count", "64" },
                { "llama.embedding_length", "2048" },
                { "llama.block_count", "16" },
                { "llama.feed_forward_length", "8192" },
                { "llama.attention.head_count", "32" },
                { "llama.attention.head_count_kv", "8" },
                { "llama.attention.layer_norm_rms_epsilon", "0.000010" },

                { "tokenizer.ggml.eos_token_id", "128009" },
                { "tokenizer.ggml.model", "gpt2" },
                { "tokenizer.ggml.bos_token_id", "128000" },
            };

            // Print all keys
            foreach (var (key, value) in _model.Metadata)
                _testOutputHelper.WriteLine($"{key} = {value}");

            // Check every key
            foreach (var (key, value) in expected)
                Assert.Equal(_model.Metadata[key], value);
        }
    }
}