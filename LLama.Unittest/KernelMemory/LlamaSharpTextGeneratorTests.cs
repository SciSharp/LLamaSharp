using LLamaSharp.KernelMemory;
using Xunit.Abstractions;

namespace LLama.Unittest.KernelMemory
{
    public class LlamaSharpTextGeneratorTests
        : ITextTokenizerTests, IDisposable
    {        
        private readonly LlamaSharpTextGenerator _textGenerator;

        public LlamaSharpTextGeneratorTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {            
            _textGenerator = new LlamaSharpTextGenerator(_lsConfig);

            _generator = _textGenerator;
        }

        public void Dispose()
        {
            _textGenerator.Dispose();
        }       
    }
}
