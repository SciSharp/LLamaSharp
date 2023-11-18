using LLama.Common;

namespace LLama.Unittest;

public sealed class LLamaEmbedderTests
    : IDisposable
{
    private readonly LLamaEmbedder _embedder;

    public LLamaEmbedderTests()
    {
        var @params = new ModelParams(Constants.ModelPath);
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
    public void EmbedCompare()
    {
        var cat = _embedder.GetEmbeddings("cat");
        var kitten = _embedder.GetEmbeddings("kitten");
        var spoon = _embedder.GetEmbeddings("spoon");

        Normalize(cat);
        Normalize(kitten);
        Normalize(spoon);

        var close = Dot(cat, kitten);
        var far = Dot(cat, spoon);

        // This comparison seems backwards, but remember that with a
        // dot product 1.0 means **identical** and 0.0 means **completely opposite**!
        Assert.True(close > far);
    }
}