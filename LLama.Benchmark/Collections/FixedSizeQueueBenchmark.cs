using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using LLama.Common;

namespace LLama.Benchmark.Collections;

[SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[BenchmarkCategory("Collections", "FixedSizeQueue")]
public class FixedSizeQueueBenchmark
{
    [Params(32, 512, 4096)]
    public int Capacity { get; set; }

    private int[] _values = Array.Empty<int>();

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(0, Capacity * 4).ToArray();
    }

    [Benchmark]
    public int EnqueueWrap()
    {
        var queue = new FixedSizeQueue<int>(Capacity);
        foreach (var value in _values)
            queue.Enqueue(value);
        return queue.Count;
    }

    [Benchmark]
    public int IterateTailSum()
    {
        var queue = new FixedSizeQueue<int>(Capacity);
        foreach (var value in _values)
            queue.Enqueue(value);

        var sum = 0;
        foreach (var value in queue)
            sum += value;
        return sum;
    }
}
