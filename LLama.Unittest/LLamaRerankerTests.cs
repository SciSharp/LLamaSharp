using LLama.Common;
using LLama.Extensions;
using LLama.Native;
using Microsoft.Extensions.AI;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace LLama.Unittest;

public sealed class LLamaRerankerTests: IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly LLamaReranker _reranker;
    public LLamaRerankerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var @params = new ModelParams(Constants.RerankingModelPath)
        {
            ContextSize = 0,
            SeqMax = 1,
            PoolingType = LLamaPoolingType.Rank,
            GpuLayerCount = Constants.CIGpuLayerCount,
        };
        using var weights = LLamaWeights.LoadFromFile(@params);
        _reranker = new LLamaReranker(weights, @params);
    }

    public void Dispose()
    {
        _reranker.Dispose();
    }

    [Fact]
    public async Task CompareRerankingScore()
    {
        

        var input = "what is panda?";
        var documents = new string[] {
            "hi",
            "it's a bear",
            string.Join(", ","The giant panda (Ailuropoda melanoleuca)",
            "sometimes called a panda bear or simply panda",
            "is a bear species endemic to China.") 
        };
        var scores = await _reranker.GetRelevanceScores(input, documents, normalize: false);

        Assert.True(documents.Length == scores.Count);

        _testOutputHelper.WriteLine($"Rerank score 0: {scores[0]:F4}");
        _testOutputHelper.WriteLine($"Rerank score 1: {scores[1]:F4}");
        _testOutputHelper.WriteLine($"Rerank score 2: {scores[2]:F4}");
    }

    [Fact]
    public async Task MostRelevantDocument()
    {
        var input = "what is panda?";
        var documents = new string[] {
            "hi",
            "it's a bear",
            string.Join(", ","The giant panda (Ailuropoda melanoleuca)",
            "sometimes called a panda bear or simply panda",
            "is a bear species endemic to China.")
        };
        var scores = await _reranker.GetRelevanceScores(input, documents, normalize: true);

        Assert.NotNull(scores);
        Assert.True(documents.Length == scores.Count);

        int maxIndex = scores.Select((score, index) => (score, index))
                             .MaxBy(x => x.score)
                             .index;

        var maxScoreDocument = documents[maxIndex];
        Assert.Equal(documents[2], maxScoreDocument);
    }
}
