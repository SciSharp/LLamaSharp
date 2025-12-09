namespace LLamaSharp.Jinja;

public class JinjaException : Exception
{
    internal Location? Location;
    public JinjaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public JinjaException(string message)
        : base(message)
    {
    }

    public string? LocationString => Location?.ToString();
}

