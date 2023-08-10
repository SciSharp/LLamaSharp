using LLama.Common;

namespace LLama.Unittest;

public class LLamaEmbedderTests
    : IDisposable
{
    private readonly LLamaEmbedder _embedder = new(new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin"));

    public void Dispose()
    {
        _embedder.Dispose();
    }

    private static float Dot(float[] a, float[] b)
    {
        Assert.Equal(a.Length, b.Length);
        return a.Zip(b, (x, y) => x + y).Sum();
    }

    private static void AssertApproxStartsWith(float[] array, float[] start, float epsilon = 0.00001f)
    {
        for (int i = 0; i < start.Length; i++)
            Assert.Equal(array[i], start[i], epsilon);
    }

    [Fact]
    public void EmbedBasic()
    {
        var hello = _embedder.GetEmbeddings("cat");

        Assert.NotNull(hello);
        Assert.NotEmpty(hello);
        //Assert.Equal(_embedder.EmbeddingSize, hello.Length);

        // Expected value generate with llama.cpp embedding.exe
        var expected = new float[] { -0.127304f, -0.678057f, -0.085244f, -0.956915f, -0.638633f };
        AssertApproxStartsWith(hello, expected);
    }

    [Fact]
    public void EmbedCompare()
    {
        var cat = _embedder.GetEmbeddings("cat");
        var kitten = _embedder.GetEmbeddings("kitten");
        var spoon = _embedder.GetEmbeddings("spoon");

        Console.WriteLine(string.Join(",", cat));

        var close = Dot(cat, kitten);
        var far = Dot(cat, spoon);

        Assert.True(close < far);
    }
}