using LLama.Common;

namespace LLama.Unittest;

public class LLamaEmbedderTests
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

    private static void AssertApproxStartsWith(float[] expected, float[] actual, float epsilon = 0.08f)
    {
        for (int i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], actual[i], epsilon);
    }

    // todo: enable this one llama2 7B gguf is available
    //[Fact]
    //public void EmbedBasic()
    //{
    //    var cat = _embedder.GetEmbeddings("cat");

    //    Assert.NotNull(cat);
    //    Assert.NotEmpty(cat);

    //    // Expected value generate with llama.cpp embedding.exe
    //    var expected = new float[] { -0.127304f, -0.678057f, -0.085244f, -0.956915f, -0.638633f };
    //    AssertApproxStartsWith(expected, cat);
    //}

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