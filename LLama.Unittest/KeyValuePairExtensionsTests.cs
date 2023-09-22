namespace LLama.Unittest;

public class KeyValuePairExtensionsTests
{
    [Fact]
    public void Deconstruct()
    {
        var kvp = new KeyValuePair<int, string>(1, "2");

        var (a, b) = kvp;

        Assert.Equal(1, a);
        Assert.Equal("2", b);
    }
}