using Spectre.Console;
using System;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenerateSolution
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.InputEncoding = Encoding.Unicode;
            System.Console.OutputEncoding = Encoding.Unicode;

            // Check if we can accept key strokes
            if (!AnsiConsole.Profile.Capabilities.Interactive)
            {
                AnsiConsole.MarkupLine("[red]Environment does not support interaction.[/]");
                return;
            }

            var options = AskOptions();
            var cmakePath = AskCMakePath();
            if(string.IsNullOrEmpty(cmakePath) == true) 
            {
                cmakePath = "C:\\Program Files\\CMake\\bin\\cmake.exe";
            }
            AnsiConsole.MarkupLine("You have selected: [yellow]{0}[/]", cmakePath);

            string cmakeListsPath = @"..\..\..\..\CMakeLists.txt";

            //cmake [<options>] -B <path-to-build> [-S <path-to-source>]
            //TODO: get the chosen arguments from above (hardcoded values below)
            //TODO: edit the CMakeList.txt.in template and create the CMakeLists.txt with the chosen options
            cmakeListsPath += " -G \"Visual Studio 17 2022\" -A x64 -B ..\\..\\..\\..\\ -S ..\\..\\..\\..\\";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = cmakePath,
                Arguments = cmakeListsPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                bool bSuccess = false;
                string lastError = "";
                AnsiConsole.Progress()
                    .AutoClear(false)
                    .Columns(new ProgressColumn[]
                    {
                            new TaskDescriptionColumn(),
                            new SpinnerColumn(Spinner.Known.Ascii),
                    })
                    .Start(ctx =>
                    {
                        var cmakeTask = ctx.AddTask("Generating VS Solution", autoStart: false).IsIndeterminate();
                        cmakeTask.StartTask();
                        using (Process process = new Process())
                        {
                            process.StartInfo = startInfo;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            lastError = process.StandardError.ReadToEnd();
                            process.WaitForExit();
                            cmakeTask.StopTask();
                            if (process.ExitCode == 0)
                            {
                                bSuccess = true;
                            }
                        }
                    });

                if (bSuccess == true)
                {
                    AnsiConsole.WriteLine("VS solution generated successfully.");
                }
                else
                {
                    AnsiConsole.WriteLine($"Error running CMake configuration: {lastError}");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("[red]ERROR[/] " + ex.Message);
            }

            Console.ReadLine();
        }

        public static string AskCMakePath()
        {
            return AnsiConsole.Prompt(
                            new TextPrompt<string>("What's your [green]CMake path[/] (default: C:\\Program Files\\CMake\\bin\\cmake.exe)?")
                                .AllowEmpty());
        }

        public static List<string> AskOptions()
        {
            var options = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .PageSize(10)
                    .Title("Select the preferred [green]options[/]?")
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to toggle an option, [green]<enter>[/] to accept)[/]")
                    .AddChoiceGroup("Avx", new[]
                    {
                        "Avx2", "Avx512"
                    })
                    .AddChoiceGroup("Cuda", new[]
                    {
                        "Cuda"
                    })
                    .AddChoices(new[]
                    {
                        "x64",
                    })
                    .AddChoiceGroup("Visual Studio", new[]
                    {
                        "Visual Studio 16 2019", 
                        "Visual Studio 17 2022"
                    })
                    );

            if (options.Count > 0)
            {
                AnsiConsole.MarkupLine("You have selected: [yellow]{0}[/]", string.Join(",",options));
            }
            
            return options;
        }
    }
}
