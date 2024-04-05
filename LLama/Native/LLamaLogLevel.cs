using System;
using Microsoft.Extensions.Logging;

namespace LLama.Native
{
    /// <summary>
    /// Severity level of a log message
    /// </summary>
    public enum LLamaLogLevel
    {
        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that track the general flow of the application.
        /// </summary>
        Info = 4,

        /// <summary>
        /// Logs that are used for interactive investigation during development.
        /// </summary>
        Debug = 5,
    }

    internal static class LLamaLogLevelExtensions
    {
        public static LogLevel ToLogLevel(this LLamaLogLevel llama)
        {
            return (llama) switch
            {
                LLamaLogLevel.Error => LogLevel.Error,
                LLamaLogLevel.Warning => LogLevel.Warning,
                LLamaLogLevel.Info => LogLevel.Information,
                LLamaLogLevel.Debug => LogLevel.Debug,
                _ => throw new ArgumentOutOfRangeException(nameof(llama), llama, null)
            };
        }
    }
}
