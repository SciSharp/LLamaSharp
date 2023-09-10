using LLama.Common;

namespace LLama.Unittest
{
    public class LLamaContextTests
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
            Assert.Equal(0, _context.KVCacheTokenCount);
        }
    }
}
