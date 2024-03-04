using LLama.Common;
using LLama.Native;

namespace LLama.Unittest
{
    // Test the same things as llama model + image embedings
    //
    public sealed class LLavaWeightTests
        : IDisposable
    {
        private readonly LLamaWeights _llamaWeights;
        private readonly LLavaWeights _lLavaWeights;
        private readonly LLamaContext _context;
        
        public LLavaWeightTests()
        {
            var @params = new ModelParams(Constants.ModelPath)
            {
                // Llava models requires big context
                ContextSize = 4096
            };
            _llamaWeights = LLamaWeights.LoadFromFile(@params);
            _lLavaWeights = LLavaWeights.LoadFromFile(Constants.LLavaMmpPath);
            
            _context = _llamaWeights.CreateContext(@params);
            
        }

        public void Dispose()
        {
            _llamaWeights.Dispose();
            _lLavaWeights.Dispose();
        }

        // [Fact]
        // public void CheckProperties()
        // {
        //     Assert.Equal(4096, (int)_context.ContextSize);
        //     Assert.Equal(4096, _context.EmbeddingSize);
        //     Assert.Equal(32000, _context.VocabCount);
        // }

        /*[Fact]
        public void Tokenize()
        {
            var tokens = _context.Tokenize("The quick brown fox", true);

            Assert.Equal(new LLamaToken[] { 1, 450, 4996, 17354, 1701, 29916 }, tokens);
        }

        [Fact]
        public void TokenizeNewline()
        {
            var tokens = _context.Tokenize("\n", false, false);

            Assert.Equal(new LLamaToken[] { 29871, 13 }, tokens);
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

            Assert.Equal(new LLamaToken[] { 450, 4996, 17354, 1701, 29916 }, tokens);
        }

        [Fact]
        public void TokenizeEmpty()
        {
            var tokens = _context.Tokenize("", false);

            Assert.Equal(Array.Empty<LLamaToken>(), tokens);
        }*/
        
        
        [Fact]
        public void EmbedImageAsFileName()
        {
            int n_past = 0;
            Assert.True( _lLavaWeights.EmbedImage( _context, Constants.LLavaImage, ref n_past ) );
        }        
        
        [Fact]
        public void EmbedImageAsBinary()
        {
            int n_past = 0;
            byte[] image = System.IO.File.ReadAllBytes(Constants.LLavaImage);
            Assert.True( _lLavaWeights.EmbedImage( _context, image, ref n_past ) );
        }        
        
    }
}
