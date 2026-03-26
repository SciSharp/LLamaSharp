using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using Microsoft.Extensions.Logging.Abstractions;

namespace LLama.Unittest;

[Collection("MtmdNoCi")]
[Trait("Category", "NoCI")]
public sealed class MtmdContextGuardTests
{
    private readonly MtmdNoCiFixture _fixture;

    public MtmdContextGuardTests(MtmdNoCiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void InteractiveExecutor_LoadMtmdPromptSegments_PreservesOrderedPromptAndMediaBoundary()
    {
        using var context = _fixture.CreateContext();
        using var chunks = _fixture.TokenizePromptWithMedia($"Before {_fixture.MediaMarker}");
        var executor = new TrackingInteractiveExecutor(context, _fixture.Mtmd);
        var fillerToken = _fixture.GetFillerToken(context);

        var (imageTokenCount, imagePositionCount) = GetImageChunkMetadata(chunks);
        executor.LoadPendingMtmd(chunks, fillerToken);

        Assert.Contains(fillerToken, executor.EmbedInputs);
        Assert.Equal((int)chunks.CountPositions(), executor.PendingPositions);
        Assert.Equal((int)chunks.CountTokens(), executor.PendingKvTokens);

        executor.QueuePromptText();

        Assert.NotEmpty(executor.PendingEmbeds);
        var queuedTextCount = executor.PendingEmbeds.Count;
        Assert.True(executor.PendingMediaSegment);

        executor.ClearPendingEmbeds();

        Assert.Equal((int)chunks.CountPositions() - queuedTextCount, executor.PendingPositions);
        Assert.Equal((int)chunks.CountTokens() - queuedTextCount, executor.PendingKvTokens);
    }

    [Fact]
    public void InteractiveExecutor_MtmdOverflowThrowsWithoutContextShift()
    {
        using var context = _fixture.CreateContext();
        using var chunks = _fixture.TokenizePromptWithMedia($"{_fixture.MediaMarker} describe");
        var executor = new TrackingInteractiveExecutor(context, _fixture.Mtmd);
        var fillerToken = _fixture.GetFillerToken(context);

        executor.LoadPendingMtmd(chunks, fillerToken, pastTokensCount: (int)context.ContextSize - 1, pendingEmbeds: [(LLamaToken)1]);

        Assert.True(executor.PendingPositions > 1);

        var ex = Assert.Throws<RuntimeError>(() => executor.CheckContextGuard(new InferenceParams()));

        Assert.Contains("Context shifting is not supported", ex.Message);
    }

    [Fact]
    public void InteractiveExecutor_MtmdGuardUsesKvOccupancyInsteadOfPositions()
    {
        using var context = _fixture.CreateContext();
        using var chunks = _fixture.TokenizePromptWithMedia($"Before {_fixture.MediaMarker}");
        var executor = new TrackingInteractiveExecutor(context, _fixture.Mtmd);

        executor.LoadPendingMtmd(chunks, _fixture.GetFillerToken(context));

        var positionCompatiblePast = (int)context.ContextSize - executor.PendingPositions;
        var kvOverflowPast = (int)context.ContextSize - executor.PendingKvTokens + 1;

        Assert.True(positionCompatiblePast + executor.PendingPositions <= context.ContextSize);
        Assert.True(kvOverflowPast + executor.PendingKvTokens > context.ContextSize);

        executor.SetOccupiedCounts(positionCompatiblePast, kvOverflowPast);

        var ex = Assert.Throws<RuntimeError>(() => executor.CheckContextGuard(new InferenceParams()));
        Assert.Contains("Context shifting is not supported", ex.Message);
    }

    [Fact]
    public void InstructExecutor_MtmdOverflowThrowsWithoutContextShift()
    {
        using var context = _fixture.CreateContext();
        using var chunks = _fixture.TokenizePromptWithMedia($"{_fixture.MediaMarker} describe");
        var executor = new TrackingInstructExecutor(context, _fixture.Mtmd);
        var fillerToken = _fixture.GetFillerToken(context);

        executor.LoadPendingMtmd(chunks, fillerToken, pastTokensCount: (int)context.ContextSize - 1, pendingEmbeds: [(LLamaToken)1]);

        Assert.True(executor.PendingPositions > 1);

        var ex = Assert.Throws<RuntimeError>(() => executor.CheckContextGuard());

        Assert.Contains("Context shifting is not supported", ex.Message);
    }

    [Fact]
    public async Task InteractiveExecutor_MultimodalSessionAndStateApisAreRejected()
    {
        using var context = _fixture.CreateContext();
        var executor = new TrackingInteractiveExecutor(context, _fixture.Mtmd);
        var statePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        Assert.Throws<NotSupportedException>(() => executor.WithSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.SaveSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.GetStateData());
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.SaveState(statePath));
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.LoadState(statePath));
    }

    [Fact]
    public async Task InstructExecutor_MultimodalSessionAndStateApisAreRejected()
    {
        using var context = _fixture.CreateContext();
        var executor = new TrackingInstructExecutor(context, _fixture.Mtmd);
        var statePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        Assert.Throws<NotSupportedException>(() => executor.WithSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.SaveSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.GetStateData());
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.SaveState(statePath));
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.LoadState(statePath));
    }

    [Fact]
    public void ChatSession_MultimodalSessionPersistenceApisAreRejected()
    {
        using var context = _fixture.CreateContext();
        var executor = new TrackingInteractiveExecutor(context, _fixture.Mtmd);
        var chatSession = new ChatSession(executor);
        var sessionPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var state = new SessionState(
            contextState: null,
            executorState: new InteractiveExecutor.InteractiveExecutorState(),
            history: new ChatHistory(),
            inputTransformPipeline: [],
            outputTransform: new LLamaTransforms.EmptyTextOutputStreamTransform(),
            historyTransform: new LLamaTransforms.DefaultHistoryTransform());

        Assert.Throws<NotSupportedException>(() => chatSession.GetSessionState());
        Assert.Throws<NotSupportedException>(() => chatSession.SaveSession(sessionPath));
        Assert.Throws<NotSupportedException>(() => chatSession.LoadSession(state));
        Assert.Throws<NotSupportedException>(() => chatSession.LoadSession(sessionPath));
    }

    private static (int ImageTokenCount, int ImagePositionCount) GetImageChunkMetadata(SafeMtmdInputChunks chunks)
    {
        foreach (var chunk in chunks.Enumerate())
        {
            using var scopedChunk = chunk;
            if (scopedChunk.Type == SafeMtmdInputChunk.SafeMtmdInputChunkType.Image)
                return (checked((int)scopedChunk.NTokens), checked((int)scopedChunk.NPos));
        }

        throw new InvalidOperationException("Synthetic MTMD test chunks do not contain an image segment.");
    }

    private sealed class TrackingInteractiveExecutor : InteractiveExecutor
    {
        public TrackingInteractiveExecutor(LLamaContext context, MtmdWeights clipModel)
            : base(context, clipModel, NullLogger.Instance)
        {
        }

        public int PendingPositions => GetPendingInputPositionCount();

        public int PendingKvTokens => GetPendingInputKvTokenCount();

        public IReadOnlyList<LLamaToken> EmbedInputs => _embed_inps;

        public IReadOnlyList<LLamaToken> PendingEmbeds => _embeds;

        public bool PendingMediaSegment => HasPendingMtmdMediaSegment();

        public void LoadPendingMtmd(SafeMtmdInputChunks chunks, LLamaToken fillerToken, int pastTokensCount = 0, IReadOnlyList<LLamaToken>? pendingEmbeds = null)
        {
            LoadMtmdPromptSegments(chunks, fillerToken, replaceExisting: true);
            _pastTokensCount = pastTokensCount;
            _kvTokenCount = pastTokensCount;
            _embeds = pendingEmbeds?.ToList() ?? [];
        }

        public void SetOccupiedCounts(int pastTokensCount, int kvTokenCount)
        {
            _pastTokensCount = pastTokensCount;
            _kvTokenCount = kvTokenCount;
        }

        public void QueuePromptText()
            => QueueNextMtmdPromptInput();

        public void ClearPendingEmbeds()
            => _embeds.Clear();

        public void CheckContextGuard(InferenceParams inferenceParams)
        {
            var tokensToKeep = inferenceParams.TokensKeep;
            if (tokensToKeep < 0 || tokensToKeep > _embed_inps.Count)
            {
                tokensToKeep = _embed_inps.Count;
            }
            else
            {
                tokensToKeep += Convert.ToInt32(Context.Vocab.ShouldAddBOS);
            }

            EnsurePendingInputFitsContext(tokensToKeep);
        }
    }

    private sealed class TrackingInstructExecutor : InstructExecutor
    {
        public TrackingInstructExecutor(LLamaContext context, MtmdWeights clipModel)
            : base(context, clipModel, logger: NullLogger.Instance)
        {
        }

        public int PendingPositions => GetPendingInputPositionCount();

        public int PendingKvTokens => GetPendingInputKvTokenCount();

        public void LoadPendingMtmd(SafeMtmdInputChunks chunks, LLamaToken fillerToken, int pastTokensCount = 0, IReadOnlyList<LLamaToken>? pendingEmbeds = null)
        {
            LoadMtmdPromptSegments(chunks, fillerToken, replaceExisting: true);
            _pastTokensCount = pastTokensCount;
            _kvTokenCount = pastTokensCount;
            _embeds = pendingEmbeds?.ToList() ?? [];
        }

        public void SetOccupiedCounts(int pastTokensCount, int kvTokenCount)
        {
            _pastTokensCount = pastTokensCount;
            _kvTokenCount = kvTokenCount;
        }

        public void CheckContextGuard()
            => EnsurePendingInputFitsContext(_embed_inps.Count);
    }
}
