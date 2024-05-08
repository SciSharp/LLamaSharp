using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace LLama.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            Console.WriteLine(summary);
        }
    }
}