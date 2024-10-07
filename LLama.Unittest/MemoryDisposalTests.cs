using LLama.Common;

namespace LLama.Unittest;

public class MemoryDisposalTests
{
    [Fact]
    public void ModelDisposal()
    {
        var @params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 2048,
            GpuLayerCount = 0,
        };
        var model = LLamaWeights.LoadFromFile(@params);

        Assert.False(model.NativeHandle.IsClosed);
        model.Dispose();
        Assert.True(model.NativeHandle.IsClosed);
    }

    [Fact]
    public void ContextDisposal()
    {
        var @params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 128,
            GpuLayerCount = 0,            
        };
        var model = LLamaWeights.LoadFromFile(@params);

        var ctx = model.CreateContext(@params);

        // Disposing the model handle does **not** free the memory, because there's stilla context
        Assert.False(model.NativeHandle.IsClosed);
        model.Dispose();
        Assert.False(model.NativeHandle.IsClosed);

        // Disposing the context frees context and model weights
        ctx.Dispose();
        Assert.True(model.NativeHandle.IsClosed);
        Assert.True(ctx.NativeHandle.IsClosed);
    }
}