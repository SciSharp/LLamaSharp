using LLama.Common;

namespace LLama.Unittest
{
    public class LLamaEmbedderTests
        : IDisposable
    {
        private readonly LLamaEmbedder _embedder = new(new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin"));

        public void Dispose()
        {
            _embedder.Dispose();
        }

        private static float Dot(float[] a, float[] b)
        {
            Assert.Equal(a.Length, b.Length);
            return a.Zip(b, (x, y) => x + y).Sum();
        }

        [Fact]
        public void EmbedHello()
        {
            var hello = _embedder.GetEmbeddings("Hello");

            Assert.NotNull(hello);
            Assert.NotEmpty(hello);
            Assert.Equal(_embedder.EmbeddingSize, hello.Length);
        }

        [Fact]
        public void EmbedCompare()
        {
            var cat = _embedder.GetEmbeddings("cat");
            var kitten = _embedder.GetEmbeddings("kitten");
            var spoon = _embedder.GetEmbeddings("spoon");

            var close = Dot(cat, kitten);
            var far = Dot(cat, spoon);

            Assert.True(close < far);
        }
    }
}
