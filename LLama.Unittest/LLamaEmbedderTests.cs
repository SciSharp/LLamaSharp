using LLama.Common;
using Xunit.Abstractions;

namespace LLama.Unittest;

public sealed class LLamaEmbedderTests
    : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly LLamaEmbedder _embedder;

    public LLamaEmbedderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 4096,
            Threads = 5,
            EmbeddingMode = true,
        };
        using var weights = LLamaWeights.LoadFromFile(@params);
        _embedder = new(weights, @params);
    }

    public void Dispose()
    {
        _embedder.Dispose();
    }

    private static float Dot(float[] a, float[] b)
    {
        Assert.Equal(a.Length, b.Length);
        return a.Zip(b, (x, y) => x * y).Sum();
    }


    [Fact]
    public async Task EmbedCompare()
    {
        var cat = await _embedder.GetEmbeddings("The cat is cute");
        var kitten = await _embedder.GetEmbeddings("The kitten is kawaii");
        var spoon = await _embedder.GetEmbeddings("The spoon is not real");

        _testOutputHelper.WriteLine($"Cat    = [{string.Join(",", cat.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Kitten = [{string.Join(",", kitten.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Spoon  = [{string.Join(",", spoon.AsMemory().Slice(0, 7).ToArray())}...]");

        var close = 1 - Dot(cat, kitten);
        var far = 1 - Dot(cat, spoon);
        Assert.True(close < far);
    }
}