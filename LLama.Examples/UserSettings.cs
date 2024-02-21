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
        string? defaultPath = ReadDefaultModelPath();
        return defaultPath is null || alwaysPrompt
            ? PromptUserForPath()
            : PromptUserForPathWithDefault(defaultPath);
    }

    private static string PromptUserForPath()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Please input your model path: ");
            string? path = Console.ReadLine();

            if (File.Exists(path))
            {
                WriteDefaultModelPath(path);
                return path;
            }

            Console.WriteLine("ERROR: invalid model file path\n");
        }
    }

    private static string PromptUserForPathWithDefault(string defaultPath)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Default model: {defaultPath}");
            Console.Write($"Please input a model path (or ENTER for default): ");
            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                return defaultPath;
            }

            if (File.Exists(path))
            {
                WriteDefaultModelPath(path);
                return path;
            }

            Console.WriteLine("ERROR: invalid model file path\n");
        }
    }
}
