using LLama.Common;

namespace LLama.Unittest
{
    public sealed class LLamaContextTests
        : IDisposable
    {
        private readonly LLamaWeights _weights;
        private readonly LLamaContext _context;

        public LLamaContextTests()
        {
            var @params = new ModelParams(Constants.ModelPath)
            {
                ContextSize = 768,
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
            Assert.Equal(768, _context.ContextSize);
            Assert.Equal(4096, _context.EmbeddingSize);
            Assert.Equal(32000, _context.VocabCount);
        }

        [Fact]
        public void Tokenize()
        {
            var tokens = _context.Tokenize("The quick brown fox", true);

            Assert.Equal(new[] { 1, 450, 4996, 17354, 1701, 29916 }, tokens);
        }

        [Fact]
        public void TokenizeWithoutBOS()
        {
            var tokens = _context.Tokenize("The quick brown fox", false);

            Assert.Equal(new[] { 450, 4996, 17354, 1701, 29916 }, tokens);
        }

        [Fact]
        public void TokenizeEmpty()
        {
            var tokens = _context.Tokenize("", false);

            Assert.Equal(Array.Empty<int>(), tokens);
        }
    }
}
