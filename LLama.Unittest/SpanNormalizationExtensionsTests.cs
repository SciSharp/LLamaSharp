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
}
