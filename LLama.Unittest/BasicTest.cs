using System.Text;
using LLama.Common;

namespace LLama.Unittest
{
    public sealed class BasicTest
        : IDisposable
    {
        private readonly ModelParams _params;
        private readonly LLamaWeights _model;

        public BasicTest()
        {
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
            Assert.Equal(Encoding.UTF8, _model.Encoding);
        }

        [Fact]
        public void CloneContext()
        {
            var original = _model.CreateContext(_params);

            // Evaluate something (doesn't matter what, as long as it begins with token 1)
            original.Eval(new[] { 1, 42, 321 }, 0);

            // Clone current state
            var clone = original.Clone();

            // Now evaluate something more
            var reply1a = original.Eval(new[] { 4, 5, 6 }, 3);
            var reply2a = original.Eval(new[] { 7, 8, 9 }, 6);

            // Assert that the context replied differently each time
            Assert.NotEqual(reply1a, reply2a);

            // Give the same prompts to the cloned state
            var reply1b = clone.Eval(new[] { 4, 5, 6 }, 3);
            var reply2b = clone.Eval(new[] { 7, 8, 9 }, 6);

            // Assert that the cloned context replied in the same way as originally
            Assert.Equal(reply1a, reply1b);
            Assert.Equal(reply2a, reply2b);
        }
    }
}