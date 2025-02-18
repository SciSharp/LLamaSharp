using System.Text;
using LLama.Common;
using Xunit.Abstractions;

namespace LLama.Unittest;

public class StreamingTextDecoderTests
    : IDisposable
{
    private readonly LLamaWeights _model;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ModelParams _params;

    public StreamingTextDecoderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _params = new ModelParams(Constants.GenerativeModelPath2);
        _model = LLamaWeights.LoadFromFile(_params);
    }

    public void Dispose()
    {
        _model.Dispose();
    }

    [Fact]
    public void DecodesSimpleText()
    {
        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);

        const string text = "The cat sat on the mat";
        var tokens = _model.NativeHandle.Tokenize(text, false, false, Encoding.UTF8);

        foreach (var lLamaToken in tokens)
            decoder.Add(lLamaToken);

        Assert.Equal(text, decoder.Read().Trim());
    }

    [Fact]
    public void DecodesComplexText()
    {
        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);

        const string text = "猫坐在垫子上 😀🤨🤐😏";
        var tokens = _model.NativeHandle.Tokenize(text, false, false, Encoding.UTF8);

        foreach (var lLamaToken in tokens)
            decoder.Add(lLamaToken);

        Assert.Equal(text, decoder.Read().Trim());
    }
}