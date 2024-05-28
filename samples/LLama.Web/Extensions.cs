using LLama.Web.Common;

namespace LLama.Web;

public static  class Extensions
{
    /// <summary>
    /// Combines the AntiPrompts list and AntiPrompt csv 
    /// </summary>
    /// <param name="sessionConfig">The session configuration.</param>
    /// <returns>Combined AntiPrompts with duplicates removed</returns>
    public static List<string> GetAntiPrompts(this ISessionConfig sessionConfig)
    {
        return CombineCSV(sessionConfig.AntiPrompts, sessionConfig.AntiPrompt);
    }

    /// <summary>
    /// Combines the OutputFilters list and OutputFilter csv 
    /// </summary>
    /// <param name="sessionConfig">The session configuration.</param>
    /// <returns>Combined OutputFilters with duplicates removed</returns>
    public static List<string> GetOutputFilters(this ISessionConfig sessionConfig)
    {
        return CombineCSV(sessionConfig.OutputFilters, sessionConfig.OutputFilter);
    }

    /// <summary>
    /// Combines a string list and a csv and removes duplicates
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="csv">The CSV.</param>
    /// <returns>Combined list with duplicates removed</returns>
    private static List<string> CombineCSV(List<string> list, string csv)
    {
        var results = list is null || list.Count == 0
            ? CommaSeparatedToList(csv)
            : CommaSeparatedToList(csv).Concat(list);
        return results
            .Distinct()
            .ToList();
    }

    private static List<string> CommaSeparatedToList(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<string>();

        return value.Split(",", StringSplitOptions.RemoveEmptyEntries)
             .Select(x => x.Trim())
             .ToList();
    }
}
