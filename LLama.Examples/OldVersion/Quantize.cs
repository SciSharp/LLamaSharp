using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Examples.Old
{
    public class Quantize
    {
        public Quantize()
        {

        }

        public void Run(string srcFileName, string dstFilename, string ftype, int nthread = -1)
        {
            if(LLamaQuantizer.Quantize(srcFileName, dstFilename, ftype, nthread))
            {
                Console.WriteLine("Quantization succeed!");
            }
            else
            {
                Console.WriteLine("Quantization failed!");
            }
        }
    }
}
