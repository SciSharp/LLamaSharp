using System.Text;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Unittest
{
    /// <summary>
    /// A prompt is fed to the model in BatchSize chunks. If a chunk happens to end on an end-of-generation
    /// token (e.g. the end-of-turn marker of a chat template), the executor must still generate a reply.
    /// See https://github.com/SciSharp/LLamaSharp/issues/1465
    /// </summary>
    public sealed class PromptBatchBoundaryTests
        : IDisposable
    {
        private const string Prompt = "<|im_start|>system\nYou are a helpful assistant.<|im_end|>\n<|im_start|>user\nWhat is the capital of France?<|im_end|>\n<|im_start|>assistant\n";

        private readonly LLamaWeights _weights;

        public PromptBatchBoundaryTests()
        {
            _weights = LLamaWeights.LoadFromFile(new ModelParams(Constants.GenerativeModelPath2));
        }

        public void Dispose()
        {
            _weights.Dispose();
        }

        /// <summary>
        /// Create a context whose BatchSize makes the first prompt chunk end exactly on the first end-of-generation token of the prompt
        /// </summary>
        private LLamaContext CreateContextWithChunkEndingOnEog()
        {
            var tokens = _weights.Tokenize(Prompt, true, true, Encoding.UTF8);
            var eogIndex = Array.FindIndex(tokens, t => t.IsEndOfGeneration(_weights.Vocab));
            Assert.True(eogIndex > 0, "Prompt should contain an end-of-generation token");
            Assert.True(eogIndex < tokens.Length - 1, "End-of-generation token should not be the last prompt token");

            var @params = new ModelParams(Constants.GenerativeModelPath2)
            {
                ContextSize = 256,
                BatchSize = (uint)(eogIndex + 1),
                GpuLayerCount = Constants.CIGpuLayerCount,
            };
            return _weights.CreateContext(@params);
        }

        private static InferenceParams CreateInferenceParams()
        {
            return new InferenceParams
            {
                MaxTokens = 8,
                AntiPrompts = ["<|im_end|>"],
                SamplingPipeline = new GreedySamplingPipeline(),
            };
        }

        [Fact]
        public async Task InteractiveExecutor_PromptChunkEndingOnEog_StillGeneratesReply()
        {
            using var context = CreateContextWithChunkEndingOnEog();
            var executor = new InteractiveExecutor(context);

            var reply = new StringBuilder();
            await foreach (var text in executor.InferAsync(Prompt, CreateInferenceParams(), TestContext.Current.CancellationToken))
                reply.Append(text);

            Assert.False(string.IsNullOrWhiteSpace(reply.ToString()));
        }

        [Fact]
        public async Task InstructExecutor_PromptChunkEndingOnEos_StillGeneratesReply()
        {
            using var context = CreateContextWithChunkEndingOnEog();
            Assert.True(_weights.Vocab.EOS.HasValue);

            // Only meaningful if the EOG token found in the prompt is the EOS token, which InstructExecutor checks for
            var tokens = _weights.Tokenize(Prompt, true, true, Encoding.UTF8);
            Assert.Equal(_weights.Vocab.EOS!.Value, tokens[(int)context.BatchSize - 1]);

            var executor = new InstructExecutor(context);

            var reply = new StringBuilder();
            await foreach (var text in executor.InferAsync(Prompt, CreateInferenceParams(), TestContext.Current.CancellationToken))
                reply.Append(text);

            // InstructExecutor emits "\n> " when it stops to wait for input, ignore that
            Assert.False(string.IsNullOrWhiteSpace(reply.ToString().Replace("\n> ", "")));
        }
    }
}
