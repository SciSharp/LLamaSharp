namespace LLama.Unittest
{
    public sealed class TextTransformTests
    {
        [Fact]
        public void NaiveTextInputTransformTrimsText()
        {
            var transform = new LLamaTransforms.NaiveTextInputTransform();

            Assert.Equal("hello", transform.Transform("hello"));
            Assert.Equal("hello", transform.Transform(" hello"));
            Assert.Equal("hello", transform.Transform("hello "));
            Assert.Equal("hello", transform.Transform(" hello "));
            Assert.Equal("hello world", transform.Transform(" hello world "));
        }

        [Fact]
        public async Task EmptyTextOutputStreamTransformDoesNothing()
        {
            var input = new[] { "Hello", "world" };

            var transform = new LLamaTransforms.EmptyTextOutputStreamTransform();

            Assert.Equal(input, await transform.TransformAsync(input.ToAsyncEnumerable()).ToArrayAsync());
        }
    }
}
