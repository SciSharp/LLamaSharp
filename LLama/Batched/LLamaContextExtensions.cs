using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using LLama.Native;

namespace LLama.Batched;

internal static class LLamaContextExtensions
{
    private const uint FileHeaderMagic = 3430400180;

    /// <summary>
    /// Save the state of a particular sequence to specified path. Also save some extra data which will be returned when loading.
    /// Data saved with this method <b>must</b> be saved with <see cref="LoadState(LLamaContext, string, LLamaSeqId, out byte[])"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="filename"></param>
    /// <param name="sequence"></param>
    /// <param name="header"></param>
    internal static void SaveState(this LLamaContext context, string filename, LLamaSeqId sequence, ReadOnlySpan<byte> header)
    {
        // Delete that file before overwriting it
        if (File.Exists(filename))
            File.Delete(filename);

        // Get the exact size of the state
        var stateSize = context.NativeHandle.GetStateSize(sequence);

        // Space for "extra" byte plus a 8 byte header
        var prefixSize = header.Length + 8;

        // Add enough space for the "extra" data and a 6 byte header
        var totalFileSize = (nuint)prefixSize + stateSize;

        // Map the file and write the bytes directly to it.
        nuint writtenBytes = 0;
        using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Create, null, (long)totalFileSize))
        {
            using (var view = file.CreateViewAccessor(0, (long)totalFileSize))
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    try
                    {
                        // Write prefix data
                        BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(ptr + writtenBytes, 4), FileHeaderMagic);
                        writtenBytes += 4;
                        BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(ptr + writtenBytes, 4), (uint)header.Length);
                        writtenBytes += 4;
                        header.CopyTo(new Span<byte>(ptr + writtenBytes, header.Length));
                        writtenBytes += (nuint)header.Length;

                        // Write state data
                        writtenBytes += context.NativeHandle.GetState(ptr + writtenBytes, stateSize, sequence);
                    }
                    finally
                    {
                        view.SafeMemoryMappedViewHandle.ReleasePointer();
                    }
                }
            }
        }

        Debug.Assert(totalFileSize == writtenBytes, $"Expected to write {totalFileSize} bytes, but actually wrote {writtenBytes}");
    }

    /// <summary>
    /// Load the state from the specified path into a particular sequence. Also reading header data. Must only be used with
    /// data previously saved with <see cref="SaveState(LLamaContext, string, LLamaSeqId, ReadOnlySpan{byte})"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="filename"></param>
    /// <param name="sequence"></param>
    /// <param name="header"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void LoadState(this LLamaContext context, string filename, LLamaSeqId sequence, out byte[] header)
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
                    var readBytes = 0;

                    // Read header
                    var magic = BinaryPrimitives.ReadUInt32BigEndian(new ReadOnlySpan<byte>(ptr + readBytes, 4));
                    readBytes += 4;
                    if (magic != FileHeaderMagic)
                        throw new InvalidOperationException("Invalid file header");

                    var headerLength = checked((int)BinaryPrimitives.ReadUInt32BigEndian(new ReadOnlySpan<byte>(ptr + readBytes, 4)));
                    readBytes += 4;

                    header = new byte[headerLength];
                    new Span<byte>(ptr + readBytes, headerLength).CopyTo(header);
                    readBytes += headerLength;

                    context.NativeHandle.SetState(ptr + readBytes, (nuint)((long)view.SafeMemoryMappedViewHandle.ByteLength - readBytes), sequence);
                }
                finally
                {
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }
    }
}