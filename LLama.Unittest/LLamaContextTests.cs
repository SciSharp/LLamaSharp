using LLama.Common;
using LLama.Native;

namespace LLama.Unittest
{
    public sealed class LLamaContextTests
        : IDisposable
    {
        private readonly LLamaWeights _weights;
        private readonly LLamaContext _context;

        public LLamaContextTests()
        {
            var @params = new ModelParams(Constants.GenerativeModelPath)
            {
                ContextSize = 128,
                GpuLayerCount = Constants.CIGpuLayerCount,
            };
            _weights = LLamaWeights.LoadFromFile(@params);
            _context = _weights.CreateContext(@params);
        }

        public void Dispose()
        {
            _weights.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void CheckProperties()
        {
            Assert.Equal(128u, _context.ContextSize);
            Assert.Equal(2048, _context.EmbeddingSize);
            Assert.Equal(128256, _context.VocabCount);
        }

        [Fact]
        public void Tokenize()
        {
            var tokens = _context.Tokenize("The quick brown fox", true);

            Assert.Equal(new LLamaToken[] { 128000, 791, 4062, 14198, 39935 }, tokens);
        }

        [Fact]
        public void TokenizeNewline()
        {
            var tokens = _context.Tokenize("\n", false, false);

            Assert.Equal(new LLamaToken[] { 198 }, tokens);
        }

        [Fact]
        public void TokenizeRoundtripSpecialStrings()
        {
            var strings = new[]
            {
                "\t", "\t\t", "\t\t\t",
                "\n\n", "\n\n\n", "\n\n\n\n",
                "\t\n", "\t\n\t\n\n\n\n\t\t",
                "\b", "\v", "\0"
            };

            foreach (var s in strings)
            {
                var tokens = _context.Tokenize(s, false, false);
                var decoder = new StreamingTokenDecoder(_context);
                decoder.AddRange(tokens);

                var str = decoder.Read();

                Assert.Equal(s, str.TrimStart(' '));
            }
        }

        [Fact]
        public void TokenizeWithoutBOS()
        {
            var tokens = _context.Tokenize("The quick brown fox", false);

            Assert.Equal(new LLamaToken[] { 791, 4062, 14198, 39935 }, tokens);
        }

        [Fact]
        public void TokenizeEmpty()
        {
            var tokens = _context.Tokenize("", false);

            Assert.Equal(Array.Empty<LLamaToken>(), tokens);
        }

        [Fact]
        public void SaveLoadState()
        {
            using var state1 = _context.GetState();

            var stream = new MemoryStream();
            state1.Save(stream);

            stream.Position = 0;

            using var state2 = LLamaContext.State.Load(stream);

            Assert.Equal(state1.Size, state2.Size);
        }

        [Fact]
        public async Task SaveLoadStateAsync()
        {
            using var state1 = _context.GetState();

            var stream = new MemoryStream();
            await state1.SaveAsync(stream);

            stream.Position = 0;

            using var state2 = await LLamaContext.State.LoadAsync(stream);

            Assert.Equal(state1.Size, state2.Size);
        }
    }
}
