using System.Text;
using LLama.Common;
using LLama.Native;
using LLama.Extensions;

namespace LLama.Unittest.Native;

public class SafeLlamaModelHandleTests
{
    private readonly LLamaWeights _model;
    private readonly SafeLlamaModelHandle TestableHandle;

    public SafeLlamaModelHandleTests()
    {
        var @params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 1,
            GpuLayerCount = Constants.CIGpuLayerCount
        };
        _model = LLamaWeights.LoadFromFile(@params);

        TestableHandle = _model.NativeHandle;
    }

    [Fact]
    public void MetadataValByKey_ReturnsCorrectly()
    {
        const string key = "general.name";
        var template = _model.NativeHandle.MetadataValueByKey(key);
        var name = Encoding.UTF8.GetStringFromSpan(template!.Value.Span);

        const string expected = "LLaMA v2";
        Assert.Equal(expected, name);

        var metadataLookup = _model.Metadata[key];
        Assert.Equal(expected, metadataLookup);
        Assert.Equal(name, metadataLookup);
    }
}
