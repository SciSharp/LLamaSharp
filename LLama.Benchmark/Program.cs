using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace LLama.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            Console.WriteLine(summary);
        }
    }
}