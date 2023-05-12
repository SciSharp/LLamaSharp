using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Examples
{
    public class GetEmbeddings
    {
        LLamaEmbedder _embedder;
        public GetEmbeddings(string modelPath)
        {
            _embedder = new LLamaEmbedder(new LLamaParams(model: modelPath));
        }

        public void Run(string text)
        {
            Console.WriteLine(string.Join(", ", _embedder.GetEmbeddings(text)));
        }
    }
}
