namespace LLamaSharp.Jinja;

public abstract class TemplateNode
{
    internal readonly Location Location;

    private protected TemplateNode(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        Location = location;
    }

    private protected abstract void DoRender(StringWriter writer, Context context);

    public string Render(Context context)
    {
        var writer = new StringWriter();
        Render(writer, context);
        return writer.ToString();
    }

    internal void Render(StringWriter writer, Context context)
    {
        try
        {
            DoRender(writer, context);
        }
        catch (LoopControlException ex) when (Location.Source is not null)
        {
            throw new LoopControlException(ex.Message + Environment.NewLine + Location.ToString(), ex.ControlType);
        }
        catch (JinjaException ex)
        {
            ex.Location = Location;
            throw;
        }
        catch (Exception ex) when (Location.Source is not null)
        {
            throw new JinjaException(ex.Message + Environment.NewLine + Location.ToString(), ex);
        }
    }
}

