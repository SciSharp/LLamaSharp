#nullable enable

using System.Runtime.InteropServices;
using LLama.Native;
using Moq;

namespace LLama.Unittest;

/// <summary>
/// Exercise the v0.9.0 ABI against the loaded native libraries without model downloads.
/// </summary>
public class NativeCompatibilityTests
{
    /// <summary>
    /// Verify that a matching native backend is loaded.
    /// </summary>
    [Fact]
    public void NativeVersionMatchesBindings()
    {
        Assert.StartsWith("0.9.0", Marshal.PtrToStringUTF8(NativeApi.llama_version()));
    }

    /// <summary>
    /// Check values on both sides of the inserted model parameter field.
    /// </summary>
    [Fact]
    public void ModelDefaultsMatchNative()
    {
        var parameters = LLamaModelParams.Default();
        Assert.Equal(LLamaLoadMode.Auto, parameters.load_mode);
        Assert.Equal(LLamaLazyMode.Auto, parameters.lazy_mode);
        Assert.Equal(0, parameters.main_gpu);
        Assert.True(parameters.use_extra_bufts);
        Assert.False(parameters.load_mtp);
    }

    /// <summary>
    /// Check fields following both insertions in the context parameter layout.
    /// </summary>
    [Fact]
    public void ContextDefaultsMatchNative()
    {
        var parameters = LLamaContextParams.Default();
        Assert.Equal(1u, parameters.n_outputs_max_per_seq);
        Assert.True(parameters.n_threads > 0);
        Assert.Equal(GGMLType.GGML_TYPE_F16, parameters.type_k);
        Assert.Equal((nuint)0, parameters.moe_cache_size);
        Assert.Equal(IntPtr.Zero, parameters.abort_callback);
        Assert.False(parameters.embeddings);
        Assert.True(parameters.offload_kqv);
        Assert.False(parameters.kv_unified);
        Assert.Equal(IntPtr.Zero, parameters.ctx_other);
        Assert.Equal(GGMLType.GGML_TYPE_COUNT, LLamaModelQuantizeParams.Default().output_tensor_type);
    }

    /// <summary>
    /// Exercise return-by-value marshalling of multimodal parameters and bitmap options.
    /// </summary>
    [Fact]
    public void MultimodalDefaultsMatchNative()
    {
        var parameters = MtmdContextParams.Default();
        Assert.True(parameters.UseGpu);
        Assert.Equal(IntPtr.Zero, parameters.Device);
        Assert.True(parameters.PrintTimings);
        Assert.Equal(4, parameters.NThreads);
        Assert.Equal(-1, parameters.ImageMinTokens);
        Assert.Equal(1024, parameters.BatchMaxTokens);
        Assert.Equal("<__media__>", parameters.MediaMarker);

        using var scope = parameters.ToNativeScope();
        Assert.Equal(parameters.Device, scope.Value.device);
        var options = NativeApi.mtmd_helper_init_opt_default();
        Assert.Equal(4f, options.video_params.fps_target);
        Assert.Equal(5000, options.video_params.timestamp_interval_ms);
        Assert.False(options.video_params.no_soundtrack);
    }

    /// <summary>
    /// Check that vocabulary size and penalty arguments reach the new native signature.
    /// </summary>
    [Theory]
    [InlineData(4f, 1.25f)]
    [InlineData(-4f, -8.75f)]
    public void PenaltiesApplyAndReset(float original, float expected)
    {
        using var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());
        chain.AddPenalties(3, 4, 2f, 0.5f, 0.25f);
        chain.Accept((LLamaToken)1);
        var candidates = LLamaTokenDataArray.Create(new[] { 2f, original, 3f });
        using var pin = LLamaTokenDataArrayNative.Create(candidates, out var native);
        chain.Apply(ref native);
        Assert.Equal(expected, native.Data[1].Logit);
        Assert.Equal(2f, native.Data[0].Logit);

        chain.Reset();
        native.Data[1].Logit = original;
        chain.Apply(ref native);
        Assert.Equal(original, native.Data[1].Logit);
    }

    /// <summary>
    /// Exercise the managed callback table through native apply, clone, reset and disposal.
    /// </summary>
    [Fact]
    public void CustomSamplerCallbacksAndCloneWork()
    {
        var sampler = new Mock<ICustomSampler>();
        var clone = new Mock<ICustomSampler>();
        sampler.SetupGet(value => value.Name).Returns("compatibility");
        clone.SetupGet(value => value.Name).Returns("compatibility-clone");
        sampler.Setup(value => value.Clone()).Returns(clone.Object);

        using (var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default()))
        {
            chain.AddCustom(sampler.Object);
            chain.AddClone(chain, 0);
            Assert.Equal("compatibility", chain.GetName(0));
            Assert.Equal("compatibility-clone", chain.GetName(1));
            var candidates = LLamaTokenDataArray.Create(new[] { 1f, 2f });
            using var pin = LLamaTokenDataArrayNative.Create(candidates, out var native);
            chain.Apply(ref native);
            chain.Accept((LLamaToken)1);
            chain.Reset();
        }

        sampler.Verify(value => value.Clone(), Times.Once);
        foreach (var mock in new[] { sampler, clone })
        {
            mock.Verify(value => value.Apply(ref It.Ref<LLamaTokenDataArrayNative>.IsAny), Times.Once);
            mock.Verify(value => value.Accept((LLamaToken)1), Times.Once);
            mock.Verify(value => value.Reset(), Times.Once);
            mock.Verify(value => value.Dispose(), Times.Once);
        }
    }
}
