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
            EmbeddingMode = true,
        };
        using var weights = LLamaWeights.LoadFromFile(@params);
        _embedder = new(weights, @params);
    }

    public void Dispose()
    {
        _embedder.Dispose();
    }

    private static float Magnitude(float[] a)
    {
        return MathF.Sqrt(a.Zip(a, (x, y) => x * y).Sum());
    }

    private static void Normalize(float[] a)
    {
        var mag = Magnitude(a);
        for (var i = 0; i < a.Length; i++)
            a[i] /= mag;
    }

    private static float Dot(float[] a, float[] b)
    {
        Assert.Equal(a.Length, b.Length);
        return a.Zip(b, (x, y) => x * y).Sum();
    }

    [Fact]
    public async Task EmbedCompare()
    {
        var cat = await _embedder.GetEmbeddings("cat");
        var kitten = await _embedder.GetEmbeddings("kitten");
        var spoon = await _embedder.GetEmbeddings("spoon");

        Normalize(cat);
        Normalize(kitten);
        Normalize(spoon);

        var close = 1 - Dot(cat, kitten);
        var far = 1 - Dot(cat, spoon);

        Assert.True(close < far);

        _testOutputHelper.WriteLine($"Cat    = [{string.Join(",", cat.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Kitten = [{string.Join(",", kitten.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Spoon  = [{string.Join(",", spoon.AsMemory().Slice(0, 7).ToArray())}...]");
    }
}