using System;
using System.Threading;
using System.Threading.Tasks;
using LLama.Exceptions;
using LLama.Native;

namespace LLama;

/// <summary>
/// Lightweight wrapper around the MTMD native context and its helpers.
/// </summary>
public sealed class MtmdWeights
    : IDisposable
{
    private readonly object _syncRoot = new();

    /// <summary>
    /// The native handle, which is used in the native APIs
    /// </summary>
    /// <remarks>Be careful how you use this!</remarks>
    public SafeMtmdModelHandle NativeHandle { get; }

    private MtmdWeights(SafeMtmdModelHandle handle)
    {
        NativeHandle = handle ?? throw new ArgumentNullException(nameof(handle));
    }

    /// <summary>
    /// Load weights into memory
    /// </summary>
    /// <param name="mmProject">Path to the mmproj file</param>
    /// <param name="textModel">The text model</param>
    /// <param name="mtmdCtxParams">Parameters for MTMD context creation</param>
    /// <returns></returns>
    public static MtmdWeights LoadFromFile(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams)
    {
        return new MtmdWeights(SafeMtmdModelHandle.LoadFromFile(
            mmProject ?? throw new ArgumentNullException(nameof(mmProject)),
            textModel ?? throw new ArgumentNullException(nameof(textModel)),
            mtmdCtxParams ?? throw new ArgumentNullException(nameof(mtmdCtxParams))
        ));
    }

    /// <summary>
    /// Load weights into memory
    /// </summary>
    /// <param name="mmProject">Path to the mmproj file</param>
    /// <param name="textModel">The text model</param>
    /// <param name="mtmdCtxParams">Parameters for MTMD context creation</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<MtmdWeights> LoadFromFileAsync(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams, CancellationToken token = default)
    {
        if (mmProject == null)
            throw new ArgumentNullException(nameof(mmProject));
        if (textModel == null)
            throw new ArgumentNullException(nameof(textModel));
        if (mtmdCtxParams == null)
            throw new ArgumentNullException(nameof(mtmdCtxParams));

        var model = await Task.Run(() =>
        {
            try
            {
                // Load the model
                return LoadFromFile(mmProject, textModel, mtmdCtxParams);
            }
            catch (LoadWeightsFailedException)
            {
                // Convert a LoadWeightsFailedException into a cancellation exception if possible.
                token.ThrowIfCancellationRequested();

                // Ok the weights failed to load for some reason other than cancellation.
                throw;
            }
        }, token);

        return model;
    }

    /// <summary>
    /// Load media from disk and keep it pending for the next tokenize call.
    /// </summary>
    public SafeMtmdEmbed LoadMedia(string path)
    {
        lock (_syncRoot)
            return NativeHandle.LoadMediaFromFile(path);
    }

    /// <summary>
    /// Load media from an in-memory buffer and keep it pending for the next tokenize call.
    /// </summary>
    public SafeMtmdEmbed LoadMedia(ReadOnlySpan<byte> data)
    {
        lock (_syncRoot)
            return NativeHandle.LoadMediaFromBuffer(data);
    }

    /// <summary>
    /// Load media from disk as a standalone embedding without touching the shared pending-media queue.
    /// </summary>
    public SafeMtmdEmbed LoadMediaStandalone(string path)
    {
        lock (_syncRoot)
            return NativeHandle.CreateMediaEmbedFromFile(path);
    }

    /// <summary>
    /// Load media from an in-memory buffer as a standalone embedding without touching the shared pending-media queue.
    /// </summary>
    public SafeMtmdEmbed LoadMediaStandalone(ReadOnlySpan<byte> data)
    {
        lock (_syncRoot)
            return NativeHandle.CreateMediaEmbedFromBuffer(data);
    }

    /// <summary>
    /// Clear any pending media buffers before or after tokenization.
    /// </summary>
    public void ClearMedia()
    {
        lock (_syncRoot)
            NativeHandle.ClearMedia();
    }

    /// <summary>
    /// Tokenize text (with optional special tokens) against the pending media buffers.
    /// </summary>
    public int Tokenize(string text, bool addSpecial, bool parseSpecial, out SafeMtmdInputChunks? chunks)
    {
        lock (_syncRoot)
            return NativeHandle.Tokenize(text, addSpecial, parseSpecial, out chunks);
    }

    /// <summary>
    /// Tokenize text (with optional special tokens) against explicit media embeddings.
    /// The caller retains ownership of <paramref name="embeds"/>.
    /// </summary>
    public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, out SafeMtmdInputChunks? chunks)
    {
        lock (_syncRoot)
            return NativeHandle.Tokenize(text, addSpecial, parseSpecial, embeds, out chunks);
    }

    /// <summary>
    /// Evaluate a chunk batch using the helper that performs mtmd encode + llama decode.
    /// </summary>
    public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
    {
        lock (_syncRoot)
            return NativeHandle.EvaluateChunks(chunks, llamaContext, ref nPast, seqId, nBatch, logitsLast);
    }

    public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
    {
        lock (_syncRoot)
            return NativeHandle.EvaluateChunk(chunkPtr, llamaContext, ref nPast, seqId, nBatch, logitsLast);
    }

    public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, ref int nPast, int seqId, int nBatch)
    {
        lock (_syncRoot)
            return NativeHandle.DecodeImageChunk(chunkPtr, llamaContext, encodedEmbeddings, ref nPast, seqId, nBatch);
    }

    /// <summary>
    /// Indicates whether the model supports vision inputs.
    /// </summary>
    public bool SupportsVision => NativeHandle.SupportVision();

    /// <summary>
    /// Indicates whether the model supports audio inputs.
    /// </summary>
    public bool SupportsAudio => NativeHandle.SupportAudio();

    /// <summary>
    /// Indicates whether the model decodes using the non-causal path.
    /// </summary>
    public bool UsesNonCausalAttention => NativeHandle.DecodeUseNonCausal();

    /// <summary>
    /// Indicates whether the model decodes using multi-scale RoPE.
    /// </summary>
    public bool UsesMRope => NativeHandle.DecodeUseMRope();

    /// <summary>
    /// Gets the audio bitrate advertised by the model.
    /// </summary>
    public int AudioBitrate => NativeHandle.GetAudioBitrate();

    /// <inheritdoc />
    public void Dispose() => NativeHandle.Dispose();
}
