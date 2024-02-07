using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace LLama.Native;
#if NET6_0_OR_GREATER
[SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries",
    Justification = "Intended to allow hooking native library resolution")]
internal static class LlamaModuleInitializer
{
    private static string? _selectedPath = null;
    private static string? _platformExtension;
    private static ConcurrentDictionary<string, IntPtr> _libraryHandles = new();
    public static string? CurrentPath => _selectedPath;
    public static List<string> SearchPaths { get; } = new List<string>();

    // Hook up import resolver whenever this library is loaded.
    // The import resolver is specific to this assembly, so it has no impact on the application loading it.
    [ModuleInitializer]
    internal static void InitializeLibrary()
    {
        NativeLibrary.SetDllImportResolver(typeof(LlamaModuleInitializer).Assembly, ImportResolver);
    }

    private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchpath)
    {
        if (SearchPaths.Count == 0)
            return IntPtr.Zero;
        // We'll be adding this back after, need to strip it off so we don't end up with liblib
        if (libraryName.StartsWith("lib"))
            libraryName = libraryName[3..];
        // don't want the extension either.
        if (libraryName.EndsWith(PlatformExtension))
            libraryName = libraryName[..^PlatformExtension.Length];
        return _libraryHandles.GetOrAdd(libraryName, TryLocateLibrary);
    }

    private static IntPtr TryLocateLibrary(string libraryName)
    {
        foreach (var path in EnumerateSearchPaths().Select(Path.GetFullPath))
        {
            if (!Directory.Exists(path))
                continue;

            foreach (var fileName in EnumerateFileNamePermutations(libraryName))
            {
                var targetFile = Path.Combine(path, fileName);
                if (!File.Exists(targetFile))
                    continue;
                if (!NativeLibrary.TryLoad(targetFile, out var handle) || handle == IntPtr.Zero)
                    continue;
                // Set selected path, if it was null. Do it in a thread safe way though
                Interlocked.CompareExchange(ref _selectedPath, path, null);
                return handle;
            }
        }

        // This will trigger the next/default import resolver to take over.
        return IntPtr.Zero;
    }

    /// <summary>
    /// Unloads native libraries loaded by this class, and removes the learned path.
    /// The libraries must not be in use, or bad things will happen.
    /// </summary>
    public static void Reset()
    {
        Interlocked.Exchange(ref _selectedPath, null);

        var handles = _libraryHandles.ToArray();

        foreach (var handle in handles)
        {
            NativeLibrary.Free(handle.Value);
            _libraryHandles.TryRemove(handle.Key, out _);
        }
    }

    private static IEnumerable<string> EnumerateSearchPaths()
    {
        if (_selectedPath is not null)
        {
            // Always prefer the first location we found
            yield return _selectedPath;
        }

        foreach (var path in SearchPaths)
            yield return path;
    }

    // The reason for this, is we don't want the static initializer to fail during the module initializer hook in.
    // That may be very difficult to debug. Doing it this way causes this to run on first use, instead of first type access.
    private static string PlatformExtension => _platformExtension ??=
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ".dll"
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? ".so"
                : ".dylib";

    private static IEnumerable<string> EnumerateFileNamePermutations(string libraryName)
    {
        // By convention, libxxxx comes first on non-windows, and second on windows.
        var names = new[]
        {
            $"{libraryName}{PlatformExtension}",
            $"lib{libraryName}{PlatformExtension}"
        };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Array.Reverse(names);

        return names;
    }
}

#endif