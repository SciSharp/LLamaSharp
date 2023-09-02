using LLama.Native;
using System;
using System.Diagnostics;
using System.IO;
using static LLama.Common.ILLamaLogger;

namespace LLama.Common;

/// <summary>
/// receives log messages from LLamaSharp
/// </summary>
public interface ILLamaLogger
{
    /// <summary>
    /// Severity level of a log message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs that are used for interactive investigation during development.
        /// </summary>
        Debug = 1,

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
        Info = 4
    }

    /// <summary>
    /// Write the log in customized way
    /// </summary>
    /// <param name="source">The source of the log. It may be a method name or class name.</param>
    /// <param name="message">The message.</param>
    /// <param name="level">The log level.</param>
    void Log(string source, string message, LogLevel level);
}

/// <summary>
/// The default logger of LLamaSharp. On default it write to console. Use methods of `LLamaLogger.Default` to change the behavior.
/// It's recommended to inherit `ILLamaLogger` to customize the behavior.
/// </summary>
public sealed class LLamaDefaultLogger
    : ILLamaLogger
{
    private static readonly Lazy<LLamaDefaultLogger> _instance = new Lazy<LLamaDefaultLogger>(() => new LLamaDefaultLogger());

    private bool _toConsole = true;
    private bool _toFile;

    private FileStream? _fileStream;
    private StreamWriter? _fileWriter;

    /// <summary>
    /// Get the default logger instance
    /// </summary>
    public static LLamaDefaultLogger Default => _instance.Value;

    private LLamaDefaultLogger()
    {

    }

    /// <summary>
    /// Enable logging output from llama.cpp
    /// </summary>
    /// <returns></returns>
    public LLamaDefaultLogger EnableNative()
    {
        EnableNativeLogCallback();
        return this;
    }

    /// <summary>
    /// Enable writing log messages to console
    /// </summary>
    /// <returns></returns>
    public LLamaDefaultLogger EnableConsole()
    {
        _toConsole = true;
        return this;
    }

    /// <summary>
    /// Disable writing messages to console
    /// </summary>
    /// <returns></returns>
    public LLamaDefaultLogger DisableConsole()
    {
        _toConsole = false;
        return this;
    }

    /// <summary>
    /// Enable writing log messages to file
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public LLamaDefaultLogger EnableFile(string filename, FileMode mode = FileMode.Append)
    {
        _fileStream = new FileStream(filename, mode, FileAccess.Write);
        _fileWriter = new StreamWriter(_fileStream);
        _toFile = true;
        return this;
    }

    /// <summary>
    /// Disable writing log messages to file
    /// </summary>
    /// <param name="filename">unused!</param>
    /// <returns></returns>
    [Obsolete("Use DisableFile method without 'filename' parameter")]
    public LLamaDefaultLogger DisableFile(string filename)
    {
        return DisableFile();
    }

    /// <summary>
    /// Disable writing log messages to file
    /// </summary>
    /// <returns></returns>
    public LLamaDefaultLogger DisableFile()
    {
        if (_fileWriter is not null)
        {
            _fileWriter.Close();
            _fileWriter = null;
        }
        if (_fileStream is not null)
        {
            _fileStream.Close();
            _fileStream = null;
        }
        _toFile = false;
        return this;
    }

    /// <summary>
    /// Log a message
    /// </summary>
    /// <param name="source">The source of this message (e.g. class name)</param>
    /// <param name="message">The message to log</param>
    /// <param name="level">Severity level of this message</param>
    public void Log(string source, string message, LogLevel level)
    {
        if (level == LogLevel.Info)
        {
            Info(message);
        }
        else if (level == LogLevel.Debug)
        {

        }
        else if (level == LogLevel.Warning)
        {
            Warn(message);
        }
        else if (level == LogLevel.Error)
        {
            Error(message);
        }
    }

    /// <summary>
    /// Write a log message with "Info" severity
    /// </summary>
    /// <param name="message"></param>
    public void Info(string message)
    {
        message = MessageFormat("info", message);
        if (_toConsole)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        if (_toFile)
        {
            Debug.Assert(_fileStream is not null);
            Debug.Assert(_fileWriter is not null);
            _fileWriter.WriteLine(message);
        }
    }

    /// <summary>
    /// Write a log message with "Warn" severity
    /// </summary>
    /// <param name="message"></param>
    public void Warn(string message)
    {
        message = MessageFormat("warn", message);
        if (_toConsole)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        if (_toFile)
        {
            Debug.Assert(_fileStream is not null);
            Debug.Assert(_fileWriter is not null);
            _fileWriter.WriteLine(message);
        }
    }

    /// <summary>
    /// Write a log message with "Error" severity
    /// </summary>
    /// <param name="message"></param>
    public void Error(string message)
    {
        message = MessageFormat("error", message);
        if (_toConsole)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        if (_toFile)
        {
            Debug.Assert(_fileStream is not null);
            Debug.Assert(_fileWriter is not null);
            _fileWriter.WriteLine(message);
        }
    }

    private static string MessageFormat(string level, string message)
    {
        var now = DateTime.Now;
        return $"[{now:yyyy.MM.dd HH:mm:ss}][{level}]: {message}";
    }

    /// <summary>
    /// Register native logging callback
    /// </summary>
	private void EnableNativeLogCallback()
    {
        // TODO: Move to a more appropriate place once we have a intitialize method
        NativeApi.llama_log_set(NativeLogCallback);
    }

    /// <summary>
    /// Callback for native logging function
    /// </summary>
    /// <param name="level">The log level</param>
    /// <param name="message">The log message</param>
    private void NativeLogCallback(LogLevel level, string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        // Note that text includes the new line character at the end for most events.
        // If your logging mechanism cannot handle that, check if the last character is '\n' and strip it
        // if it exists.
        // It might not exist for progress report where '.' is output repeatedly.
        Log(default!, message.TrimEnd('\n'), level);
    }

}