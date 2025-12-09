namespace LLamaSharp.Jinja;

internal abstract class Expression
{
    public readonly Location Location;

    protected Expression(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        Location = location;
    }

    protected abstract Value DoEvaluate(Context context);

    public Value Evaluate(Context context)
    {
        try
        {
            return DoEvaluate(context);
        }
        catch (JinjaException ex)
        {
            ex.Location = Location;
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error evaluating expression at {Location}", ex);
        }
    }
}

