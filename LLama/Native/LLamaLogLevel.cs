using System;
using Microsoft.Extensions.Logging;

namespace LLama.Native
{
    /// <summary>
    /// Severity level of a log message. This enum should always be aligned with
    /// the one defined on llama.cpp side at
    /// https://github.com/ggerganov/llama.cpp/blob/0eb4e12beebabae46d37b78742f4c5d4dbe52dc1/ggml/include/ggml.h#L559
    /// </summary>
    public enum LLamaLogLevel
    {
        /// <summary>
        /// Logs are never written.
        /// </summary>
        None = 0,

        /// <summary>
        /// Logs that are used for interactive investigation during development.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs that track the general flow of the application.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Continue log level is equivalent to None in the way it is used in llama.cpp.
        /// </summary>
        Continue = 5,
    }

    internal static class LLamaLogLevelExtensions
    {
        /// <summary>
        /// Keeps track of the previous log level to be able to handle the log level <see cref="LLamaLogLevel.Continue"/>.
        /// </summary>
        [ThreadStatic] private static LogLevel _previous;

        public static LogLevel ToLogLevel(this LLamaLogLevel llama)
        {
            _previous = llama switch
            {
                LLamaLogLevel.None => LogLevel.None,
                LLamaLogLevel.Debug => LogLevel.Debug,
                LLamaLogLevel.Info => LogLevel.Information,
                LLamaLogLevel.Warning => LogLevel.Warning,
                LLamaLogLevel.Error => LogLevel.Error,
                LLamaLogLevel.Continue => _previous,
                _ => throw new ArgumentOutOfRangeException(nameof(llama), llama, null)
            };
            return _previous;
        }
    }
}