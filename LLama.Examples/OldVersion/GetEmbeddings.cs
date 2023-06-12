using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.OldVersion;

namespace LLama.Examples.Old
{
    public class GetEmbeddings
    {
        LLama.OldVersion.LLamaEmbedder _embedder;
        public GetEmbeddings(string modelPath)
        {
            _embedder = new LLama.OldVersion.LLamaEmbedder(new LLamaParams(model: modelPath));
        }

        public void Run(string text)
        {
            Console.WriteLine(string.Join(", ", _embedder.GetEmbeddings(text)));
        }
    }
}
