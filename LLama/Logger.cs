using System;
using System.Diagnostics;
using System.IO;

namespace LLama.Types;

/// <summary>
/// The logger of LLamaSharp. On default it write to console. User methods of `LLamaLogger.Default` to change the behavior.
/// </summary>
public sealed class LLamaLogger
{
    private static readonly Lazy<LLamaLogger> _instance = new Lazy<LLamaLogger>(() => new LLamaLogger());

    private bool _toConsole = true;
    private bool _toFile = false;

    private FileStream? _fileStream = null;
    private StreamWriter _fileWriter = null;

    public static LLamaLogger Default => _instance.Value;

    private LLamaLogger()
    {
        
    }

    public LLamaLogger EnableConsole()
    {
        _toConsole = true;
        return this;
    }

    public LLamaLogger DisableConsole()
    {
        _toConsole = false;
        return this;
    }

    public LLamaLogger EnableFile(string filename, FileMode mode = FileMode.Append)
    {
        _fileStream = new FileStream(filename, mode, FileAccess.Write);
        _fileWriter = new StreamWriter(_fileStream);
        _toFile = true;
        return this;
    }

    public LLamaLogger DisableFile(string filename)
    {
        if(_fileWriter is not null)
        {
            _fileWriter.Close();
            _fileWriter = null;
        }
        if(_fileStream is not null)
        {
            _fileStream.Close();
            _fileStream = null;
        }
        _toFile = false;
        return this;
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
}