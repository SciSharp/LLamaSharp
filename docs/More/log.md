# The Logger in LLamaSharp

LLamaSharp supports customized logger because it could be used in many kinds of applications, like Winform/WPF, WebAPI and Blazor, so that the preference of logger varies.

## Define customized logger

What you need to do is to implement the `ILogger` interface. 

```cs
public interface ILLamaLogger
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error
    }
    void Log(string source, string message, LogLevel level);
}
```

The `source` specifies where the log message is from, which could be a function, a class, etc..

The `message` is the log message itself.

The `level` is the level of the information in the log. As shown above, there're four levels, which are `info`, `debug`, `warning` and `error` respectively.

The following is a simple example of the logger implementation:

```cs
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
}
```