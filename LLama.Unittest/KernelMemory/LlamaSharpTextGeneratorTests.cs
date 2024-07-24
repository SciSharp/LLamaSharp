using LLama.Common;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using static System.Net.Mime.MediaTypeNames;

namespace LLama.Unittest.KernelMemory
{
    public class LlamaSharpTextGeneratorTests : ITextTokenizerTests, IDisposable
    {        
        private readonly LlamaSharpTextGenerator _textGenerator;

        public LlamaSharpTextGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
