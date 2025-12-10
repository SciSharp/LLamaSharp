namespace LLamaSharp.Jinja;

internal class ArgumentsValue
{
    public readonly List<Value> Args;
    public readonly List<(string Name, Value Value)> Kwargs;

    public ArgumentsValue()
    {
        Args = [];
        Kwargs = [];
    }

    public void ExpectArgs(string methodName, (int Min, int Max) posCount, (int Min, int Max) kwCount)
    {
        if (Args.Count < posCount.Min || Args.Count > posCount.Max || Kwargs.Count < kwCount.Min || Kwargs.Count > kwCount.Max)
            throw new JinjaException($"{methodName} must have between {posCount.Min} and {posCount.Max} positional arguments, and between {kwCount.Min} and {kwCount.Max} keyword arguments");
    }

    public void ExpectArgs(string methodName, (int Min, int Max) posCount) => ExpectArgs(methodName, posCount, (0, 0));
}

