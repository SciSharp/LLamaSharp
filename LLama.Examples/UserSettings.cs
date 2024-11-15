using Spectre.Console;

namespace LLama.Examples;

internal static class UserSettings
{
    private static readonly string SettingsModelPath = Path.Join(AppContext.BaseDirectory, "DefaultModel.env");
    private static readonly string SettingsMMprojPath = Path.Join(AppContext.BaseDirectory, "DefaultMMProj.env");
    private static readonly string SettingsImagePath = Path.Join(AppContext.BaseDirectory, "DefaultImage.env");
    private static readonly string WhisperModelPath = Path.Join(AppContext.BaseDirectory, "DefaultWhisper.env");

    private static string? ReadDefaultPath(string file)
    {
        if (!File.Exists(file))
            return null;

        string path = File.ReadAllText(file).Trim();
        if (!File.Exists(path))
            return null;

        return path;
    }

    public static string GetModelPath(bool alwaysPrompt = false)
    {
        return PromptPath("model.gguf", SettingsModelPath, alwaysPrompt);
    }

    public static string GetMMProjPath(bool alwaysPrompt = false)
    {
        return PromptPath("mmproj", SettingsMMprojPath, alwaysPrompt);
    }    

    public static string GetImagePath(bool alwaysPrompt = false)
    {
        return PromptPath("image", SettingsImagePath, alwaysPrompt);
    }

    public static string GetWhisperPath(bool alwaysPrompt = false)
    {
        return PromptPath("whisper model.bin", WhisperModelPath, alwaysPrompt);
    }

    private static string PromptPath(string label, string saveFile, bool alwaysPrompt)
    {
        var defaultPath = ReadDefaultPath(saveFile);
        var path = defaultPath is null || alwaysPrompt
            ? PromptUserForPath(label)
            : PromptUserForPathWithDefault(defaultPath, label);

        if (File.Exists(path))
            WriteDefaultPath(saveFile, path);

        return path;

        static void WriteDefaultPath(string settings, string path)
        {
            File.WriteAllText(settings, path);
        }

        static string PromptUserForPath(string text = "model")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"Please input your {text} path:")
                   .PromptStyle("white")
                   .Validate(File.Exists, $"[red]ERROR: invalid {text} file path - file does not exist[/]")
            );
        }

        static string PromptUserForPathWithDefault(string defaultPath, string text = "model")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"Please input your {text} path (or ENTER for default):")
                   .DefaultValue(defaultPath)
                   .PromptStyle("white")
                   .Validate(File.Exists, $"[red]ERROR: invalid {text} file path - file does not exist[/]")
            );
        }
}
}
