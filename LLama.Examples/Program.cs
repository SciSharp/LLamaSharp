using LLama.Native;
using Spectre.Console;

AnsiConsole.MarkupLineInterpolated(
    $"""
    [purple]======================================================================================================[/]
     __       __                                       ____     __
    /\ \     /\ \                                     /\  _`\  /\ \
    \ \ \    \ \ \         __       ___ ___       __  \ \,\L\_\\ \ \___       __     _ __   _____
     \ \ \  __\ \ \  __  /'__`\   /' __` __`\   /'__`\ \/_\__ \ \ \  _ `\   /'__`\  /\` __\/\  __`\
      \ \ \L\ \\ \ \L\ \/\ \L\.\_ /\ \/\ \/\ \ /\ \L\.\_ /\ \L\ \\ \ \ \ \ /\ \L\.\_\ \ \/ \ \ \L\ \
       \ \____/ \ \____/\ \__/.\_\\ \_\ \_\ \_\\ \__/.\_\\ `\____\\ \_\ \_\\ \__/.\_\\ \_\  \ \ ,__/
        \/___/   \/___/  \/__/\/_/ \/_/\/_/\/_/ \/__/\/_/ \/_____/ \/_/\/_/ \/__/\/_/ \/_/   \ \ \/
    [purple]=========================================================================================[/] \ \_\ [purple]======[/]
                                                                                               \/_/

    """);

// Configure native library to use
NativeLibraryConfig
   .Instance
   .WithCuda()
   .WithLogs(LLamaLogLevel.Debug)
   .WithLogCallback((level, message) =>
    {
        var bg = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[{level}]: {message}");
        Console.BackgroundColor = bg;
    });

// Calling this method forces loading to occur now.
NativeApi.llama_empty_call();

await ExampleRunner.Run();

