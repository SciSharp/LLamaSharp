using LLama.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace LLama
{
    /// <summary>
    /// A llama_context, which holds all the context required to interact with a model
    /// </summary>
    public sealed class LLamaContext
        : IDisposable
    {
        private readonly ILogger? _logger;

        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => NativeHandle.VocabCount;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public uint ContextSize => NativeHandle.ContextSize;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => NativeHandle.EmbeddingSize;

        /// <summary>
        /// The context params set for this context
        /// </summary>
        public IContextParams Params { get; }

        /// <summary>
        /// The native handle, which is used to be passed to the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLLamaContextHandle NativeHandle { get; }

        /// <summary>
        /// The encoding set for this model to deal with text input.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Get or set the number of threads to use for generation
        /// </summary>
        public int GenerationThreads
        {
            get => NativeHandle.GenerationThreads;
            set => NativeHandle.GenerationThreads = value;
        }

        /// <summary>
        /// Get or set the number of threads to use for batch processing
        /// </summary>
        public int BatchThreads
        {
            get => NativeHandle.BatchThreads;
            set => NativeHandle.BatchThreads = value;
        }

        /// <summary>
        /// Get the maximum batch size for this context
        /// </summary>
        public uint BatchSize => NativeHandle.BatchSize;

        /// <summary>
        /// Get the special tokens for the model associated with this context
        /// </summary>
        public SafeLlamaModelHandle.ModelTokens Tokens { get; }
        
        /// <summary>
        /// Create a new LLamaContext for the given LLamaWeights
        /// </summary>
        /// <param name="model"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public LLamaContext(LLamaWeights model, IContextParams @params, ILogger? logger = null)
        {
            if (model.NativeHandle.IsClosed)
                throw new ObjectDisposedException("Cannot create context, model weights have been disposed");

            Params = @params;

            _logger = logger;
            Encoding = @params.Encoding;

            @params.ToLlamaContextParams(out var lparams);
            NativeHandle = SafeLLamaContextHandle.Create(model.NativeHandle, lparams);

            Tokens = model.Tokens;
        }

        /// <summary>
        /// Tokenize a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Whether to add a bos to the text.</param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        public LLamaToken[] Tokenize(string text, bool addBos = true, bool special = false)
        {
            return NativeHandle.Tokenize(text, addBos, special, Encoding);
        }

        /// <summary>
        /// Detokenize the tokens to text.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        [Obsolete("Use a `StreamingTokenDecoder` instead")]
        public string DeTokenize(IReadOnlyList<LLamaToken> tokens)
        {
            // Do **not** use this method as an example of how to correctly use the StreamingTokenDecoder!
            // It should be kept around for the entire time you are decoding one stream of tokens.

            var decoder = new StreamingTokenDecoder(this);
            decoder.AddRange(tokens);
            return decoder.Read();
        }

        #region state load/save
        /// <summary>
        /// Save the state to specified path.
        /// </summary>
        /// <param name="filename"></param>
        public void SaveState(string filename)
        {
            // Delete that file before overwriting it
            if (File.Exists(filename))
                File.Delete(filename);

            // Get the exact size of the state
            var stateSize = NativeHandle.GetStateSize();

            // Map the file and write the bytes directly to it. This saves copying the bytes into a C# array
            nuint writtenBytes;
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Create, null, checked((long)stateSize)))
            using (var view = file.CreateViewAccessor(0, checked((long)stateSize)))
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    try
                    {
                        writtenBytes = NativeHandle.GetState(ptr, stateSize);
                    }
                    finally
                    {
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }

            Debug.Assert(stateSize == writtenBytes, $"Expected to write {stateSize} bytes, but actually wrote {writtenBytes}");
        }

        /// <summary>
        /// Save the state of a particular sequence to specified path.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sequence"></param>
        public void SaveState(string filename, LLamaSeqId sequence)
        {
            // Delete that file before overwriting it
            if (File.Exists(filename))
                File.Delete(filename);

            // Get the exact size of the state
            var stateSize = NativeHandle.GetStateSize(sequence);

            // Map the file and write the bytes directly to it. This saves copying the bytes into a C# array
            nuint writtenBytes;
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Create, null, checked((long)stateSize)))
            using (var view = file.CreateViewAccessor(0, checked((long)stateSize)))
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    try
                    {
                        writtenBytes = NativeHandle.GetState(ptr, stateSize, sequence);
                    }
                    finally
                    {
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }

            Debug.Assert(stateSize == writtenBytes, $"Expected to write {stateSize} bytes, but actually wrote {writtenBytes}");
        }

        /// <summary>
        /// Get the state data as an opaque handle, which can be loaded later using <see cref="LoadState(State)"/>
        /// </summary>
        /// <remarks>Use <see cref="SaveState(string)"/> if you intend to save this state to disk.</remarks>
        /// <returns></returns>
        public State GetState()
        {
            var stateSize = NativeHandle.GetStateSize();

            // Allocate a chunk of memory large enough to hold the entire state
            var memory = Marshal.AllocHGlobal((nint)stateSize);
            try
            {
                // Copy the state data into memory
                nuint actualSize;
                unsafe
                {
                    actualSize = NativeHandle.GetState((byte*)memory, stateSize);
                }

                Debug.Assert(actualSize == stateSize);

                // Wrap memory in a "state"
                var state = new State(memory, actualSize);

                // Set memory to zero, to prevent it being freed in finally block
                memory = IntPtr.Zero;

                return state;
            }
            finally
            {
                if (memory != IntPtr.Zero)
                    Marshal.FreeHGlobal(memory);
            }
        }

        /// <summary>
        /// Get the state data as an opaque handle, which can be loaded later using <see cref="LoadState(State)"/>
        /// </summary>
        /// <remarks>Use <see cref="SaveState(string, LLamaSeqId)"/> if you intend to save this state to disk.</remarks>
        /// <returns></returns>
        public SequenceState GetState(LLamaSeqId sequence)
        {
            var stateSize = NativeHandle.GetStateSize(sequence);

            // Allocate a chunk of memory large enough to hold the entire state
            var memory = Marshal.AllocHGlobal((nint)stateSize);
            try
            {
                // Copy the state data into memory, discover the actual size required
                nuint actualSize;
                unsafe
                {
                    actualSize = NativeHandle.GetState((byte*)memory, stateSize, sequence);
                }

                Debug.Assert(actualSize == stateSize);

                // Wrap memory in a "state"
                var state = new SequenceState(memory, actualSize);

                // Set memory to zero, to prevent it being freed in finally block
                memory = IntPtr.Zero;

                return state;
            }
            finally
            {
                if (memory != IntPtr.Zero)
                    Marshal.FreeHGlobal(memory);
            }
        }

        /// <summary>
        /// Load the state from specified path.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadState(string filename)
        {
            // Map state file into memory and pass that pointer directly to `llama_set_state_data` to load from
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, null))
            using (var view = file.CreateViewAccessor())
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    try
                    {
                        NativeHandle.SetState(ptr, (nuint)view.SafeMemoryMappedViewHandle.ByteLength);
                    }
                    finally
                    {
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }
        }

        /// <summary>
        /// Load the state from specified path into a particular sequence
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sequence"></param>
        public void LoadState(string filename, LLamaSeqId sequence)
        {
            // Map state file into memory and pass that pointer directly to `llama_set_state_data` to load from
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, null))
            using (var view = file.CreateViewAccessor())
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    try
                    {
                        NativeHandle.SetState(ptr, (nuint)view.SafeMemoryMappedViewHandle.ByteLength, sequence);
                    }
                    finally
                    {
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }
        }

        /// <summary>
        /// Load the state from memory.
        /// </summary>
        /// <param name="state"></param>
        public void LoadState(State state)
        {
            unsafe
            {
                NativeHandle.SetState((byte*)state.DangerousGetHandle(), state.Size);
            }
        }

        /// <summary>
        /// Load the state from memory into a particular sequence
        /// </summary>
        /// <param name="state"></param>
        /// <param name="sequence"></param>
        public void LoadState(SequenceState state, LLamaSeqId sequence)
        {
            unsafe
            {
                NativeHandle.SetState((byte*)state.DangerousGetHandle(), state.Size, sequence);
            }
        }
        #endregion

        /// <summary>
        /// Gets whether or not the Bos token should be added.
        /// From common.cpp https://github.com/ggerganov/llama.cpp/blob/60325fa56f61c228464c9f065db3aa6a61f2156e/common/common.cpp#L2417
        /// </summary>
        /// <returns></returns>
        public bool ShouldAddBosToken()
        {
            var addBos = NativeApi.llama_add_bos_token(NativeHandle.ModelHandle);
            //return addBos != -1 ? Convert.ToBoolean(addBos) : NativeHandle.LLamaVocabType == LLamaVocabType.SentencePiece;
            return addBos;
        }

        #region eval overloads
        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        public EncodeResult Encode(LLamaBatch batch)
        {
            if (batch.TokenCount == 0)
                return 0;
            if (batch.TokenCount > BatchSize)
                throw new ArgumentException("Input contains more tokens than configured batch size", nameof(batch));

            return (EncodeResult)NativeHandle.Encode(batch);
        }

        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="cancellationToken"></param>
        public Task<EncodeResult> EncodeAsync(LLamaBatch batch, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Encode(batch), cancellationToken);
        }

        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        public DecodeResult Decode(LLamaBatch batch)
        {
            if (batch.TokenCount == 0)
                return 0;
            if (batch.TokenCount > BatchSize)
                throw new ArgumentException("Input contains more tokens than configured batch size", nameof(batch));

            return (DecodeResult)NativeHandle.Decode(batch);
        }

        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="cancellationToken"></param>
        public Task<DecodeResult> DecodeAsync(LLamaBatch batch, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Decode(batch), cancellationToken);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        public DecodeResult Decode(LLamaBatchEmbeddings batch)
        {
            if (batch.EmbeddingsCount == 0)
                return 0;
            if (batch.EmbeddingsCount > BatchSize)
                throw new ArgumentException("Input contains more tokens than configured batch size", nameof(batch));
            
            return (DecodeResult)NativeHandle.Decode(batch);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="cancellationToken"></param>
        public Task<DecodeResult> DecodeAsync(LLamaBatchEmbeddings batch, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Decode(batch), cancellationToken);
        }

        /// <summary>
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="id"></param>
        /// <param name="batch"></param>
        /// <param name="n_past"></param>
        /// <returns>A tuple, containing the decode result, the number of tokens that have <b>not</b> been decoded yet and the total number of tokens that have been decoded.</returns>
        public Task<(DecodeResult, int, int)> DecodeAsync(List<LLamaToken> tokens, LLamaSeqId id, LLamaBatch batch, int n_past)
        {
            return Task.Run(() =>
            {
                var past = n_past;
                var res = NativeHandle.Decode(tokens, id, batch, ref past);
                return (res.Item1, res.Item2, past);
                });
        }
        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            NativeHandle.Dispose();
        }

        /// <summary>
        /// The state of this context, which can be reloaded later
        /// </summary>
        public class State
            : SafeLLamaHandleBase
        {
            private readonly nuint _size;
            /// <summary>
            /// Get the size in bytes of this state object
            /// </summary>
            public nuint Size => _size;

            internal State(IntPtr memory, nuint size)
                : base(memory, true)
            {
                _size = size;
            }

            /// <inheritdoc />
            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(handle);
                return true;
            }

            /// <summary>
            /// Write all the bytes of this state to the given stream
            /// </summary>
            /// <param name="stream"></param>
            public async Task SaveAsync(Stream stream)
            {
                UnmanagedMemoryStream from;
                unsafe
                {
                    from = new UnmanagedMemoryStream((byte*)handle.ToPointer(), checked((long)Size));
                }
                await from.CopyToAsync(stream);
            }

            /// <summary>
            /// Write all the bytes of this state to the given stream
            /// </summary>
            /// <param name="stream"></param>
            public void Save(Stream stream)
            {
                UnmanagedMemoryStream from;
                unsafe
                {
                    from = new UnmanagedMemoryStream((byte*)handle.ToPointer(), checked((long)Size));
                }
                from.CopyTo(stream);
            }

            /// <summary>
            /// Load a state from a stream
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public static async Task<State> LoadAsync(Stream stream)
            {
                var memory = Marshal.AllocHGlobal((nint)stream.Length);
                var state = new State(memory, (nuint)stream.Length);

                UnmanagedMemoryStream dest;
                unsafe
                {
                    dest = new UnmanagedMemoryStream((byte*)memory.ToPointer(), stream.Length);
                }
                await stream.CopyToAsync(dest);

                return state;
            }

            /// <summary>
            /// Load a state from a stream
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public static State Load(Stream stream)
            {
                var memory = Marshal.AllocHGlobal((nint)stream.Length);
                var state = new State(memory, (nuint)stream.Length);

                unsafe
                {
                    var dest = new UnmanagedMemoryStream((byte*)memory.ToPointer(), stream.Length);
                    stream.CopyTo(dest);
                }

                return state;
            }
        }

        /// <summary>
        /// The state of a single sequence, which can be reloaded later
        /// </summary>
        public class SequenceState
            : SafeLLamaHandleBase
        {
            private readonly nuint _size;
            /// <summary>
            /// Get the size in bytes of this state object
            /// </summary>
            public nuint Size => _size;

            internal SequenceState(IntPtr memory, nuint size)
                : base(memory, true)
            {
                _size = size;
            }

            /// <inheritdoc />
            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(handle);
                return true;
            }

            /// <summary>
            /// Copy bytes to a destination pointer.
            /// </summary>
            /// <param name="dst">Destination to write to</param>
            /// <param name="length">Length of the destination buffer</param>
            /// <param name="offset">Offset from start of src to start copying from</param>
            /// <returns>Number of bytes written to destination</returns>
            public unsafe ulong CopyTo(byte* dst, ulong length, ulong offset = 0)
            {
                var copy = Math.Min(length, _size - offset);

                var src = (byte*)DangerousGetHandle();
                src += offset;

                Buffer.MemoryCopy(src, dst, length, copy);
                return copy;
            }
        }
    }
}
