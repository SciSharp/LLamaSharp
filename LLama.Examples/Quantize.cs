using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Examples
{
    public class Quantize
    {
        public Quantize()
        {

        }

        public void Run(string srcFileName, string dstFilename, string ftype, int nthread = 0, bool printInfo = true)
        {
            if(Quantizer.Quantize(srcFileName, dstFilename, ftype, nthread, printInfo))
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
