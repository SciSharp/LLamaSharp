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
        _params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 2048,
            GpuLayerCount = Constants.CIGpuLayerCount,
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
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

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
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString((IList<string>)new[]
        {
            "at",
        }, _model.NativeHandle, Encoding.UTF8);
        Assert.True(result);
    }

    [Fact]
    public void TokensNotEndWith()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString((IList<string>)new[]
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
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var result = tokens.TokensEndsWithAnyString((IList<string>)Array.Empty<string>(), _model.NativeHandle, Encoding.UTF8);
        Assert.False(result);
    }

    [Fact]
    public void TokensEndWith2()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);
        decoder.AddRange(tokens);

        var processor = new AntipromptProcessor(new[]
        {
            "a fish",
            "the mat",
            "this is an improbably long query to be using for this method"
        });
        var result = processor.Add(decoder.Read());

        Assert.True(result);
    }

    [Fact]
    public void TokensEndSubstring2()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);
        decoder.AddRange(tokens);

        var processor = new AntipromptProcessor(new[] { "at" });
        var result = processor.Add(decoder.Read());

        Assert.True(result);
    }

    [Fact]
    public void TokensNotEndWith2()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);
        decoder.AddRange(tokens);

        var processor = new AntipromptProcessor(new[]
        {
            "a fish",
            "The cat sat on the edge of the ma",
            "this is an improbably long query to be using for this method"
        });
        var result = processor.Add(decoder.Read());

        Assert.False(result);
    }

    [Fact]
    public void TokensNotEndWithNothing2()
    {
        var tokens = _model.NativeHandle.Tokenize("The cat sat on the edge of the mat", false, true, Encoding.UTF8);

        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);
        decoder.AddRange(tokens);

        var processor = new AntipromptProcessor();
        var result = processor.Add(decoder.Read());

        Assert.False(result);
    }

    [Fact]
    public void RoundTrip()
    {
        var strings = new[]
        {
            "Hello world",
            "철수",
            "😀 😃 😄 😁 😆철수😅 😂 😊 😇 🙂 ",
        };

        var charsArr = new char[1024];

        foreach (var input in strings)
        {
            // Convert into llama tokens
            var tokens = _model.NativeHandle.Tokenize(input, false, false, Encoding.UTF8);

            // Convert tokens back into characters
            var chars = _model.NativeHandle.TokensToSpan(tokens, charsArr.AsSpan(), Encoding.UTF8);

            // llama.cpp adds a space to the start of strings, remove that
            var output = new string(chars).TrimStart(' ');

            // Check that the input equals the output
            Assert.Equal(input, output);
        }
    }

    [Fact]
    public void StreamingDecoderRoundTrip()
    {
        var decoder = new StreamingTokenDecoder(Encoding.UTF8, _model);

        var strings = new[]
        {
            "Hello world",
            "철수",
            "😀 😃 😄 😁 😆철수😅 😂 😊 😇 🙂 ",
        };

        foreach (var input in strings)
        {
            decoder.Reset();

            // Convert into llama tokens
            var tokens = _model.NativeHandle.Tokenize(input, false, false, Encoding.UTF8);

            // Add tokens to decoder
            foreach (var token in tokens)
                decoder.Add(token);

            // llama.cpp adds a space to the start of strings, remove that
            var output = decoder.Read().TrimStart(' ');

            // Check that the input equals the output
            Assert.Equal(input, output);
        }
    }
}