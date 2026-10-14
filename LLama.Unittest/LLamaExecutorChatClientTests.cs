using System.Runtime.CompilerServices;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Microsoft.Extensions.AI;

namespace LLama.Unittest;

public sealed class LLamaExecutorChatClientTests
{
    [Fact]
    public async Task RawRepresentationSettingsAreKept()
    {
        var executor = new CapturingExecutor();

        await executor.AsChatClient().GetResponseAsync(
            "hello",
            new ChatOptions
            {
                RawRepresentationFactory = _ => new InferenceParams
                {
                    MaxTokens = 1024,
                    SamplingPipeline = new DefaultSamplingPipeline { Temperature = 0.1f, TopP = 0.5f, TopK = 5, Seed = 42 },
                }
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var pipeline = Assert.IsType<DefaultSamplingPipeline>(executor.InferenceParams?.SamplingPipeline);
        Assert.Equal(1024, executor.InferenceParams!.MaxTokens);
        Assert.Equal(0.1f, pipeline.Temperature);
        Assert.Equal(0.5f, pipeline.TopP);
        Assert.Equal(5, pipeline.TopK);
        Assert.Equal(42u, pipeline.Seed);
    }

    [Fact]
    public async Task RawRepresentationUnlimitedMaxTokensIsKept()
    {
        var executor = new CapturingExecutor();

        await executor.AsChatClient().GetResponseAsync(
            "hello",
            new ChatOptions
            {
                RawRepresentationFactory = _ => new InferenceParams { MaxTokens = -1 },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.Equal(-1, executor.InferenceParams!.MaxTokens);
    }

    [Fact]
    public async Task ChatOptionsOverrideRawRepresentationSettings()
    {
        var executor = new CapturingExecutor();

        await executor.AsChatClient().GetResponseAsync(
            "hello",
            new ChatOptions
            {
                MaxOutputTokens = 64,
                Temperature = 0.9f,
                TopP = 0.8f,
                TopK = 20,
                Seed = 7,
                RawRepresentationFactory = _ => new InferenceParams
                {
                    MaxTokens = 1024,
                    SamplingPipeline = new DefaultSamplingPipeline { Temperature = 0.1f, TopP = 0.5f, TopK = 5, Seed = 42 },
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var pipeline = Assert.IsType<DefaultSamplingPipeline>(executor.InferenceParams?.SamplingPipeline);
        Assert.Equal(64, executor.InferenceParams!.MaxTokens);
        Assert.Equal(0.9f, pipeline.Temperature);
        Assert.Equal(0.8f, pipeline.TopP);
        Assert.Equal(20, pipeline.TopK);
        Assert.Equal(7u, pipeline.Seed);
    }

    [Fact]
    public async Task RawRepresentationSamplingPipelineIsKept()
    {
        var executor = new CapturingExecutor();
        var greedy = new GreedySamplingPipeline();

        await executor.AsChatClient().GetResponseAsync(
            "hello",
            new ChatOptions
            {
                RawRepresentationFactory = _ => new InferenceParams { SamplingPipeline = greedy },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.Same(greedy, executor.InferenceParams?.SamplingPipeline);
    }

    [Fact]
    public async Task MaxTokensDefaultsTo256WithoutALimit()
    {
        var executor = new CapturingExecutor();

        await executor.AsChatClient().GetResponseAsync("hello", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(256, executor.InferenceParams!.MaxTokens);
    }

    /// <summary>
    /// Records the inference parameters it is given, without a model.
    /// </summary>
    private sealed class CapturingExecutor : ILLamaExecutor
    {
        public IInferenceParams? InferenceParams { get; private set; }

        public LLamaContext Context => throw new NotSupportedException();
        public bool IsMultiModal => false;
        public MtmdWeights? ClipModel => null;
        public List<SafeMtmdEmbed> Embeds { get; } = [];

        public async IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            InferenceParams = inferenceParams;
            await Task.Yield();
            yield return "hi";
        }
    }
}
