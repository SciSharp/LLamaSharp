using System.Text;
using System.Xml.Linq;
using LLama.Common;
using LLama.Extensions;
using Microsoft.Extensions.Logging;


namespace LLama.Unittest.Native;

public class SafeLlamaModelHandleVocabularyTests
{
    private readonly LLamaWeights _model;

    public SafeLlamaModelHandleVocabularyTests()
    {
        var @params = new ModelParams(Constants.RerankingModelPath)
        {
            ContextSize = 0,
            PoolingType = LLama.Native.LLamaPoolingType.Rank,
            GpuLayerCount = Constants.CIGpuLayerCount
        };
        _model = LLamaWeights.LoadFromFile(@params);
    }

    [Fact]
    public void GetLLamaTokenString()
    {
        var bos = _model.Vocab.BOS;
        var eos = _model.Vocab.EOS;

        var bosStr = _model.Vocab.LLamaTokenToString(bos, true);
        var eosStr = _model.Vocab.LLamaTokenToString(eos, true);

        Assert.Equal("<s>", bosStr);
        Assert.Equal("</s>", eosStr);
    }
}
