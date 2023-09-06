using System.Text;
using LLama.Common;
using LLama.Extensions;

namespace LLama.Unittest;

public sealed class TokenTests
    : IDisposable
{
    private readonly ModelParams _params;
    private readonly LLamaWeights _model;

    public TokenTests()
    {
        _params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
        };
        _model = LLamaWeights.LoadFromFile(_params);
    }

    public void Dispose()
    {
        _model.Dispose();
    }

    [Fact]
    public void TokensEndWith()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString(new[]
        {
            "a fish",
            "the mat",
            "this is an improbably long query to be using for this method"
        }, _model.NativeHandle, Encoding.UTF8);
        Assert.True(result);
    }

    [Fact]
    public void TokensEndSubstring()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString(new[]
        {
            "at",
        }, _model.NativeHandle, Encoding.UTF8);
        Assert.True(result);
    }

    [Fact]
    public void TokensNotEndWith()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString(new[]
        {
            "a fish",
            "The cat sat on the edge of the ma",
            "this is an improbably long query to be using for this method"
        }, _model.NativeHandle, Encoding.UTF8);
        Assert.False(result);
    }

    [Fact]
    public void TokensNotEndWithNothing()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString(Array.Empty<string>(), _model.NativeHandle, Encoding.UTF8);
        Assert.False(result);
    }
}