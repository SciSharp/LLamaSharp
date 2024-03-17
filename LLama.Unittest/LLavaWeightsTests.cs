using LLama.Common;

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
                // Exactly enough context to embed the test image
                ContextSize = 2880
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



        [Fact(Skip = "Very slow in CI")]
        public void EmbedImageAsFileName()
        {
            int n_past = 0;
            Assert.True( _lLavaWeights.EmbedImage( _context, Constants.LLavaImage, ref n_past ) );
        }

        [Fact(Skip = "Very slow in CI")]
        public void EmbedImageAsBinary()
        {
            int n_past = 0;
            byte[] image = File.ReadAllBytes(Constants.LLavaImage);
            Assert.True( _lLavaWeights.EmbedImage( _context, image, ref n_past ) );
            Assert.Equal(2880, n_past);
        }        
        
    }
}
