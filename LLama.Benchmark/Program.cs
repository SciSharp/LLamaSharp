using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace LLama.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var modelDir = args[0];
                Constants.ModelDir = modelDir;
                Console.WriteLine($"#################### model dir: {modelDir}");
            }

            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            Console.WriteLine(summary);
        }
    }
}