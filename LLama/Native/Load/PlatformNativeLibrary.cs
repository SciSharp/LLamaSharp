#if !NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// A minimal, explicit-path native library loader used as a stand-in for
    /// <see cref="System.Runtime.InteropServices.NativeLibrary"/> on target frameworks
    /// (e.g. netstandard2.0) where that API does not exist. Only the single operation
    /// actually needed by <see cref="NativeLibraryUtils"/> - loading a library from a
    /// known file path - is implemented.
    /// </summary>
    internal static class PlatformNativeLibrary
    {
        /// <summary>
        /// Try to load a native library from an explicit file path.
        /// </summary>
        /// <param name="path">Full or relative path to the native library file.</param>
        /// <param name="handle">The OS handle of the loaded library, or <see cref="IntPtr.Zero"/> if loading failed.</param>
        /// <returns>True if the library was loaded successfully.</returns>
        internal static bool TryLoad(string path, out IntPtr handle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Windows.TryLoad(path, out handle);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Mac.TryLoad(path, out handle);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Linux.TryLoad(path, out handle);

            handle = IntPtr.Zero;
            return false;
        }

        private static class Windows
        {
            internal static bool TryLoad(string path, out IntPtr handle)
            {
                handle = LoadLibraryW(path);
                return handle != IntPtr.Zero;
            }

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW")]
            private static extern IntPtr LoadLibraryW(string lpFileName);
        }

        private static class Mac
        {
            private const int RTLD_NOW = 2;

            internal static bool TryLoad(string path, out IntPtr handle)
            {
                handle = dlopen(path, RTLD_NOW);
                return handle != IntPtr.Zero;
            }

            // On macOS, dlopen is exported by libSystem, which "libdl.dylib" resolves to.
            [DllImport("libdl.dylib", EntryPoint = "dlopen", CharSet = CharSet.Ansi)]
            private static extern IntPtr dlopen(string path, int mode);
        }

        private static class Linux
        {
            private const int RTLD_NOW = 2;

            internal static bool TryLoad(string path, out IntPtr handle)
            {
                // The native library that exports dlopen varies across distros/glibc versions
                // (e.g. glibc >= 2.34 folded libdl into libc). Try each known candidate in turn,
                // the way NativeLibrary.TryLoad's internal resolver would.
                if (TryDlopen(path, RTLD_NOW, DlopenLibDl2, out handle)) return true;
                if (TryDlopen(path, RTLD_NOW, DlopenLibDl, out handle)) return true;
                if (TryDlopen(path, RTLD_NOW, DlopenLibC, out handle)) return true;

                handle = IntPtr.Zero;
                return false;
            }

            private static bool TryDlopen(string path, int mode, Func<string, int, IntPtr> dlopen, out IntPtr handle)
            {
                try
                {
                    handle = dlopen(path, mode);
                    return handle != IntPtr.Zero;
                }
                catch (DllNotFoundException)
                {
                    // The candidate native library that exports dlopen isn't present on this system, try the next one.
                    handle = IntPtr.Zero;
                    return false;
                }
            }

            [DllImport("libdl.so.2", EntryPoint = "dlopen", CharSet = CharSet.Ansi)]
            private static extern IntPtr DlopenLibDl2(string path, int mode);

            [DllImport("libdl.so", EntryPoint = "dlopen", CharSet = CharSet.Ansi)]
            private static extern IntPtr DlopenLibDl(string path, int mode);

            // musl and glibc >= 2.34 export dlopen directly from libc.
            [DllImport("libc.so.6", EntryPoint = "dlopen", CharSet = CharSet.Ansi)]
            private static extern IntPtr DlopenLibC(string path, int mode);
        }
    }
}
#endif
