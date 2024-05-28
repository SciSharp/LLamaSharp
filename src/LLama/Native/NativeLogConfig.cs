using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace LLama.Native;

/// <summary>
/// Configure llama.cpp logging
/// </summary>
public static class NativeLogConfig
{
    /// <summary>
    /// Callback from llama.cpp with log messages
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public delegate void LLamaLogCallback(LLamaLogLevel level, string message);

    /// <summary>
    /// Register a callback to receive llama log messages
    /// </summary>
    /// <param name="logCallback"></param>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_log_set")]
    private static extern void native_llama_log_set(LLamaLogCallback? logCallback);

    /// <summary>
    /// A GC handle for the current log callback to ensure the callback is not collected
    /// </summary>
    private static GCHandle? _currentLogCallbackHandle;

    /// <summary>
    /// Register a callback to receive llama log messages
    /// </summary>
    /// <param name="logCallback"></param>
#pragma warning disable IDE1006 // Naming Styles (name imitates llama.cpp)
    public static void llama_log_set(LLamaLogCallback? logCallback)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (NativeLibraryConfig.LLama.LibraryHasLoaded)
        {
            // The library is loaded, just pass the callback directly to llama.cpp
            native_llama_log_set(logCallback);

            // Save a GC handle, to ensure the callback is not collected
            _currentLogCallbackHandle?.Free();
            _currentLogCallbackHandle = null;
            if (logCallback != null)
                _currentLogCallbackHandle = GCHandle.Alloc(logCallback);
        }
        else
        {
            // We can't set the log method yet since that would cause the llama.dll to load.
            // Instead configure it to be set when the native library loading is done
            NativeLibraryConfig.Instance.WithLogCallback(logCallback);
        }
    }

    /// <summary>
    /// Register a callback to receive llama log messages
    /// </summary>
    /// <param name="logger"></param>
#pragma warning disable IDE1006 // Naming Styles (name imitates llama.cpp)
    public static void llama_log_set(ILogger? logger)
#pragma warning restore IDE1006 // Naming Styles
    {
        // Clear the logger
        if (logger == null)
        {
            llama_log_set((LLamaLogCallback?)null);
            return;
        }

        var builderThread = new ThreadLocal<StringBuilder>(() => new StringBuilder());

        // Bind a function that combines messages until a newline is encountered, then logs it all as one message
        llama_log_set((level, message) =>
        {
            var builder = builderThread.Value!;

            builder.Append(message);

            if (!message.EndsWith("\n"))
                return;

            // Remove the newline from the end
            builder.Remove(builder.Length - 1, 1);

            logger.Log(level.ToLogLevel(), "{message}", builder.ToString());
            builder.Clear();
        });
    }
}