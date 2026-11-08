using LLama.Extensions;

namespace LLama.Unittest;

public class SpanNormalizationExtensionsTests
{
    [Fact]
    public void PNormalizationP1WithNegativeValues()
    {
        var vector = new[] { 1f, -1f, 2f, -4f };

        vector.PNormalization(1);

        // p=1 is taxicab normalization, so the result should match it
        var expected = new[] { 1f, -1f, 2f, -4f }.TaxicabNormalization();
        Assert.Equal(expected, vector, (a, b) => Math.Abs(a - b) < 1e-6f);
    }

    [Fact]
    public void PNormalizationOddPWithNegativeValues()
    {
        var vector = new[] { 3f, -3f };

        vector.PNormalization(3);

        // ||v||_3 = (|3|^3 + |-3|^3)^(1/3)
        var norm = MathF.Pow(54f, 1f / 3f);
        Assert.Equal(new[] { 3f / norm, -3f / norm }, vector, (a, b) => Math.Abs(a - b) < 1e-6f);
    }

    [Fact]
    public void ZeroVectorArrayStaysZero()
    {
        Assert.Equal(new float[3], new float[3].MaxAbsoluteNormalization());
        Assert.Equal(new float[3], new float[3].TaxicabNormalization());
        Assert.Equal(new float[3], new float[3].EuclideanNormalization());
        Assert.Equal(new float[3], new float[3].PNormalization(1));
        Assert.Equal(new float[3], new float[3].PNormalization(2));
        Assert.Equal(new float[3], new float[3].PNormalization(3));
    }

    [Fact]
    public void ZeroVectorSpanStaysZero()
    {
        Assert.Equal(new float[3], new float[3].AsSpan().MaxAbsoluteNormalization().ToArray());
        Assert.Equal(new float[3], new float[3].AsSpan().TaxicabNormalization().ToArray());
        Assert.Equal(new float[3], new float[3].AsSpan().EuclideanNormalization().ToArray());
        Assert.Equal(new float[3], new float[3].AsSpan().PNormalization(3).ToArray());
        Assert.Equal(new float[3], ((ReadOnlySpan<float>)new float[3]).EuclideanNormalization());
    }
}
