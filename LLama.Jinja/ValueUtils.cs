namespace LLamaSharp.Jinja;

internal static class ValueUtils
{
    public static bool In(Value value, Value container)
    {
        var found = false;
        container.ForEach(item =>
        {
            if (!found && item.Equals(value))
                found = true;
        });
        return found;
    }
}

