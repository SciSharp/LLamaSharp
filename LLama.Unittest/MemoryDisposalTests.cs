using LLama.Common;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Unittest;

public class MemoryDisposalTests
{
    #region Test Helpers

    private static ulong GetGPUFreeMemory()
    {
        ulong freemem = 0;
        int dc = 0;

        try
        {
            dc = ggml_backend_cuda_get_device_count();
        }
        catch (Exception){}

        if (dc > 0)
        {
            ulong totalmem = 0;
            ggml_backend_cuda_get_device_memory(0, out freemem, out totalmem);
        }
        return freemem;
    }

    private static List<KeyValuePair<LLamaWeights, LLamaContext>> _models = new List<KeyValuePair<LLamaWeights, LLamaContext>>();

    private async static Task<KeyValuePair<LLamaWeights, LLamaContext>> RunModel(ModelParams pars)
    {
        var model = LLamaWeights.LoadFromFile(pars);
        var ctx = model.CreateContext(pars);        
        var executor = new InstructExecutor(ctx);
        var inferenceParams = new InferenceParams() { Temperature = 0.8f, MaxTokens = 600 };
        await foreach (var text in executor.InferAsync("This is a short very interesting story for the test.", inferenceParams))
        {
            //nothing
        }
        return new KeyValuePair<LLamaWeights, LLamaContext>(model, ctx);
    }

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    private static extern int ggml_backend_cuda_get_device_count();

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ggml_backend_cuda_get_device_memory(int device, out ulong free, out ulong total);

    #endregion Test Helpers

    /// <summary>
    /// This is a test to see if GPU memory is freed properly.
    /// The test needs to be adjusted based on the available GPU memory in order to maximize GPU memory use.
    /// </summary>
    //[Fact]
    [Fact(Skip = "CI does not use CUDA")]    
    public async void GPUMemoryLeak()
    {
        ulong memS = GetGPUFreeMemory();

        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 4096,
            GpuLayerCount = 99,
            UseMemorymap = true,
            SplitMode = Native.GPUSplitMode.None,
        };

        // first run
        for (int i = 0; i < 2; i++)
        {
            var rm = await RunModel(@params);
            _models.Add(rm);
        }

        foreach (var m in _models)
        {
            m.Key.Dispose();
            m.Value.Dispose();
            Assert.True(m.Key.NativeHandle.IsClosed);
            Assert.True(m.Value.NativeHandle.IsClosed);
        }
        _models.Clear();

        // second run
        for (int i = 0; i < 2; i++)
        {
            var rm = await RunModel(@params);
            _models.Add(rm);
        }

        foreach (var m in _models)
        {
            m.Key.Dispose();
            m.Value.Dispose();
            Assert.True(m.Key.NativeHandle.IsClosed);
            Assert.True(m.Value.NativeHandle.IsClosed);
        }
        _models.Clear();

        if (memS > 0)
        {
            ulong memE = GetGPUFreeMemory();
            Assert.True(memE >= memS);
        }
    }

    [Fact]
    public void ModelDisposal()
    {
        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
        };
        var model = LLamaWeights.LoadFromFile(@params);

        Assert.False(model.NativeHandle.IsClosed);
        model.Dispose();
        Assert.True(model.NativeHandle.IsClosed);
    }

    [Fact]
    public void ContextDisposal()
    {
        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
        };
        var model = LLamaWeights.LoadFromFile(@params);

        var ctx = model.CreateContext(@params);

        // Disposing the model handle does **not** free the memory, because there's still a context
        Assert.False(model.NativeHandle.IsClosed);
        model.Dispose();
        Assert.False(model.NativeHandle.IsClosed);

        // Disposing the context frees context and model weights
        ctx.Dispose();
        Assert.True(model.NativeHandle.IsClosed);
        Assert.True(ctx.NativeHandle.IsClosed);
    }
}