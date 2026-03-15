using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using Microsoft.Extensions.Logging.Abstractions;

namespace LLama.Unittest;

public sealed class MtmdContextGuardTests : IDisposable
{
    private const string EnableEnvVar = "LLAMASHARP_RUN_MTMD_TESTS";
    private const string SkipReason = "MTMD tests are opt-in. Set LLAMASHARP_RUN_MTMD_TESTS=1 to run them.";

    private readonly bool _isEnabled;
    private readonly LLamaWeights? _weights;
    private readonly ModelParams? _modelParams;

    public MtmdContextGuardTests()
    {
        _isEnabled = string.Equals(Environment.GetEnvironmentVariable(EnableEnvVar), "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Environment.GetEnvironmentVariable(EnableEnvVar), "true", StringComparison.OrdinalIgnoreCase);

        if (!_isEnabled)
            return;

        NativeLogConfig.llama_log_set(static (_, _) => { });

        _modelParams = new ModelParams(Constants.GenerativeModelPath2)
        {
            ContextSize = 128,
            BatchSize = 8,
            UBatchSize = 8,
            VocabOnly = false,
            GpuLayerCount = Constants.CIGpuLayerCount,
        };
        _weights = LLamaWeights.LoadFromFile(_modelParams);
    }

    public void Dispose()
    {
        if (!_isEnabled)
            return;

        _weights.Dispose();
        NativeLogConfig.llama_log_set((NativeLogConfig.LLamaLogCallback?)null);
    }

    [SkippableFact]
    public void InteractiveExecutor_LoadMtmdPromptSegments_PreservesOrderedPromptAndMediaBoundary()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        using var chunks = new SafeMtmdInputChunks(NativeApi.mtmd_test_create_input_chunks());
        var executor = new TrackingInteractiveExecutor(context);
        var fillerToken = (LLamaToken)99;

        var (imageTokenCount, imagePositionCount) = GetSyntheticImageChunkMetadata(chunks);
        executor.LoadPendingMtmd(chunks, fillerToken);

        Assert.Equal(
            new[] { (LLamaToken)1, 2, 3, 4, 5 }.Concat(Enumerable.Repeat(fillerToken, imageTokenCount)),
            executor.EmbedInputs);
        Assert.Equal(5 + imagePositionCount, executor.PendingPositions);

        executor.QueuePromptText();

        Assert.Equal(new[] { (LLamaToken)1, 2, 3, 4, 5 }, executor.PendingEmbeds);
        Assert.True(executor.PendingMediaSegment);

        executor.ClearPendingEmbeds();

        Assert.Equal(imagePositionCount, executor.PendingPositions);
    }

    [SkippableFact]
    public void InteractiveExecutor_MtmdOverflowThrowsWithoutContextShift()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        using var chunks = new SafeMtmdInputChunks(NativeApi.mtmd_test_create_input_chunks());
        var executor = new TrackingInteractiveExecutor(context);

        executor.LoadPendingMtmd(chunks, fillerToken: 99, pastTokensCount: (int)context.ContextSize - 1, pendingEmbeds: [(LLamaToken)1]);

        Assert.True(executor.PendingPositions > 1);

        var ex = Assert.Throws<RuntimeError>(() => executor.CheckContextGuard(new InferenceParams()));

        Assert.Contains("Context shifting is not supported", ex.Message);
    }

    [SkippableFact]
    public void InstructExecutor_MtmdOverflowThrowsWithoutContextShift()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        using var chunks = new SafeMtmdInputChunks(NativeApi.mtmd_test_create_input_chunks());
        var executor = new TrackingInstructExecutor(context);

        executor.LoadPendingMtmd(chunks, fillerToken: 99, pastTokensCount: (int)context.ContextSize - 1, pendingEmbeds: [(LLamaToken)1]);

        Assert.True(executor.PendingPositions > 1);

        var ex = Assert.Throws<RuntimeError>(() => executor.CheckContextGuard());

        Assert.Contains("Context shifting is not supported", ex.Message);
    }

    [SkippableFact]
    public async Task InteractiveExecutor_MultimodalSessionAndStateApisAreRejected()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        var executor = new TrackingInteractiveExecutor(context);
        var statePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        Assert.Throws<NotSupportedException>(() => executor.WithSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.SaveSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.GetStateData());
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.SaveState(statePath));
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.LoadState(statePath));
    }

    [SkippableFact]
    public async Task InstructExecutor_MultimodalSessionAndStateApisAreRejected()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        var executor = new TrackingInstructExecutor(context);
        var statePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        Assert.Throws<NotSupportedException>(() => executor.WithSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.SaveSessionFile(statePath));
        Assert.Throws<NotSupportedException>(() => executor.GetStateData());
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.SaveState(statePath));
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.LoadState(statePath));
    }

    [SkippableFact]
    public void ChatSession_MultimodalSessionPersistenceApisAreRejected()
    {
        Skip.IfNot(_isEnabled, SkipReason);

        using var context = _weights!.CreateContext(_modelParams!, NullLogger.Instance);
        var executor = new TrackingInteractiveExecutor(context);
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

    private static (int ImageTokenCount, int ImagePositionCount) GetSyntheticImageChunkMetadata(SafeMtmdInputChunks chunks)
    {
        foreach (var chunk in chunks.Enumerate())
        {
            using var scopedChunk = chunk;
            if (scopedChunk.Type == SafeMtmdInputChunk.SafeMtmdInputChunkType.Image)
                return (checked((int)scopedChunk.NTokens), checked((int)scopedChunk.NPos));
        }

        throw new InvalidOperationException("Synthetic MTMD test chunks do not contain an image segment.");
    }

    private static MtmdWeights CreateFakeMtmdWeights()
        => (MtmdWeights)RuntimeHelpers.GetUninitializedObject(typeof(MtmdWeights));

    private static class TrackingMultimodalExecutorBase
    {
        private static readonly FieldInfo ClipModelField = typeof(StatefulExecutorBase)
            .GetField("<ClipModel>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Unable to find ClipModel backing field.");

        public static void MarkAsMultimodal(StatefulExecutorBase executor)
        {
            ClipModelField.SetValue(executor, CreateFakeMtmdWeights());
        }
    }

    private sealed class TrackingInteractiveExecutor : InteractiveExecutor
    {
        public TrackingInteractiveExecutor(LLamaContext context)
            : base(context, NullLogger.Instance)
        {
            TrackingMultimodalExecutorBase.MarkAsMultimodal(this);
        }

        public int PendingPositions => GetPendingInputPositionCount();

        public IReadOnlyList<LLamaToken> EmbedInputs => _embed_inps;

        public IReadOnlyList<LLamaToken> PendingEmbeds => _embeds;

        public bool PendingMediaSegment => HasPendingMtmdMediaSegment();

        public void LoadPendingMtmd(SafeMtmdInputChunks chunks, LLamaToken fillerToken, int pastTokensCount = 0, IReadOnlyList<LLamaToken>? pendingEmbeds = null)
        {
            LoadMtmdPromptSegments(chunks, fillerToken, replaceExisting: true);
            _pastTokensCount = pastTokensCount;
            _embeds = pendingEmbeds?.ToList() ?? [];
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
        public TrackingInstructExecutor(LLamaContext context)
            : base(context, logger: NullLogger.Instance)
        {
            TrackingMultimodalExecutorBase.MarkAsMultimodal(this);
        }

        public int PendingPositions => GetPendingInputPositionCount();

        public void LoadPendingMtmd(SafeMtmdInputChunks chunks, LLamaToken fillerToken, int pastTokensCount = 0, IReadOnlyList<LLamaToken>? pendingEmbeds = null)
        {
            LoadMtmdPromptSegments(chunks, fillerToken, replaceExisting: true);
            _pastTokensCount = pastTokensCount;
            _embeds = pendingEmbeds?.ToList() ?? [];
        }

        public void CheckContextGuard()
            => EnsurePendingInputFitsContext(_embed_inps.Count);
    }
}
