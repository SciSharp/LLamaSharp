using LLama.Common;
using LLama.Extensions;
using LLama.Native;
using Xunit.Abstractions;

namespace LLama.Unittest;

public sealed class LLamaEmbedderTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LLamaEmbedderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static float Dot(float[] a, float[] b)
    {
        Assert.Equal(a.Length, b.Length);
        return a.Zip(b, (x, y) => x * y).Sum();
    }

    private async Task CompareEmbeddings(string modelPath)
    {
        var @params = new ModelParams(modelPath)
        {
            ContextSize = 8,
            Threads = 4,
            GpuLayerCount = Constants.CIGpuLayerCount,
            PoolingType = LLamaPoolingType.Mean,
        };
        using var weights = LLamaWeights.LoadFromFile(@params);
        using var embedder = new LLamaEmbedder(weights, @params);

        var cat = (await embedder.GetEmbeddings("The cat is cute")).Single().EuclideanNormalization();
        Assert.DoesNotContain(float.NaN, cat);

        var kitten = (await embedder.GetEmbeddings("The kitten is cute")).Single().EuclideanNormalization();
        Assert.DoesNotContain(float.NaN, kitten);

        var spoon = (await embedder.GetEmbeddings("The spoon is not real")).Single().EuclideanNormalization();
        Assert.DoesNotContain(float.NaN, spoon);

        _testOutputHelper.WriteLine($"Cat    = [{string.Join(",", cat.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Kitten = [{string.Join(",", kitten.AsMemory().Slice(0, 7).ToArray())}...]");
        _testOutputHelper.WriteLine($"Spoon  = [{string.Join(",", spoon.AsMemory().Slice(0, 7).ToArray())}...]");

        var close = 1 - Dot(cat, kitten);
        var far = 1 - Dot(cat, spoon);

        _testOutputHelper.WriteLine("");
        _testOutputHelper.WriteLine($"Cat.Kitten (Close): {close:F4}");
        _testOutputHelper.WriteLine($"Cat.Spoon  (Far):   {far:F4}");

        Assert.True(close < far);
    }

    [Fact]
    public async Task EmbedCompareEmbeddingModel()
    {
        await CompareEmbeddings(Constants.EmbeddingModelPath);
    }

    [Fact]
    public async Task EmbedCompareGenerateModel()
    {
        await CompareEmbeddings(Constants.GenerativeModelPath);
    }

    private async Task NonPooledEmbeddings(string modelPath)
    {
        var @params = new ModelParams(modelPath)
        {
            ContextSize = 8,
            Threads = 4,
            GpuLayerCount = Constants.CIGpuLayerCount,
            PoolingType = LLamaPoolingType.None,
        };
        using var weights = LLamaWeights.LoadFromFile(@params);
        using var embedder = new LLamaEmbedder(weights, @params);

        var kitten = await embedder.GetEmbeddings("the kitten is kawaii");
        foreach (var embd in kitten)
            Assert.DoesNotContain(float.NaN, embd);
    }

    [Fact]
    public async Task EmbeddingModelNonPooledEmbeddings()
    {
        await NonPooledEmbeddings(Constants.EmbeddingModelPath);
    }

    [Fact]
    public async Task GenerativeModelNonPooledEmbeddings()
    {
        await NonPooledEmbeddings(Constants.GenerativeModelPath);
    }
}