using LLama.Exceptions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Collections.Generic;
using LLama.Abstractions;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        static NativeApi()
        {
            // Overwrite the Dll import resolver for this assembly. The resolver gets
            // called by the runtime every time that a call into a DLL is required. The
            // resolver returns the loaded DLL handle. This allows us to take control of
            // which llama.dll is used.
            SetDllImportResolver();

            // Set flag to indicate that this point has been passed. No native library config can be done after this point.
            NativeLibraryConfig.LLama.LibraryHasLoaded = true;
            NativeLibraryConfig.LLava.LibraryHasLoaded = true;

            // Immediately make a call which requires loading the llama DLL. This method call
            // can't fail unless the DLL hasn't been loaded.
            try
            {
                llama_empty_call();
            }
            catch (DllNotFoundException)
            {
                throw new RuntimeError("The native library cannot be correctly loaded. It could be one of the following reasons: \n" +
                    "1. No LLamaSharp backend was installed. Please search LLamaSharp.Backend and install one of them. \n" +
                    "2. You are using a device with only CPU but installed cuda backend. Please install cpu backend instead. \n" +
                    "3. One of the dependency of the native library is missed. Please use `ldd` on linux, `dumpbin` on windows and `otool`" +
                    "to check if all the dependency of the native library is satisfied. Generally you could find the libraries under your output folder.\n" +
                    "4. Try to compile llama.cpp yourself to generate a libllama library, then use `LLama.Native.NativeLibraryConfig.WithLibrary` " +
                    "to specify it at the very beginning of your code. For more information about compilation, please refer to LLamaSharp repo on github.\n");
            }

            // Now that the "loaded" flag is set configure logging in llama.cpp
            if (NativeLibraryConfig.LLama.LogCallback != null)
                NativeLogConfig.llama_log_set(NativeLibraryConfig.LLama.LogCallback);

            // Init llama.cpp backend
            llama_backend_init();
        }

#if NET5_0_OR_GREATER
        private static IntPtr _loadedLlamaHandle;
        private static IntPtr _loadedLlavaSharedHandle;
#endif

        private static void SetDllImportResolver()
        {
            // NativeLibrary is not available on older runtimes. We'll have to depend on
            // the normal runtime dll resolution there.
#if NET5_0_OR_GREATER
            NativeLibrary.SetDllImportResolver(typeof(NativeApi).Assembly, (name, _, _) =>
            {
                if (name == "llama")
                {
                    // If we've already loaded llama return the handle that was loaded last time.
                    if (_loadedLlamaHandle != IntPtr.Zero)
                        return _loadedLlamaHandle;

                    // Try to load a preferred library, based on CPU feature detection
                    _loadedLlamaHandle = NativeLibraryUtils.TryLoadLibrary(NativeLibraryConfig.LLama, out _loadedLLamaLibrary);
                    return _loadedLlamaHandle;
                }

                if (name == "llava_shared")
                {
                    // If we've already loaded llava return the handle that was loaded last time.
                    if (_loadedLlavaSharedHandle != IntPtr.Zero)
                        return _loadedLlavaSharedHandle;

                    // Try to load a preferred library, based on CPU feature detection
                    _loadedLlavaSharedHandle = NativeLibraryUtils.TryLoadLibrary(NativeLibraryConfig.LLava, out _loadedLLavaLibrary);
                    return _loadedLlavaSharedHandle;
                }

                // Return null pointer to indicate that nothing was loaded.
                return IntPtr.Zero;
            });
#endif
        }

        /// <summary>
        /// Get the loaded native library. If you are using netstandard2.0, it will always return null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static INativeLibrary? GetLoadedNativeLibrary(NativeLibraryName name)
        {
            return name switch
            {
                NativeLibraryName.LLama => _loadedLLamaLibrary,
                NativeLibraryName.LLava => _loadedLLavaLibrary,
                _ => throw new ArgumentException($"Library name {name} is not found.")
            };
        }

        internal const string libraryName = "llama";
        internal const string llavaLibraryName = "llava_shared";

        private static INativeLibrary? _loadedLLamaLibrary = null;
        private static INativeLibrary? _loadedLLavaLibrary = null;
    }
}
