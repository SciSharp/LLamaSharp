using System;
using Microsoft.Extensions.Logging;
using Serilog;
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
    private static ILoggerFactory _loggerFactory;
    private static readonly object _lock = new object();

    public static Logger Default => _instance.Value;

    private Logger()
    {
        var logConfig = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}");

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(logConfig.CreateLogger(), dispose: true);
        });
    }

    public void ToConsole()
    {
        // 不需要处理，Serilog 默认就输出到控制台
    }

    public void ToFile(string filename)
    {
        var logConfig = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(filename, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}");

        lock (_lock)
        {
            _loggerFactory.Dispose();

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(logConfig.CreateLogger(), dispose: true);
            });
        }
    }

    public void Info(string message)
    {
        _loggerFactory.CreateLogger<Logger>().LogInformation(message);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Warn(string message)
    {
        _loggerFactory.CreateLogger<Logger>().LogWarning(message);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Error(string message)
    {
        _loggerFactory.CreateLogger<Logger>().LogError(message);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}