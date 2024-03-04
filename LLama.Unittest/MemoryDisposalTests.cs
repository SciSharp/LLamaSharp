using LLama.Common;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Unittest;

public class MemoryDisposalTests
{
    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ggml_backend_cuda_get_device_count();

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ggml_backend_cuda_get_device_description(int device, StringBuilder description, ulong description_size);

    [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ggml_backend_cuda_get_device_memory(int device, out ulong free, out ulong total);

    private static ulong GetGPUFreeMemory()
    {
        ulong freemem = 0;
        int dc = ggml_backend_cuda_get_device_count();
        if (dc > 0)
        {
            ulong totalmem = 0;
            StringBuilder sb = new StringBuilder(1024 * 1024);
            ggml_backend_cuda_get_device_description(0, sb, 1024 * 1024);
            ggml_backend_cuda_get_device_memory(0, out freemem, out totalmem);
        }
        return freemem;
    }

    [Fact(Skip = "CI does not use CUDA")]
    public void ModelDisposal()
    {
        ulong memS = GetGPUFreeMemory();

        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
        };
        var model = LLamaWeights.LoadFromFile(@params);

        Assert.False(model.NativeHandle.IsClosed);
        model.Dispose();
        Assert.True(model.NativeHandle.IsClosed);

        if (memS > 0)
        {
            ulong memE = GetGPUFreeMemory();
            Assert.True(memE >= memS);
        }
    }

    [Fact(Skip = "CI does not use CUDA")]
    public void ContextDisposal()
    {
        ulong memS = GetGPUFreeMemory();

        var @params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
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

        if (memS > 0)
        {
            ulong memE = GetGPUFreeMemory();
            Assert.True(memE >= memS);
        }
    }
}