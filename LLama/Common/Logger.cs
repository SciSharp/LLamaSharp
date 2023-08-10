using LLama.Native;
using System;
using System.Diagnostics;
using System.IO;
using static LLama.Common.ILLamaLogger;

namespace LLama.Common;

public interface ILLamaLogger
{
    public enum LogLevel
    {
        Debug = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    /// <summary>
    /// Write the log in cosutomized way
    /// </summary>
    /// <param name="source">The source of the log. It may be a method name or class name.</param>
    /// <param name="message">The message.</param>
    /// <param name="level">The log level.</param>
    void Log(string source, string message, LogLevel level);
}

/// <summary>
/// The default logger of LLamaSharp. On default it write to console. User methods of `LLamaLogger.Default` to change the behavior.
/// It's more recommended to inherit `ILLamaLogger` to cosutomize the behavior.
/// </summary>
public sealed class LLamaDefaultLogger : ILLamaLogger
{
    private static readonly Lazy<LLamaDefaultLogger> _instance = new Lazy<LLamaDefaultLogger>(() => new LLamaDefaultLogger());

    private bool _toConsole = true;
    private bool _toFile = false;

    private FileStream? _fileStream = null;
    private StreamWriter _fileWriter = null;

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

	public LLamaDefaultLogger EnableConsole()
    {
        _toConsole = true;
        return this;
    }

    public LLamaDefaultLogger DisableConsole()
    {
        _toConsole = false;
        return this;
    }

    public LLamaDefaultLogger EnableFile(string filename, FileMode mode = FileMode.Append)
    {
        _fileStream = new FileStream(filename, mode, FileAccess.Write);
        _fileWriter = new StreamWriter(_fileStream);
        _toFile = true;
        return this;
    }

    public LLamaDefaultLogger DisableFile(string filename)
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

    private string MessageFormat(string level, string message)
    {
        DateTime now = DateTime.Now;
        string formattedDate = now.ToString("yyyy.MM.dd HH:mm:ss");
        return $"[{formattedDate}][{level}]: {message}";
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