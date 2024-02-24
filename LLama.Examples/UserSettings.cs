using Spectre.Console;

namespace LLama.Examples;

internal static class UserSettings
{
    private static readonly string SettingsFilePath = Path.Join(AppContext.BaseDirectory, "DefaultModel.env");

    private static string? ReadDefaultModelPath()
    {
        if (!File.Exists(SettingsFilePath))
            return null;

        string path = File.ReadAllText(SettingsFilePath).Trim();
        if (!File.Exists(path))
            return null;

        return path;
    }

    private static void WriteDefaultModelPath(string path)
    {
        File.WriteAllText(SettingsFilePath, path);
    }

    public static string GetModelPath(bool alwaysPrompt = false)
    {
        var defaultPath = ReadDefaultModelPath();
        var path = defaultPath is null || alwaysPrompt
            ? PromptUserForPath()
            : PromptUserForPathWithDefault(defaultPath);

        if (File.Exists(path))
            WriteDefaultModelPath(path);

        return path;
    }

    private static string PromptUserForPath()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Please input your model path:")
               .PromptStyle("white")
               .Validate(File.Exists, "[red]ERROR: invalid model file path - file does not exist[/]")
        );
    }

    private static string PromptUserForPathWithDefault(string defaultPath)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Please input your model path (or ENTER for default):")
               .DefaultValue(defaultPath)
               .PromptStyle("white")
               .Validate(File.Exists, "[red]ERROR: invalid model file path - file does not exist[/]")
        );
    }
}
