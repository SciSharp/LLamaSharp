using System.Text;
using LLama.Common;
using LLama.Extensions;

namespace LLama.Unittest.Native;

public class SafeLlamaModelHandleTests
{
    private readonly LLamaWeights _model;

    public SafeLlamaModelHandleTests()
    {
        var @params = new ModelParams(Constants.GenerativeModelPath2)
        {
            ContextSize = 1,
            GpuLayerCount = Constants.CIGpuLayerCount
        };
        _model = LLamaWeights.LoadFromFile(@params);
    }

    [Fact]
    public void MetadataValByKey_ReturnsCorrectly()
    {
        const string key = "general.name";
        var template = _model.NativeHandle.MetadataValueByKey(key);
        var name = Encoding.UTF8.GetStringFromSpan(template!.Value.Span);

        const string expected = "SmolLM 360M";
        Assert.Equal(expected, name);

        var metadataLookup = _model.Metadata[key];
        Assert.Equal(expected, metadataLookup);
        Assert.Equal(name, metadataLookup);
    }
}
