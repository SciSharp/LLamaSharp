using System;
using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LLama.Unittest;

[CollectionDefinition("MtmdNoCi", DisableParallelization = true)]
public sealed class MtmdNoCiCollection : ICollectionFixture<MtmdNoCiFixture>
{
}

public sealed class MtmdNoCiFixture : IDisposable
{
    public MtmdNoCiFixture()
    {
        NativeLogConfig.llama_log_set(static (_, _) => { });

        ModelParams = new ModelParams(Constants.MtmdModelPath)
        {
            ContextSize = 1024 * 32,
            GpuLayerCount = Constants.CIGpuLayerCount,
        };

        Weights = LLamaWeights.LoadFromFile(ModelParams);

        MtmdParams = MtmdContextParams.Default();
        MtmdParams.NThreads = Math.Max(1, Constants.CIGpuLayerCount);
        MtmdParams.UseGpu = false;

        Mtmd = MtmdWeights.LoadFromFile(Constants.MtmdMmpPath, Weights, MtmdParams);
        MediaMarker = MtmdParams.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";
    }

    public ModelParams ModelParams { get; } = null!;

    public LLamaWeights Weights { get; } = null!;

    public MtmdContextParams MtmdParams { get; } = null!;

    public MtmdWeights Mtmd { get; } = null!;

    public string MediaMarker { get; } = string.Empty;

    public LLamaContext CreateContext()
        => Weights.CreateContext(ModelParams, NullLogger.Instance);

    public SafeMtmdInputChunks TokenizePromptWithMedia(string prompt)
        => TokenizePromptWithMedia(prompt, Constants.MtmdImage);

    public SafeMtmdInputChunks TokenizePromptWithMedia(string prompt, string mediaPath)
    {
        using var embed = Mtmd.LoadMediaStandalone(mediaPath);
        var embeds = new[] { embed };
        var status = Mtmd.Tokenize(prompt, addSpecial: true, parseSpecial: true, embeds, out var chunks);
        Assert.Equal(0, status);
        Assert.NotNull(chunks);
        return chunks!;
    }

    public LLamaToken GetFillerToken(LLamaContext context)
    {
        var markerTokens = context.Tokenize(MediaMarker, false, true);
        return markerTokens.Length > 0
            ? markerTokens[^1]
            : context.Vocab.EOS ?? default;
    }

    public void Dispose()
    {
        Mtmd.Dispose();
        Weights.Dispose();
        NativeLogConfig.llama_log_set((NativeLogConfig.LLamaLogCallback?)null);
    }
}
