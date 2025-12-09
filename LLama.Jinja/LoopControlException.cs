namespace LLamaSharp.Jinja;

public class LoopControlException : JinjaException
{
    public readonly LoopControlType ControlType;

    public LoopControlException(string message, LoopControlType controlType)
    : base(message) => ControlType = controlType;

    public LoopControlException(LoopControlType controlType)
        : this($"{(controlType == LoopControlType.Continue ? "continue" : "break")} outside of a loop", controlType)
    {
    }
}