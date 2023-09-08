using LLama.Native;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;

/// <summary>
/// The default logger of LLamaSharp. On default it write to console. Use methods of `LLamaLogger.Default` to change the behavior.
/// It's recommended to inherit `ILogger` to customize the behavior.
/// </summary>
public sealed class LLamaDefaultLogger
    : ILogger
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
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Log("[LLamaSharp]", formatter(state, exception), logLevel);
    }


    /// <summary>
    /// Checks if the given <paramref name="logLevel" /> is enabled.
    /// </summary>
    /// <param name="logLevel">level to be checked.</param>
    /// <returns>
    ///   <see langword="true" /> if enabled; <see langword="false" /> otherwise.
    /// </returns>
    public bool IsEnabled(LogLevel logLevel) => true;


    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>
    /// A disposable object that ends the logical operation scope on dispose.
    /// </returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;


    /// <summary>
    /// Log a message
    /// </summary>
    /// <param name="source">The source of this message (e.g. class name)</param>
    /// <param name="message">The message to log</param>
    /// <param name="level">Severity level of this message</param>
    private void Log(string source, string message, LogLevel level)
    {
        if (level == LogLevel.Information)
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
    private void Info(string message)
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
    private void Warn(string message)
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
    private void Error(string message)
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
    private void NativeLogCallback(LLamaNativeLogType level, string message)
    {
        if (string.IsNullOrEmpty(message))
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
        Log("[LLama.cpp]", message.TrimEnd('\n'), loglevel);
    }
}