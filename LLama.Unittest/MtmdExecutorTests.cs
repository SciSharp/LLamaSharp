using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LLama.Unittest;

[Trait("Category", "NoCI")]
public class MtmdExecutorTests : IDisposable
{
    private readonly LLamaWeights _weights;
    private readonly MtmdContextParams _mtmdParams;
    private readonly MtmdWeights _mtmd;
    private readonly ModelParams _modelParams;

    public MtmdExecutorTests()
    {
        _modelParams = new ModelParams(Constants.MtmdModelPath)
        {
            ContextSize = 1024 * 32,
            GpuLayerCount = Constants.CIGpuLayerCount,
        };

        _weights = LLamaWeights.LoadFromFile(_modelParams);

        _mtmdParams = MtmdContextParams.Default();
        _mtmdParams.NThreads = Math.Max(1, Constants.CIGpuLayerCount);
        _mtmdParams.UseGpu = false;

        _mtmd = MtmdWeights.LoadFromFile(Constants.MtmdMmpPath, _weights, _mtmdParams);
    }

    public void Dispose()
    {
        _mtmd.Dispose();
        _weights.Dispose();
    }

    [Fact]
    public async Task InteractiveExecutor_EvaluateChunks_DoesNotRetokenize()
    {
        using var context = _weights.CreateContext(_modelParams, NullLogger.Instance);
        var executor = new InteractiveExecutor(context, _mtmd, NullLogger.Instance);
        var marker = _mtmdParams.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";
        var prompt = $"{marker}\nDescribe the image succinctly.";

        executor.Embeds.Add(_mtmd.LoadMedia(Constants.MtmdImage));

        await foreach (var _ in executor.InferAsync(prompt, new InferenceParams { MaxTokens = 0 }))
        {
            Assert.True(false, "Prefill should not emit generated text");
        }

        var diagnostics = executor.GetDiagnostics();
        Assert.Equal(diagnostics.EmbedCount, diagnostics.ConsumedCount);
        Assert.Equal(diagnostics.ConsumedCount, diagnostics.PastCount);
        Assert.Equal(0, diagnostics.PendingEmbedCount);
    }

    [Fact]
    public async Task InstructExecutor_MtmdPromptAdvancesPastTokensOnce()
    {
        using var context = _weights.CreateContext(_modelParams, NullLogger.Instance);
        var executor = new InstructExecutor(context, _mtmd, logger: NullLogger.Instance);
        executor.Embeds.Add(_mtmd.LoadMedia(Constants.MtmdImage));

        var prompt = $"{_mtmdParams.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>"} Provide details.";

        await foreach (var _ in executor.InferAsync(prompt, new InferenceParams { MaxTokens = 0 }))
        {
        }

        var diagnostics = executor.GetDiagnostics();
        Assert.Equal(diagnostics.EmbedCount, diagnostics.ConsumedCount);
        Assert.Equal(diagnostics.ConsumedCount, diagnostics.PastCount);
        Assert.Equal(0, diagnostics.PendingEmbedCount);
    }
}
