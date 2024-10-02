using Spectre.Console;

namespace LLama.Examples;

internal static class UserSettings
{
    private static readonly string SettingsModelPath = Path.Join(AppContext.BaseDirectory, "DefaultModel.env");
    private static readonly string SettingsMMprojPath = Path.Join(AppContext.BaseDirectory, "DefaultMMProj.env");
    private static readonly string SettingsImagePath = Path.Join(AppContext.BaseDirectory, "DefaultImage.env");

    private static string? ReadDefaultPath(string file)
    {
        if (!File.Exists(file))
            return null;

        string path = File.ReadAllText(file).Trim();
        if (!File.Exists(path))
            return null;

        return path;
    }

    private static void WriteDefaultPath(string settings, string path)
    {
        File.WriteAllText(settings, path);
    }

    public static string GetModelPath(bool alwaysPrompt = false)
    {
        var defaultPath = ReadDefaultPath(SettingsModelPath);
        var path = defaultPath is null || alwaysPrompt
            ? PromptUserForPath()
            : PromptUserForPathWithDefault(defaultPath);

        if (File.Exists(path))
            WriteDefaultPath(SettingsModelPath, path);

        return path;
    }
    
    // TODO: Refactorize
    public static string GetMMProjPath(bool alwaysPrompt = false)
    {
        var defaultPath = ReadDefaultPath(SettingsMMprojPath);
        var path = defaultPath is null || alwaysPrompt
            ? PromptUserForPath("MMProj")
            : PromptUserForPathWithDefault(defaultPath, "MMProj");

        if (File.Exists(path))
            WriteDefaultPath(SettingsMMprojPath, path);

        return path;
    }    
    
    // TODO: Refactorize
    public static string GetImagePath(bool alwaysPrompt = false)
    {
        var defaultPath = ReadDefaultPath(SettingsImagePath);
        var path = defaultPath is null || alwaysPrompt
            ? PromptUserForPath("image")
            : PromptUserForPathWithDefault(defaultPath, "image");

        if (File.Exists(path))
            WriteDefaultPath(SettingsImagePath, path);

        return path;
    }    

    private static string PromptUserForPath(string text = "model")
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(string.Format("Please input your {0} path:", text) )
               .PromptStyle("white")
               .Validate(File.Exists, string.Format("[red]ERROR: invalid {0} file path - file does not exist[/]", text) )
        );
    }

    private static string PromptUserForPathWithDefault(string defaultPath, string text = "model")
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(string.Format("Please input your {0} path (or ENTER for default):", text) )
               .DefaultValue(defaultPath)
               .PromptStyle("white")
               .Validate(File.Exists, string.Format("[red]ERROR: invalid {0} file path - file does not exist[/]", text))
        );
    }
}
