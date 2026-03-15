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
    public const string EnableEnvVar = "LLAMASHARP_RUN_NOCI_MTMD";
    public const string SkipReason = "MTMD NoCI tests are opt-in. Set LLAMASHARP_RUN_NOCI_MTMD=1 to run them.";

    public MtmdNoCiFixture()
    {
        IsEnabled = string.Equals(Environment.GetEnvironmentVariable(EnableEnvVar), "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Environment.GetEnvironmentVariable(EnableEnvVar), "true", StringComparison.OrdinalIgnoreCase);

        if (!IsEnabled)
            return;

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

    public bool IsEnabled { get; }

    public ModelParams ModelParams { get; } = null!;

    public LLamaWeights Weights { get; } = null!;

    public MtmdContextParams MtmdParams { get; } = null!;

    public MtmdWeights Mtmd { get; } = null!;

    public string MediaMarker { get; } = string.Empty;

    public LLamaContext CreateContext()
        => IsEnabled
            ? Weights.CreateContext(ModelParams, NullLogger.Instance)
            : throw new InvalidOperationException(SkipReason);

    public void Dispose()
    {
        if (!IsEnabled)
            return;

        Mtmd.Dispose();
        Weights.Dispose();
        NativeLogConfig.llama_log_set((NativeLogConfig.LLamaLogCallback?)null);
    }
}
