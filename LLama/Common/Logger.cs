using LLama.Native;
using Microsoft.Extensions.Logging;

public static class Logger
{
    private static ILoggerFactory _loggerFactory;
    private static ILogger<NativeApi> _nativeLogger;

    public static void Configure(ILoggerFactory loggerFactory, bool enableNativeLogging)
    {
        _loggerFactory = loggerFactory;
        if (enableNativeLogging)
        {
            _nativeLogger = _loggerFactory.CreateLogger<NativeApi>();
            NativeApi.llama_log_set(NativeLogCallback);
        }
    }

    public static ILogger<T>? Create<T>()
    {
        return _loggerFactory?.CreateLogger<T>();
    }

    private static void NativeLogCallback(LLamaNativeLogType level, string message)
    {
        if (_nativeLogger is null || string.IsNullOrEmpty(message))
            return;

        var loglevel = level switch
        {
            LLamaNativeLogType.Info => LogLevel.Information,
            LLamaNativeLogType.Debug => LogLevel.Debug,
            LLamaNativeLogType.Warning => LogLevel.Warning,
            LLamaNativeLogType.Error => LogLevel.Error,
            _ => LogLevel.None
        };

        // Note that text includes the new line character at the end for most events.
        // If your logging mechanism cannot handle that, check if the last character is '\n' and strip it
        // if it exists.
        // It might not exist for progress report where '.' is output repeatedly.
        _nativeLogger.Log(loglevel, message.TrimEnd('\n'));
    }
}
