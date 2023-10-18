namespace LLama.Native;

public record struct LLamaSeqId
{
    public int Value;

    public LLamaSeqId(int value)
    {
        Value = value;
    }

    public static explicit operator int(LLamaSeqId pos) => pos.Value;

    public static explicit operator LLamaSeqId(int value) => new(value);
}