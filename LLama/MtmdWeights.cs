
using System;
using System.Threading;
using System.Threading.Tasks;
using LLama.Native;

namespace LLama;

/// <summary>
/// Lightweight wrapper around the MTMD native context and its helpers.
/// </summary>
public sealed class MtmdWeights : IDisposable
{
    public SafeMtmdModelHandle NativeHandle { get; }

    private MtmdWeights(SafeMtmdModelHandle handle)
    {
        NativeHandle = handle ?? throw new ArgumentNullException(nameof(handle));
    }

    public static MtmdWeights LoadFromFile(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams)
    {
        if (mmProject == null) throw new ArgumentNullException(nameof(mmProject));
        if (textModel == null) throw new ArgumentNullException(nameof(textModel));
        if (mtmdCtxParams == null) throw new ArgumentNullException(nameof(mtmdCtxParams));

        var handle = SafeMtmdModelHandle.LoadFromFile(mmProject, textModel, mtmdCtxParams);
        return new MtmdWeights(handle);
    }

    public static Task<MtmdWeights> LoadFromFileAsync(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams, CancellationToken token = default)
    {
        return Task.Run(() => LoadFromFile(mmProject, textModel, mtmdCtxParams), token);
    }

    /// <summary>
    /// Load media from disk and keep it pending for the next tokenize call.
    /// </summary>
    public SafeMtmdEmbed LoadMedia(string path) => NativeHandle.LoadMediaFromFile(path);

    /// <summary>
    /// Load media from an in-memory buffer and keep it pending for the next tokenize call.
    /// </summary>
    public SafeMtmdEmbed LoadMedia(ReadOnlySpan<byte> data) => NativeHandle.LoadMediaFromBuffer(data);

    /// <summary>
    /// Clear any pending media buffers before or after tokenization.
    /// </summary>
    public void ClearMedia() => NativeHandle.ClearMedia();

    /// <summary>
    /// Tokenize text (with optional special tokens) against the pending media buffers.
    /// </summary>
    public int Tokenize(string text, bool addSpecial, bool parseSpecial, out SafeMtmdInputChunks? chunks)
        => NativeHandle.Tokenize(text, addSpecial, parseSpecial, out chunks);

    /// <summary>
    /// Tokenize text (with optional special tokens) against explicit media embeddings.
    /// The caller retains ownership of <paramref name="embeds"/>.
    /// </summary>
    public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, out SafeMtmdInputChunks? chunks)
        => NativeHandle.Tokenize(text, addSpecial, parseSpecial, embeds, out chunks);

    /// <summary>
    /// Evaluate a chunk batch using the helper that performs mtmd encode + llama decode.
    /// </summary>
    public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
        => NativeHandle.EvaluateChunks(chunks, llamaContext, ref nPast, seqId, nBatch, logitsLast);

    public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
        => NativeHandle.EvaluateChunk(chunkPtr, llamaContext, ref nPast, seqId, nBatch, logitsLast);

    public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, ref int nPast, int seqId, int nBatch)
        => NativeHandle.DecodeImageChunk(chunkPtr, llamaContext, encodedEmbeddings, ref nPast, seqId, nBatch);

    public ulong CountTokens(SafeMtmdInputChunks chunks) => NativeHandle.CountTokens(chunks);

    public long CountPositions(SafeMtmdInputChunks chunks) => NativeHandle.CountPositions(chunks);

    public bool SupportsVision => NativeHandle.SupportVision();
    public bool SupportsAudio => NativeHandle.SupportAudio();
    public bool UsesNonCausalAttention => NativeHandle.DecodeUseNonCausal();
    public bool UsesMRope => NativeHandle.DecodeUseMRope();
    public int AudioBitrate => NativeHandle.GetAudioBitrate();

    public void Dispose() => NativeHandle.Dispose();
}
