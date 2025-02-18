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
            _params = new ModelParams(Constants.GenerativeModelPath2)
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
        public void AdvancedModelProperties()
        {
            // These are the keys in the llama 7B test model. This will need changing if
            // tests are switched to use a new model!
            var expected = new Dictionary<string, string>
            {
                { "general.name", "SmolLM 360M" },
                { "general.architecture", "llama" },
                { "general.quantization_version", "2" },
                { "general.file_type", "7" },

                { "llama.context_length", "2048" },
                { "llama.rope.dimension_count", "64" },
                { "llama.embedding_length", "960" },
                { "llama.block_count", "32" },
                { "llama.feed_forward_length", "2560" },
                { "llama.attention.head_count", "15" },
                { "llama.attention.head_count_kv", "5" },
                { "llama.attention.layer_norm_rms_epsilon", "0.000010" },

                { "tokenizer.ggml.eos_token_id", "2" },
                { "tokenizer.ggml.model", "gpt2" },
                { "tokenizer.ggml.bos_token_id", "1" },
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