using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace LLama.Native;

/// <summary>
/// Methods for configuring llama.cpp logging
/// </summary>
public class NativeLogConfig
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
    /// Register a callback to receive llama log messages
    /// </summary>
    /// <param name="logCallback"></param>
#pragma warning disable IDE1006 // Naming Styles (name imitates llama.cpp)
    public static void llama_log_set(LLamaLogCallback? logCallback)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (NativeLibraryConfig.LibraryHasLoaded)
        {
            // The library is loaded, just pass the callback directly to llama.cpp
            native_llama_log_set(logCallback);
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

        // Bind a function that converts into the correct log level
        llama_log_set((level, message) =>
        {
            switch (level)
            {
                case LLamaLogLevel.Error:
                    logger.LogError("(llama.cpp): {message}", message);
                    break;
                case LLamaLogLevel.Warning:
                    logger.LogWarning("(llama.cpp): {message}", message);
                    break;
                case LLamaLogLevel.Info:
                    logger.LogInformation("(llama.cpp): {message}", message);
                    break;
                case LLamaLogLevel.Debug:
                    logger.LogDebug("(llama.cpp): {message}", message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        });
    }
}