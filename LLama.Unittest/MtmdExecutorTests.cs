using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LLama.Unittest;

[Collection("MtmdNoCi")]
[Trait("Category", "NoCI")]
public class MtmdExecutorTests
{
    private readonly MtmdNoCiFixture _fixture;

    public MtmdExecutorTests(MtmdNoCiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task InteractiveExecutor_EvaluateChunks_DoesNotRetokenize()
    {
        using var context = _fixture.CreateContext();
        var executor = new InteractiveExecutor(context, _fixture.Mtmd, NullLogger.Instance);
        var marker = _fixture.MediaMarker;
        var prompt = $"Before {marker} after.";

        executor.Embeds.Add(_fixture.Mtmd.LoadMedia(Constants.MtmdImage));

        await foreach (var _ in executor.InferAsync(prompt, new InferenceParams { MaxTokens = 0 }))
        {
            Assert.Fail("Prefill should not emit generated text");
        }

        var diagnostics = executor.GetDiagnostics();
        Assert.True(diagnostics.ConsumedCount > 0);
        Assert.Equal(0, diagnostics.PendingEmbedCount);
    }

    [Fact]
    public async Task InstructExecutor_MtmdPromptAdvancesPastTokensOnce()
    {
        using var context = _fixture.CreateContext();
        var executor = new InstructExecutor(context, _fixture.Mtmd, logger: NullLogger.Instance);
        executor.Embeds.Add(_fixture.Mtmd.LoadMedia(Constants.MtmdImage));

        var prompt = $"{_fixture.MediaMarker} Provide details.";

        await foreach (var _ in executor.InferAsync(prompt, new InferenceParams { MaxTokens = 0 }))
        {
        }

        var diagnostics = executor.GetDiagnostics();
        Assert.True(diagnostics.ConsumedCount > 0);
        Assert.Equal(0, diagnostics.PendingEmbedCount);
    }

    [Fact]
    public async Task InteractiveExecutor_PreprocessMtmd_PreservesInterleavedPromptOrder()
    {
        using var context = _fixture.CreateContext();
        var executor = new InspectableInteractiveExecutor(context, _fixture.Mtmd);
        var marker = _fixture.MediaMarker;
        var prompt = $"Before {marker} after.";
        using var expectedChunks = _fixture.TokenizePromptWithMedia(prompt);
        var fillerToken = _fixture.GetFillerToken(context);
        var expectedPromptTokens = BuildLogicalPromptTokens(expectedChunks, fillerToken);
        var expectedLeadingTextTokens = GetLeadingTextTokens(expectedChunks);

        executor.Embeds.Add(_fixture.Mtmd.LoadMedia(Constants.MtmdImage));
        await executor.PreparePromptAsync(prompt, addBos: true, replaceExisting: true);

        Assert.Equal(expectedPromptTokens, executor.EmbedInputs);
        Assert.Equal((int)expectedChunks.CountPositions(), executor.PendingPositions);

        executor.QueuePromptText();

        Assert.Equal(expectedLeadingTextTokens, executor.PendingEmbeds);
        Assert.True(executor.PendingMediaSegment);
    }

    private static IReadOnlyList<LLamaToken> BuildLogicalPromptTokens(SafeMtmdInputChunks chunks, LLamaToken fillerToken)
    {
        var tokens = new List<LLamaToken>();

        foreach (var chunk in chunks.Enumerate())
        {
            using var scopedChunk = chunk;
            if (scopedChunk.Type == SafeMtmdInputChunk.SafeMtmdInputChunkType.Text)
            {
                tokens.AddRange(scopedChunk.GetTextTokensSpan().ToArray().Select(static token => (LLamaToken)token));
            }
            else
            {
                tokens.AddRange(Enumerable.Repeat(fillerToken, checked((int)scopedChunk.NTokens)));
            }
        }

        return tokens;
    }

    private static IReadOnlyList<LLamaToken> GetLeadingTextTokens(SafeMtmdInputChunks chunks)
    {
        foreach (var chunk in chunks.Enumerate())
        {
            using var scopedChunk = chunk;
            if (scopedChunk.Type == SafeMtmdInputChunk.SafeMtmdInputChunkType.Text)
                return scopedChunk.GetTextTokensSpan().ToArray().Select(static token => (LLamaToken)token).ToArray();

            break;
        }

        throw new InvalidOperationException("Expected the tokenized multimodal prompt to begin with a text chunk.");
    }

    private sealed class InspectableInteractiveExecutor : InteractiveExecutor
    {
        public InspectableInteractiveExecutor(LLamaContext context, MtmdWeights clipModel)
            : base(context, clipModel, NullLogger.Instance)
        {
        }

        public IReadOnlyList<LLamaToken> EmbedInputs => _embed_inps;

        public IReadOnlyList<LLamaToken> PendingEmbeds => _embeds;

        public int PendingPositions => GetPendingInputPositionCount();

        public bool PendingMediaSegment => HasPendingMtmdMediaSegment();

        public Task PreparePromptAsync(string prompt, bool addBos, bool replaceExisting)
            => PreprocessMtmd(prompt, new InferStateArgs(), addBos, replaceExisting);

        public void QueuePromptText()
            => QueueNextMtmdPromptInput();
    }
}
