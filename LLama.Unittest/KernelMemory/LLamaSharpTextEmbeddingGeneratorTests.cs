using LLama.Common;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LLama.Unittest.KernelMemory
{
    public class LLamaSharpTextEmbeddingGeneratorTests : ITextTokenizerTests, IDisposable
    {
        private readonly LLamaSharpTextEmbeddingGenerator _embeddingGenerator;

        public LLamaSharpTextEmbeddingGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
