using LLama.Native;
using Spectre.Console;
using System.Runtime.InteropServices;

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

// Configure logging. Change this to `true` to see log messages from llama.cpp
var showLLamaCppLogs = false;
NativeLibraryConfig
   .All
   .WithLogCallback((level, message) =>
   {
       if (showLLamaCppLogs)
           Console.WriteLine($"[llama {level}]: {message.TrimEnd('\n')}");
   });

// Configure native library to use. This must be done before any other llama.cpp methods are called!
NativeLibraryConfig
   .All
   .WithCuda()
   //.WithAutoDownload() // An experimental feature
   .DryRun(out var loadedllamaLibrary, out var loadedLLavaLibrary);

// Calling this method forces loading to occur now.
NativeApi.llama_empty_call();

await ExampleRunner.Run();