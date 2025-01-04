using LLamaSharp.KernelMemory;
using Xunit.Abstractions;

namespace LLama.Unittest.KernelMemory
{
    public class LLamaSharpTextEmbeddingGeneratorTests
        : ITextTokenizerTests, IDisposable
    {
        private readonly LLamaSharpTextEmbeddingGenerator _embeddingGenerator;

        public LLamaSharpTextEmbeddingGeneratorTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            _embeddingGenerator = new LLamaSharpTextEmbeddingGenerator(_lsConfig);
            
            _generator = _embeddingGenerator;
        }

        public void Dispose()
        {
            _embeddingGenerator.Dispose();
        }       
    }
}
