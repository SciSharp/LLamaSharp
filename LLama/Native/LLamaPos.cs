namespace LLama.Native;

public record struct LLamaPos
{
    public int Value;

    public LLamaPos(int value)
    {
        Value = value;
    }

    public static explicit operator int(LLamaPos pos) => pos.Value;

    public static explicit operator LLamaPos(int value) => new(value);
}